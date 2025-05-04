using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace ExperienceService.Swagger
{
    public class SwaggerAuthorizationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get all the authorization attributes from the method and controller
            var authorizeAttributes = context.MethodInfo.GetCustomAttributes<AuthorizeAttribute>(true)
                .Union(context.MethodInfo.DeclaringType.GetCustomAttributes<AuthorizeAttribute>(true));

            // Get all the allow anonymous attributes from the method
            var allowAnonymousAttributes = context.MethodInfo.GetCustomAttributes<AllowAnonymousAttribute>(true);

            if (allowAnonymousAttributes.Any())
            {
                // If method has [AllowAnonymous], add this information to the description
                operation.Description = operation.Description != null
                    ? $"{operation.Description}\n\n**Authorization:** Accessible to anonymous users"
                    : "**Authorization:** Accessible to anonymous users";
                return;
            }

            if (authorizeAttributes.Any())
            {
                // Get the roles from the attributes
                var roles = authorizeAttributes
                    .Where(attr => !string.IsNullOrWhiteSpace(attr.Roles))
                    .SelectMany(attr => attr.Roles.Split(','))
                    .Distinct()
                    .OrderBy(r => r)
                    .ToList();

                var authSummary = roles.Any()
                    ? $"**Required Roles:** {string.Join(", ", roles)}"
                    : "**Authorization:** Requires authentication";

                // Add authorization information to the operation description
                operation.Description = operation.Description != null
                    ? $"{operation.Description}\n\n{authSummary}"
                    : authSummary;

                // Add security requirements
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] { }
                        }
                    }
                };
            }
        }
    }
} 