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
                    // Log with a 500ms timeout
                    var logTask = Task.Run(() => {
                        _logger.Information("Request: {Method} {Path}", 
                            context.Request.Method, 
                            context.Request.Path);
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