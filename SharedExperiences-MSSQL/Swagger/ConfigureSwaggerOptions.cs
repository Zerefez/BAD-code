using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace ExperienceService.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            // Add validation filter
            options.OperationFilter<SwaggerCustomValidationFilter>();
            
            // Add authorization filter
            options.OperationFilter<SwaggerAuthorizationFilter>();

            // Configure JWT Authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            // Add basic information about the API
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Shared Experiences API",
                Version = "v1",
                Description = "API for managing shared experiences with role-based access control",
                Contact = new OpenApiContact
                {
                    Name = "API Support",
                    Email = "support@example.com"
                }
            });
        }
    }
}
