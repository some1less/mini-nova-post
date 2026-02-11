using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniNova.API.Extensions;
using MiniNova.API.Middleware;
using MiniNova.BLL.Generators;
using MiniNova.BLL.Helpers.Options;
using MiniNova.BLL.Interfaces;
using MiniNova.BLL.Security.Auth;
using MiniNova.BLL.Security.Tokens;
using MiniNova.BLL.Services;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;
using MiniNova.DAL.Repositories;
using MiniNova.DAL.Repositories.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ------ LOADING KEYS
var issuerSigningKey = builder.Services.AddAsyncKeyLoading();

// ----- CONFIG ------
var connectionString = builder.Configuration.GetConnectionString("MNPConnection")
    ?? throw new InvalidOperationException("Connection string not found");

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<JwtOptions>() 
                 ?? throw new Exception("JWT options not found");

builder.Services.Configure<JwtOptions>(jwtSection);

// ------ DATABASE ------
builder.Services.AddDbContext<NovaDbContext>(options => 
    options.UseNpgsql(connectionString));

// ----- SERVICES ------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IOperatorRepository, OperatorRepository>();

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IOperatorService, OperatorService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<ITrackingService, TrackingService>();

builder.Services.AddSingleton<ITrackingNumberGeneratorService, TrackingNumberGeneratorService>();

// ----- SECURITY SERVICES ------
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ----- CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // using vite
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            
            IssuerSigningKey = issuerSigningKey,
            
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----- MIDDLEWARE -----

app.UseMiddleware<ExceptionHandlingMiddleware>();

// ----- DATABASE SEEDING -----
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<NovaDbContext>();
        db.Database.EnsureDeleted();
        db.Database.Migrate();
        var path = Path.Combine(AppContext.BaseDirectory, "scripts", "insert.sql");

        if (File.Exists(path))
        {
            var sql = File.ReadAllText(path);
            db.Database.ExecuteSqlRaw(sql);
            Log.Information("Database seeded successfully from SQL file!");
        }
        else
        {
            Log.Warning($"SQL File not found at: {path}. Skipping seed.");
        }
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication(); 

app.UseAuthorization();

app.MapControllers();

app.Run();
