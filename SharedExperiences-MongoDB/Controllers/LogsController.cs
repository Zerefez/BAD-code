using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedExperiences.DTO;
using SharedExperiences.Models;
using SharedExperiences.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedExperiences.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Only admin users can access logs
    public class LogsController : ControllerBase
    {
        private readonly LogService _logService;

        public LogsController(LogService logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// Search logs with filtering options
        /// </summary>
        /// <param name="query">Search parameters</param>
        /// <returns>Paginated log entries</returns>
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResponse<LogEntry>>> SearchLogs([FromQuery] LogSearchQuery query)
        {
            // Add description filter if provided
            if (!string.IsNullOrEmpty(query.Description))
            {
                query.Description = query.Description.Trim();
            }
            
            // Validate HTTP method if provided
            if (!string.IsNullOrEmpty(query.Method))
            {
                query.Method = query.Method.ToUpper();
                if (query.Method != "POST" && query.Method != "PUT" && query.Method != "DELETE")
                {
                    return BadRequest("Method must be POST, PUT, or DELETE");
                }
            }

            // Validate pagination
            if (query.Page < 1) query.Page = 1;
            if (query.PageSize < 1) query.PageSize = 20;
            if (query.PageSize > 100) query.PageSize = 100; // Limit max page size

            // Search logs
            var (logs, totalCount) = await _logService.SearchLogsAsync(query);

            // Create paginated response
            var response = new PaginatedResponse<LogEntry>
            {
                Items = logs,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };

            return Ok(response);
        }

        /// <summary>
        /// Get a summary of log operations grouped by description
        /// </summary>
        [HttpGet("operations")]
        public async Task<ActionResult> GetOperationTypes()
        {
            var operations = await _logService.GetOperationTypesAsync();
            return Ok(operations);
        }
    }
} 