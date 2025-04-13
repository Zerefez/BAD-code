using Microsoft.AspNetCore.Builder;
using Serilog;

namespace SharedExperiences.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, Serilog.ILogger logger)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>(logger);
        }
    }
} 