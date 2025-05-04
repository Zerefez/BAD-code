using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedExperiences.DTO;
using SharedExperiences.Models;
using SharedExperiences.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedExperiences.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Restrict to admin users only
    public class LogsController : ControllerBase
    {
        private readonly LogService _logService;

        public LogsController(LogService logService)
        {
            _logService = logService;
        }

        [HttpGet("diagnostics")]
        public async Task<ActionResult<DiagnosticInfo>> GetDiagnostics()
        {
            var info = await _logService.GetDiagnosticInfoAsync();
            return Ok(info);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<LogEntry>>> SearchLogs(
            [FromQuery] string? userId = null, 
            [FromQuery] string? method = null, 
            [FromQuery] string? description = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = new LogSearchQuery
            {
                UserId = userId,
                Method = method,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                Page = page < 1 ? 1 : page,
                PageSize = pageSize < 1 || pageSize > 100 ? 20 : pageSize
            };
            
            var (logs, totalCount) = await _logService.SearchLogsAsync(query);
            
            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            
            return Ok(new {
                Logs = logs,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalPages = (int)System.Math.Ceiling((double)totalCount / query.PageSize)
            });
        }

        [HttpGet("operation-types")]
        public async Task<ActionResult<IEnumerable<OperationTypeCount>>> GetOperationTypes()
        {
            var counts = await _logService.GetOperationTypesAsync();
            return Ok(counts);
        }
    }
} 