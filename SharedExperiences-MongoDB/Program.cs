using ExperienceService.Data;
using ExperienceService.Services;
using ExperienceService.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using Serilog;
using Serilog.Events;
using SharedExperiences.Middleware;
using MongoDB.Driver;
using SharedExperiences.Services;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Configure Serilog with fallback to Console and File
    var loggerConfig = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logs/fallback-.log", rollingInterval: RollingInterval.Day);

    // Try to add MongoDB logging if available
    try
    {
        var connectionString = builder.Configuration["Serilog:WriteTo:2:Args:databaseUrl"];
        var mongoClient = new MongoClient(connectionString);
        // Test the connection with a 3-second timeout
        mongoClient.GetDatabase("admin").RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}").Wait(3000);
        
        // MongoDB connection works, use full configuration
        loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration);
        
        Console.WriteLine("MongoDB connection successful, using MongoDB logging");
    }
    catch (Exception ex)
    {
        // Failed to connect to MongoDB, use fallback logging
        Console.WriteLine($"Warning: MongoDB connection failed ({ex.Message}). Using fallback logging.");
    }
    
    // Create the logger
    Log.Logger = loggerConfig.CreateLogger();
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllers()
        .AddJsonOptions(options => 
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        });

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", 
            builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });

    // Configure JWT Authentication
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
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };
    });

    // Add Authorization
    builder.Services.AddAuthorization();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

    // Add MongoDB Context
    builder.Services.AddSingleton<MongoDbContext>();

    // Add services
    builder.Services.AddScoped<DbSeeder>();
    builder.Services.AddScoped<ServiceService>();
    builder.Services.AddScoped<ProviderService>();
    builder.Services.AddScoped<SharedExperiencesService>();
    builder.Services.AddScoped<AuthService>();
    
    // Add Log Service for log management
    builder.Services.AddScoped<LogService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

    // Enable CORS
    app.UseCors("AllowAll");
    
    // Use simplified request logging middleware
    app.UseSimpleRequestLogging(Log.Logger);

    // Seed the database only in development
    if (app.Environment.IsDevelopment())
    {
        // Seed the database
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var seeder = services.GetRequiredService<DbSeeder>();
            seeder.Seed();
        }
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Clean up Serilog on application shutdown
    app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}