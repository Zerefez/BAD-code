using ExperienceService.Models;
using ExperienceService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedExperiences.DTO;
using Serilog;
using System.Text.Json;

namespace ExperienceService.Controllers;

/// <summary>
/// Controller for managing shared experiences and related queries
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize] // Default authorization requires authentication
public class SharedExperiencesController : ControllerBase
{
    private readonly SharedExperiencesService _sharedExperiencesService;
    private readonly Serilog.ILogger _logger;

    public SharedExperiencesController(SharedExperiencesService sharedExperiencesService)
    {
        _sharedExperiencesService = sharedExperiencesService;
        _logger = Log.ForContext<SharedExperiencesController>();
    }

    /// <summary>
    /// Get all shared experiences
    /// </summary>
    /// <remarks>
    /// This endpoint requires authentication.
    /// </remarks>
    /// <returns>List of all shared experiences</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SharedExperience>>> GetSharedExperiences()
    {
        var sharedExperiences = await _sharedExperiencesService.GetAllSharedExperiencesAsync();
        return Ok(sharedExperiences);
    }

    /// <summary>
    /// Get a shared experience by ID
    /// </summary>
    /// <param name="id">Shared Experience ID</param>
    /// <returns>Shared experience details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<SharedExperience>> GetSharedExperience(int id)
    {
        var sharedExperience = await _sharedExperiencesService.GetSharedExperienceByIdAsync(id);

        if (sharedExperience == null)
        {
            return NotFound();
        }

        return Ok(sharedExperience);
    }

    /// <summary>
    /// Create a new shared experience
    /// </summary>
    /// <param name="dateDto">Shared experience date data</param>
    /// <returns>Created shared experience details</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<SharedExperience>> CreateSharedExperience([FromBody] SharedExperienceDateDto dateDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.Warning("Invalid model state when creating shared experience: {@ModelErrors}", 
                ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        _logger.Information("Creating new shared experience: {@SharedExperienceData}", dateDto);
        
        var createdSharedExperience = await _sharedExperiencesService.CreateSharedExperienceAsync(dateDto);
        
        _logger.Information("Successfully created shared experience with ID: {SharedExperienceId}", 
            createdSharedExperience.SharedExperienceId);
        
        return CreatedAtAction(nameof(GetSharedExperience), new { id = createdSharedExperience.SharedExperienceId }, createdSharedExperience);
    }

    /// <summary>
    /// Update an existing shared experience
    /// </summary>
    /// <param name="id">Shared Experience ID</param>
    /// <param name="dateDto">Updated shared experience data</param>
    /// <returns>No content if successful</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateSharedExperience(int id, [FromBody] SharedExperienceDateDto dateDto)
    {
        if (id != dateDto.SharedExperienceId)
        {
            _logger.Warning("ID mismatch when updating shared experience. Route ID: {RouteId}, DTO ID: {DtoId}", 
                id, dateDto.SharedExperienceId);
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            _logger.Warning("Invalid model state when updating shared experience ID {Id}: {@ModelErrors}", 
                id, ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        _logger.Information("Updating shared experience with ID {Id}: {@UpdatedData}", id, dateDto);
        
        var updatedSharedExperience = await _sharedExperiencesService.UpdateSharedExperienceAsync(id, dateDto);
        if (updatedSharedExperience == null)
        {
            _logger.Warning("Shared experience with ID {Id} not found when trying to update", id);
            return NotFound();
        }

        _logger.Information("Successfully updated shared experience with ID: {Id}", id);
        return NoContent();
    }

    /// <summary>
    /// Delete a shared experience
    /// </summary>
    /// <param name="id">Shared Experience ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteSharedExperience(int id)
    {
        _logger.Information("Attempting to delete shared experience with ID: {Id}", id);
        
        var deleted = await _sharedExperiencesService.DeleteSharedExperienceAsync(id);
        if (!deleted)
        {
            _logger.Warning("Shared experience with ID {Id} not found when trying to delete", id);
            return NotFound();
        }

        _logger.Information("Successfully deleted shared experience with ID: {Id}", id);
        return NoContent();
    }

    /// <summary>
    /// Providers with most services (Manager/Admin only)
    /// </summary>
    /// <remarks>
    /// This endpoint is restricted to Manager and Admin roles only.
    /// </remarks>
    /// <returns>List of providers ranked by number of services</returns>
    [HttpGet("Table1")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<IEnumerable<object>>> Table1()
    {
        var providers = await _sharedExperiencesService.Table1();
        return Ok(providers);
    }

    /// <summary>
    /// Services with their providers (Public)
    /// </summary>
    /// <remarks>
    /// This endpoint is accessible to anonymous users (no authentication required).
    /// </remarks>
    /// <returns>List of services with provider information</returns>
    [HttpGet("Table2")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<object>>> Table2()
    {
        var providers = await _sharedExperiencesService.Table2();
        return Ok(providers);
    }
    
    /// <summary>
    /// Services grouped by provider (Public)
    /// </summary>
    /// <remarks>
    /// This endpoint is accessible to anonymous users (no authentication required).
    /// </remarks>
    /// <returns>List of providers with their services</returns>
    [HttpGet("Table3")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<object>>> Table3()
    {
        var providers = await _sharedExperiencesService.Table3();
        return Ok(providers);
    }

    /// <summary>
    /// Shared experience details (Manager/Admin only)
    /// </summary>
    /// <remarks>
    /// This endpoint is restricted to Manager and Admin roles only.
    /// </remarks>
    /// <param name="sharedExperienceId">Shared Experience ID</param>
    /// <returns>Detailed information about the shared experience</returns>
    [HttpGet("Table4")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<IEnumerable<object>>> Table4(int sharedExperienceId)
    {
        var providers = await _sharedExperiencesService.Table4(sharedExperienceId);
        return Ok(providers);
    }   

    /// <summary>
    /// Guests in shared experience (Public)
    /// </summary>
    /// <remarks>
    /// This endpoint is accessible to anonymous users (no authentication required).
    /// </remarks>
    /// <param name="sharedExperienceId">Shared Experience ID</param>
    /// <returns>List of guests participating in the shared experience</returns>
    [HttpGet("Table5")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<object>>> Table5(int sharedExperienceId)
    {
        var providers = await _sharedExperiencesService.Table5(sharedExperienceId);
        return Ok(providers);
    }

    /// <summary>
    /// Billing information (Manager/Admin only)
    /// </summary>
    /// <remarks>
    /// This endpoint is restricted to Manager and Admin roles only.
    /// </remarks>
    /// <param name="serviceId">Service ID</param>
    /// <returns>Billing information for the service</returns>
    [HttpGet("Table6")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<IEnumerable<object>>> Table6(int serviceId)
    {
        var providers = await _sharedExperiencesService.Table6(serviceId);
        return Ok(providers);
    }

    /// <summary>
    /// Financial summary (Manager/Admin only)
    /// </summary>
    /// <remarks>
    /// This endpoint is restricted to Manager and Admin roles only.
    /// </remarks>
    /// <returns>Financial summary of services</returns>
    [HttpGet("Table7")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<IEnumerable<object>>> Table7()
    {
        var providers = await _sharedExperiencesService.Table7();
        return Ok(providers);
    }

    /// <summary>
    /// Discount information (Manager/Admin only)
    /// </summary>
    /// <remarks>
    /// This endpoint is restricted to Manager and Admin roles only.
    /// </remarks>
    /// <returns>Discount information for services</returns>
    [HttpGet("Table8")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<IEnumerable<object>>> Table8()
    {
        var providers = await _sharedExperiencesService.Table8();
        return Ok(providers);
    }

    /// <summary>
    /// General service information (Authenticated)
    /// </summary>
    /// <remarks>
    /// This endpoint requires authentication (any role).
    /// </remarks>
    /// <returns>General service information</returns>
    [HttpGet("Table9")]
    public async Task<ActionResult<IEnumerable<object>>> Table9()
    {
        var providers = await _sharedExperiencesService.Table9();
        return Ok(providers);
    }
}