using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;

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
            
            // Log both write operations (POST/PUT/DELETE) and sensitive GET operations
            if (_loggingEnabled && (IsMethodToLog(context.Request.Method) || IsPathToLog(context.Request.Path, context.Request.Method)))
            {
                try
                {
                    // Get the operation description based on the endpoint and method
                    string description = GenerateOperationDescription(context.Request.Method, context.Request.Path);
                    
                    // Extract user ID and role if authenticated
                    string userId = "anonymous";
                    string userRole = "none";
                    string userEmail = "unknown";
                    
                    if (context.User?.Identity?.IsAuthenticated == true)
                    {
                        // Get username from claims
                        userId = context.User.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "anonymous";
                            
                        // Get role information
                        userRole = context.User.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "none";
                            
                        // Get email if available
                        userEmail = context.User.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "unknown";
                    }
                    
                    // Log with a 500ms timeout
                    var logTask = Task.Run(() => {
                        // Create log with user details for filtering
                        _logger
                            .ForContext("UserId", userId)
                            .ForContext("UserRole", userRole)
                            .ForContext("UserEmail", userEmail)
                            .ForContext("RequestId", context.TraceIdentifier)
                            .ForContext("RequestPath", context.Request.Path)
                            .ForContext("Method", context.Request.Method)
                            .Information("Request: {Method} {Path} - {Description} by user {UserId} with role {UserRole}", 
                                context.Request.Method, 
                                context.Request.Path,
                                description,
                                userId,
                                userRole);
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
            
            // Auth operations
            if (pathValue.Contains("/api/auth/login"))
                return "User login";
                
            if (pathValue.Contains("/api/auth/register"))
                return "User registration";
                
            if (pathValue.Contains("/api/auth/roles"))
                return "Role management";
                
            // Log operations
            if (pathValue.Contains("/api/logs"))
                return "Log access";
                
            // Service-related operations
            if (pathValue.Contains("/api/service"))
            {
                if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                    return "Creating new service";
                    
                if (method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
                    return "Updating existing service";
                    
                if (method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                    return "Deleting service";
                    
                if (method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    return "Viewing service details";
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
                    
                if (method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    return "Viewing provider details";
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
                    
                if (method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    return "Viewing shared experience";
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
                case "GET":
                    return "Accessing resource";
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
            // Always log write operations
            return method.Equals("POST", StringComparison.OrdinalIgnoreCase) || 
                   method.Equals("PUT", StringComparison.OrdinalIgnoreCase) || 
                   method.Equals("DELETE", StringComparison.OrdinalIgnoreCase);
        }
        
        private bool IsPathToLog(PathString path, string method)
        {
            // Only log GET requests to sensitive endpoints
            if (!method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                return false;
                
            string pathValue = path.Value?.ToLower() ?? string.Empty;
            
            // Log access to sensitive data
            return pathValue.Contains("/api/logs") ||
                   pathValue.Contains("/api/users") ||
                   pathValue.Contains("/api/admin") ||
                   pathValue.Contains("/api/auth/roles");
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