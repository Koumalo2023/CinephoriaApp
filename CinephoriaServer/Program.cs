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
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables() 
    .AddUserSecrets<Program>(optional: true);


// Configuration de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/CinephoriaLog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configuration de la base de donn�es
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<CinephoriaDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure())
               .EnableSensitiveDataLogging() // Active l'affichage des valeurs sensibles dans les logs
               .LogTo(Console.WriteLine, LogLevel.Information)); // Journalisation dans la console
}
else
{
    builder.Services.AddDbContext<CinephoriaDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlProd"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure())
               .EnableSensitiveDataLogging() // Active l'affichage des valeurs sensibles dans les logs  en production
               .LogTo(Console.WriteLine, LogLevel.Warning)); // Journalisation des avertissements et erreurs en production
}

//Configuration de MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// Ajout de MongoDbContext en tant que service Singleton
builder.Services.AddSingleton<MongoDbContext>();

// Enregistrement d'IMongoDatabase dans le conteneur DI
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
var client = new MongoClient(settings.ConnectionString);
return client.GetDatabase(settings.DatabaseName);
});

// Ajout de  Identity
builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<CinephoriaDbContext>()
    .AddDefaultTokenProviders();

// Configuration de Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});

// G�rer les injections de d�pendances
builder.Services.AddDbServiceInjection();

// Exiger la confirmation d'email
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmailConfirmed", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "EmailConfirmed" && c.Value == "true")));
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
}); 

//Configuration d'AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add AuthenticationSchema and JwtBearer
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });



//Documentation swagger
builder.Services.AddSwaggerGen(options =>
{
    // Informations g�n�rales sur l'API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Cinephoria API Documentation",
        Description = "Comprehensive API documentation for the Web, Mobile, and Desktop applications."
    });

    // Configuration pour l'authentification JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Description = "Please enter your token as follows: 'Bearer YOUR_TOKEN'",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer",
    });

    builder.Services.AddHttpContextAccessor();

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

    // Support de fichiers avec IFormFile
    options.OperationFilter<SwaggerFileOperationFilter>();

    // Inclusion des commentaires XML (optionnel si d�j� configur�)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.OrderActionsBy((apiDesc) => $"{apiDesc.HttpMethod} {apiDesc.RelativePath}");
});

// Ajouter la configuration de s�curit� (incluant les CORS) en utilisant SecurityExtensions
builder.Services.AddCustomSecurity(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



// Configuration Kestrel pour utiliser HTTPS avec le certificat sp�cifi�
builder.WebHost.ConfigureKestrel((context, options) =>
{
    // Charger les param�tres depuis appsettings.json
    options.Configure(context.Configuration.GetSection("Kestrel"));
});



var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Appliquez la politique CORS
app.UseCors(SecurityExtensions.DEFAULT_POLICY);
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath, "images")),
    RequestPath = "/images"
});
app.UseAuthorization();
app.MapControllers();

//Ex�cutez la m�thode de seeding d'administrateur lors du d�marrage de l'application
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Initialisez les r�les et utilisateurs avec SeedAdmin
        await SeedAdmin.Initialize(services, userManager, roleManager);
    }
    catch (Exception ex)
    {
        // Loggez l'exception si n�cessaire
        Console.WriteLine("Erreur lors de l'initialisation de l'administrateur par d�faut : " + ex.Message);
    }
}
app.Run();
