using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SharedExperiences.Models.Validation;

namespace ExperienceService.Swagger
{
    public class PasswordValidationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Find parameters that use PasswordValidationAttribute
            var parameters = context.MethodInfo.GetParameters();
            
            foreach (var parameter in parameters)
            {
                var properties = parameter.ParameterType.GetProperties();
                
                foreach (var property in properties)
                {
                    var passwordAttr = property.GetCustomAttributes<PasswordValidationAttribute>(true).FirstOrDefault();
                    
                    if (passwordAttr != null)
                    {
                        // Find the schema for this property in the request body
                        if (operation.RequestBody?.Content.TryGetValue("application/json", out var content) == true)
                        {
                            if (content.Schema?.Properties.TryGetValue(property.Name.ToLower(), out var propSchema) == true)
                            {
                                // Add description with password requirements
                                propSchema.Description = GetPasswordRequirementsDescription(passwordAttr);
                            }
                        }
                        
                        // Also add to the 400 response description
                        if (!operation.Responses.ContainsKey("400"))
                        {
                            operation.Responses.Add("400", new OpenApiResponse
                            {
                                Description = "Bad Request"
                            });
                        }
                        
                        var description = operation.Responses["400"].Description ?? "Bad Request";
                        if (!description.Contains("Password requirements"))
                        {
                            operation.Responses["400"].Description = description + 
                                "\r\n\r\nPassword requirements: " + GetPasswordRequirementsDescription(passwordAttr);
                        }
                    }
                }
            }
        }
        
        private string GetPasswordRequirementsDescription(PasswordValidationAttribute attr)
        {
            // Use reflection to get the attribute's field values
            var fields = attr.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
            int minLength = 8; // Default
            bool requireUppercase = true;
            bool requireLowercase = true;
            bool requireDigit = true;
            bool requireSpecialChar = true;
            
            foreach (var field in fields)
            {
                switch (field.Name)
                {
                    case "_minLength":
                        minLength = (int)field.GetValue(attr);
                        break;
                    case "_requireUppercase":
                        requireUppercase = (bool)field.GetValue(attr);
                        break;
                    case "_requireLowercase":
                        requireLowercase = (bool)field.GetValue(attr);
                        break;
                    case "_requireDigit":
                        requireDigit = (bool)field.GetValue(attr);
                        break;
                    case "_requireSpecialChar":
                        requireSpecialChar = (bool)field.GetValue(attr);
                        break;
                }
            }
            
            // Build the description
            var description = $"Must be at least {minLength} characters long";
            var requirements = new System.Collections.Generic.List<string>();
            
            if (requireUppercase) requirements.Add("uppercase letters");
            if (requireLowercase) requirements.Add("lowercase letters");
            if (requireDigit) requirements.Add("digits");
            if (requireSpecialChar) requirements.Add("special characters");
            
            if (requirements.Any())
            {
                description += $" and include {string.Join(", ", requirements)}";
            }
            
            return description;
        }
    }
    
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check for authorize attribute
            var authorizeAttributes = context.MethodInfo.GetCustomAttributes<AuthorizeAttribute>(true)
                .Union(context.MethodInfo.DeclaringType.GetCustomAttributes<AuthorizeAttribute>(true));
                
            if (!authorizeAttributes.Any())
                return;
                
            // Add authorization responses
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
            
            // Check for role-based authorization
            var roleAttributes = authorizeAttributes
                .Where(attr => !string.IsNullOrEmpty(attr.Roles))
                .Select(attr => attr.Roles)
                .ToList();
                
            if (roleAttributes.Any())
            {
                string roles = string.Join(", ", roleAttributes);
                operation.Summary = operation.Summary + $" [Requires roles: {roles}]";
            }
        }
    }
} 