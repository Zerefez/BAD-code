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
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on all interfaces when in Docker
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    if (builder.Environment.IsDevelopment())
    {
        serverOptions.ListenAnyIP(5195); // Development port
    }
    else
    {
        serverOptions.ListenAnyIP(80);
        serverOptions.ListenAnyIP(443);
    }
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

    // Add Health Checks
    builder.Services.AddHealthChecks()
        .AddSqlServer(
            connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
            name: "sqlserver",
            tags: new[] { "db", "sql", "sqlserver" })
        .AddMongoDb(
            mongodbConnectionString: builder.Configuration.GetConnectionString("MongoDB"),
            name: "mongodb",
            tags: new[] { "db", "nosql", "mongodb" });

    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c => 
    {
        // Set the comments path for the Swagger JSON and UI
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
        else
        {
            Console.WriteLine($"Warning: XML documentation file not found at {xmlPath}");
        }
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

    // Apply migrations and seed the database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var maxRetries = 3;
        var retryDelay = TimeSpan.FromSeconds(10);
        
        for (int retryCount = 0; retryCount < maxRetries; retryCount++)
        {
            try
            {
                var context = services.GetRequiredService<SharedExperiencesDbContext>();
                
                // Apply pending migrations
                context.Database.Migrate();
                Log.Information("Database migrations applied successfully");
                
                // Seed the database with roles, users, and business data
                var seeder = services.GetRequiredService<DbSeeder>();
                await seeder.SeedAsync();
                Log.Information("Database seeded successfully");
                
                // If we get here, the seeding was successful, so break out of the retry loop
                break;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while migrating or seeding the database (attempt {RetryCount} of {MaxRetries})", 
                    retryCount + 1, maxRetries);
                
                if (retryCount < maxRetries - 1)
                {
                    Log.Information("Waiting {RetryDelay} seconds before retrying...", retryDelay.TotalSeconds);
                    await Task.Delay(retryDelay);
                }
                else
                {
                    // This was the last retry attempt
                    Log.Error("Failed to seed the database after {MaxRetries} attempts", maxRetries);
                    if (!builder.Environment.IsDevelopment())
                    {
                        // In production, we want to fail fast
                        throw;
                    }
                    // In development, we'll continue despite the seeding failure
                }
            }
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
    
    // Map health check endpoint
    app.MapHealthChecks("/health");

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