using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Threading.Tasks;

namespace SharedExperiences.Middleware
{
    public class SimpleRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;
        private static bool _loggingEnabled = true;
        private static DateTime _lastWarningTime = DateTime.MinValue;
        
        public SimpleRequestLoggingMiddleware(RequestDelegate next, Serilog.ILogger logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            // Always call next middleware first to avoid blocking the pipeline
            var nextTask = _next(context);
            
            // Only attempt to log if logging is enabled and it's a POST, PUT, DELETE request
            if (_loggingEnabled && IsMethodToLog(context.Request.Method))
            {
                try
                {
                    // Get the operation description based on the endpoint and method
                    string description = GenerateOperationDescription(context.Request.Method, context.Request.Path);
                    
                    // Log with a 500ms timeout
                    var logTask = Task.Run(() => {
                        _logger.Information("Request: {Method} {Path} - {Description}", 
                            context.Request.Method, 
                            context.Request.Path,
                            description);
                    });
                    
                    // Wait for logging with timeout to avoid freezing
                    if (!logTask.Wait(500))
                    {
                        // Logging timed out, disable logging temporarily
                        _loggingEnabled = false;
                        LogWarning("Logging timed out, disabling temporarily.");
                        
                        // Re-enable logging after 30 seconds
                        Task.Run(async () => {
                            await Task.Delay(30000);
                            _loggingEnabled = true;
                        });
                    }
                }
                catch (Exception ex)
                {
                    // Log exception but don't stop the request pipeline
                    LogWarning($"Error during request logging: {ex.Message}");
                }
            }
            
            // Wait for the next middleware to complete
            await nextTask;
        }
        
        private string GenerateOperationDescription(string method, PathString path)
        {
            string pathValue = path.Value?.ToLower() ?? string.Empty;
            
            // Common patterns
            if (pathValue.Contains("/api/auth/login"))
                return "User login";
                
            if (pathValue.Contains("/api/auth/register"))
                return "User registration";
            
            // Service-related operations
            if (pathValue.Contains("/api/service"))
            {
                if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                    return "Creating new service";
                    
                if (method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
                    return "Updating existing service";
                    
                if (method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                    return "Deleting service";
            }
            
            // Provider-related operations
            if (pathValue.Contains("/api/provider"))
            {
                if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                    return "Creating new provider";
                    
                if (method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
                    return "Updating provider information";
                    
                if (method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                    return "Deleting provider";
            }
            
            // Shared experiences-related operations
            if (pathValue.Contains("/api/sharedexperiences"))
            {
                if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                    return "Creating new shared experience";
                    
                if (method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
                    return "Updating shared experience";
                    
                if (method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                    return "Deleting shared experience";
            }
            
            // Default descriptions based on method
            switch (method.ToUpper())
            {
                case "POST":
                    return "Creating new resource";
                case "PUT":
                    return "Updating existing resource";
                case "DELETE":
                    return "Deleting resource";
                default:
                    return "Performing operation";
            }
        }
        
        private void LogWarning(string message)
        {
            // Limit warnings to once per 30 seconds to avoid console spam
            if ((DateTime.Now - _lastWarningTime).TotalSeconds > 30)
            {
                _lastWarningTime = DateTime.Now;
                Console.WriteLine($"[WARNING] {DateTime.Now:HH:mm:ss} - {message}");
            }
        }
        
        private bool IsMethodToLog(string method)
        {
            return method.Equals("POST", StringComparison.OrdinalIgnoreCase) || 
                   method.Equals("PUT", StringComparison.OrdinalIgnoreCase) || 
                   method.Equals("DELETE", StringComparison.OrdinalIgnoreCase);
        }
    }
    
    // Extension method to add the middleware
    public static class SimpleRequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseSimpleRequestLogging(this IApplicationBuilder builder, Serilog.ILogger logger)
        {
            return builder.UseMiddleware<SimpleRequestLoggingMiddleware>(logger);
        }
    }
} 