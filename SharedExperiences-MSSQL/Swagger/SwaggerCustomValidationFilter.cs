using ExperienceService.Models.Validators;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace ExperienceService.Swagger
{
    public class SwaggerCustomValidationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var parameters = context.MethodInfo.GetParameters();
            
            foreach (var parameter in parameters)
            {
                var properties = parameter.ParameterType.GetProperties();
                
                foreach (var property in properties)
                {
                    var customAttributes = property.GetCustomAttributes(typeof(PositivePriceAttribute), true);
                    
                    if (customAttributes.Length > 0)
                    {
                        foreach (var response in operation.Responses)
                        {
                            if (response.Key == "400")
                            {
                                if (response.Value.Description == null)
                                {
                                    response.Value.Description = "";
                                }
                                
                                response.Value.Description += $"Price must be a positive value for {property.Name}. ";
                            }
                        }
                    }
                }
            }
        }
    }
}