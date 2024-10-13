using CinephoriaBackEnd.Data;
using CinephoriaServer.Configurations;
using CinephoriaServer.Data;
using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()  // Pour lire les variables d'environnement
    .AddUserSecrets<Program>(optional: true);  // Utilisé en développement

// Configuration de la base de données
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<CinephoriaDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));
}
else
{
    builder.Services.AddDbContext<CinephoriaDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlProd"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));
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

// Gérer les injections de dépendances
builder.Services.AddDbServiceInjection();


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
    // Informations générales sur l'API
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

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);


    options.OrderActionsBy((apiDesc) => $"{apiDesc.HttpMethod} {apiDesc.RelativePath}");

});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
