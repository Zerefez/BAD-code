
using ExperienceService.Data;
using ExperienceService.Services;
using ExperienceService.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// Add DbContext
var connectionString = "Data Source=127.0.0.1,1433;Database=SharedExperincesDB;User Id=sa;Password=Yro..2452004;TrustServerCertificate=True";


Console.WriteLine($"Connection string: {connectionString}");

builder.Services.AddDbContext<SharedExperiencesDbContext>(options =>
	options.UseSqlServer(connectionString));

// Add services
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddScoped<ServiceService>();
builder.Services.AddScoped<ProviderService>();
builder.Services.AddScoped<SharedExperiencesService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Apply migrations and seed the database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<SharedExperiencesDbContext>();
        
        // Apply pending migrations
        context.Database.Migrate();
        
        // Seed the database
        var seeder = services.GetRequiredService<DbSeeder>();
        seeder.Seed();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();