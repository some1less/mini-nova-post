using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.Interfaces;
using MiniNova.BLL.Services;
using MiniNova.DAL.Context;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MNPConnection")
    ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

builder.Services.AddDbContext<NovaDbContext>(options => 
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IOperatorService, OperatorService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<ITrackingService, TrackingService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// for inserting sample data to dabase
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

app.UseAuthorization();

app.MapControllers();

app.Run();
