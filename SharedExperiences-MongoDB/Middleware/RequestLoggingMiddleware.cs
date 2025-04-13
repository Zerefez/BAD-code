using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SharedExperiences.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly Serilog.ILogger _logger;
        
        public RequestLoggingMiddleware(RequestDelegate next, Serilog.ILogger logger)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Only log POST, PUT, DELETE requests
                if (IsLogRequiredMethod(context.Request.Method))
                {
                    await LogRequest(context);
                }
                
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in request logging middleware");
                await _next(context);
            }
        }
        
        private async Task LogRequest(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                
                string requestBody = string.Empty;
                // Only read the body if it has content
                if (context.Request.ContentLength > 0)
                {
                    requestBody = await ReadRequestBody(context.Request);
                }
                
                _logger.Information("HTTP {RequestMethod} {RequestPath} {RequestBody}", 
                    context.Request.Method, 
                    context.Request.Path, 
                    requestBody);
                    
                // Reset the request stream position for next middleware
                context.Request.Body.Position = 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error logging request");
            }
        }
        
        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            try
            {
                using (var requestStream = _recyclableMemoryStreamManager.GetStream())
                {
                    await request.Body.CopyToAsync(requestStream);
                    requestStream.Position = 0;
                    
                    using (var reader = new StreamReader(requestStream))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error reading request body");
                return "[Error reading request body]";
            }
        }
        
        private bool IsLogRequiredMethod(string method)
        {
            return method.Equals("POST", StringComparison.OrdinalIgnoreCase) || 
                   method.Equals("PUT", StringComparison.OrdinalIgnoreCase) || 
                   method.Equals("DELETE", StringComparison.OrdinalIgnoreCase);
        }
    }
} 