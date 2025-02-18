using CinephoriaBackEnd.Data;
using CinephoriaServer.Configurations;
using CinephoriaServer.Configurations.Extensions;
using CinephoriaServer.Data;
using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using System.Reflection;
using System.Text;


// Configurer le chemin racine pour les fichiers statiques si besoin
var options = new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory,
    WebRootPath = "wwwroot"
};
var builder = WebApplication.CreateBuilder(options);

// Chargement dynamique des configurations
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(optional: true);

// Configuration de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/CinephoriaLog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// Ajout du contexte de base de données
builder.Services.AddDbContext<CinephoriaDbContext>(options =>
{
    var connectionStringKey = builder.Environment.IsDevelopment() ? "PostgreSql" : "cinephoriaapp-db";
    options.UseNpgsql(builder.Configuration.GetConnectionString(connectionStringKey),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure())
          .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
          .LogTo(Console.WriteLine, LogLevel.Information);
});

// Configuration de MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = new MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});

// Ajout de Identity
builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<CinephoriaDbContext>()
    .AddDefaultTokenProviders();

// Configuration globale d'Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = true; // Exigence générale de confirmation d'email
});

// Ajout de l'autorisation avec politique personnalisée
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmailConfirmed", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "EmailConfirmed" && c.Value == "true")));
});

// Gestion des injections de dépendances
builder.Services.AddDbServiceInjection();

// Configuration d'AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configuration de l'authentification JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });

// Documentation Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Cinephoria API Documentation",
        Description = "Comprehensive API documentation for the Web, Mobile, and Desktop applications."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Description = "Please enter your token as follows: 'Bearer YOUR_TOKEN'",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    options.OperationFilter<SwaggerFileOperationFilter>();

    // Inclusion des commentaires XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    options.OrderActionsBy(apiDesc => $"{apiDesc.HttpMethod} {apiDesc.RelativePath}");
});

// Ajout de la sécurité CORS
builder.Services.AddCustomSecurity(builder.Configuration);

// Configuration Kestrel pour utiliser HTTPS avec le certificat spécifié
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

// Ajout des services MVC
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Construction de l'application
var app = builder.Build();

// Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(SecurityExtensions.DEFAULT_POLICY);
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configuration des fichiers statiques pour /images
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath, "images")),
    RequestPath = "/images"
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Initialisation de l'utilisateur administrateur par défaut
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedAdmin.Initialize(services, userManager, roleManager);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erreur lors de l'initialisation de l'administrateur par défaut : " + ex.Message);
    }
}

// Démarrage de l'application
app.Run();