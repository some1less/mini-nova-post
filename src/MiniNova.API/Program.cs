using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniNova.BLL.Helpers.Options;
using MiniNova.BLL.Interfaces;
using MiniNova.BLL.Security.Auth;
using MiniNova.BLL.Security.Tokens;
using MiniNova.BLL.Services;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;

var builder = WebApplication.CreateBuilder(args);


// ----- CONFIG ------
var connectionString = builder.Configuration.GetConnectionString("MNPConnection")
    ?? throw new InvalidOperationException("Connection string not found");

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<JwtOptions>() 
                 ?? throw new Exception("JWT options not found");

builder.Services.Configure<JwtOptions>(jwtSection);

// ------ DATABASE ------
builder.Services.AddDbContext<NovaDbContext>(options => 
    options.UseSqlite(connectionString));

// ----- SERVICES ------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IOperatorService, OperatorService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<ITrackingService, TrackingService>();

// ----- SECURITY SERVICES ------
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ------ AUTHENTICATION (JWT) -----
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----- DATABASE SEEDING -----
using (var scope = app.Services.CreateScope())
{
    var db =  scope.ServiceProvider.GetRequiredService<NovaDbContext>();
    db.Database.EnsureDeleted();
    db.Database.Migrate();
    var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "scripts", "insert.sql");
    
    if (File.Exists(path))
    {
        var sql = File.ReadAllText(path);
        db.Database.ExecuteSqlRaw(sql);
        Console.WriteLine("Database seeded successfully from SQL file!");
    }
    else
    {
        Console.WriteLine($"SQL File not found at: {path}");
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 

app.UseAuthorization();

app.MapControllers();

app.Run();
