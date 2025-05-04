using ExperienceService.Data;
using ExperienceService.Models;
using ExperienceService.Services;
using ExperienceService.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Serilog;
using Serilog.Events;
using SharedExperiences.Middleware;
using MongoDB.Driver;
using SharedExperiences.Services;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on all interfaces when in Docker
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
    serverOptions.ListenAnyIP(443);
});

try
{
    // Configure Serilog with fallback to Console and File
    var loggerConfig = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logs/fallback-.log", rollingInterval: RollingInterval.Day);

    // Try to add MongoDB logging if available
    try
    {
        var mongoConnectionString = builder.Configuration["Serilog:WriteTo:2:Args:databaseUrl"];
        var mongoClient = new MongoClient(mongoConnectionString);
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

    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c => 
    {
        // Set the comments path for the Swagger JSON and UI
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

    // Add DbContext
    var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"SQL Connection string: {sqlConnectionString}");
    builder.Services.AddDbContext<SharedExperiencesDbContext>(options =>
        options.UseSqlServer(sqlConnectionString));

    // Configure Identity
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<SharedExperiencesDbContext>()
        .AddDefaultTokenProviders();

    // Configure JWT Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });

    // Configure Identity options
    builder.Services.Configure<IdentityOptions>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;

        // User settings
        options.User.RequireUniqueEmail = true;
    });

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
    // Enable Swagger for all environments
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shared Experiences API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Collapse operations by default
        c.DefaultModelsExpandDepth(-1); // Hide schemas section
        c.DisplayRequestDuration(); // Show request duration
    });

    if (app.Environment.IsDevelopment())
    {
        // Apply migrations and seed the database
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<SharedExperiencesDbContext>();
            
            // Apply pending migrations
            context.Database.Migrate();
            
            // Seed the database with roles, users, and business data
            var seeder = services.GetRequiredService<DbSeeder>();
            await seeder.SeedAsync();
        }
    }

    // Enable CORS
    app.UseCors("AllowAll");
    
    // Use simplified request logging middleware
    app.UseSimpleRequestLogging(Log.Logger);

    app.UseHttpsRedirection();

    // Add Authentication middleware
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