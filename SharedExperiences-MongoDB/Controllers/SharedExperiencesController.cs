using ExperienceService.Models;
using ExperienceService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedExperiences.DTO;

namespace ExperienceService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SharedExperiencesController : ControllerBase
{
    private readonly SharedExperiencesService _sharedExperiencesService;

    public SharedExperiencesController(SharedExperiencesService sharedExperiencesService)
    {
        _sharedExperiencesService = sharedExperiencesService;
    }

    // GET: api/SharedExperiences
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<Provider>>> GetProviders()
    {
        var providers = await _sharedExperiencesService.GetAllProvidersAsync();
        return Ok(providers);
    }

    // GET: api/SharedExperiences/5
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<Provider>> GetProvider(string id)
    {
        var provider = await _sharedExperiencesService.GetProviderByIdAsync(id);

        if (provider == null)
        {
            return NotFound();
        }

        return provider;
    }

    // POST: api/SharedExperiences
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<Provider>> CreateProvider([FromBody] CreateAndUpdateProviderDto providerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var provider = new Provider
        {
            Name = providerDto.Name,
            Address = providerDto.Address,
            Number = providerDto.Number,
            TouristicOperatorPermit = providerDto.TouristicOperatorPermit
        };

        var createdProvider = await _sharedExperiencesService.CreateProviderAsync(provider);
        return CreatedAtAction(nameof(GetProvider), new { id = createdProvider.Id }, createdProvider);
    }

    // PUT: api/SharedExperiences/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateProvider(string id, [FromBody] CreateAndUpdateProviderDto providerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Get the existing provider first
        var existingProvider = await _sharedExperiencesService.GetProviderByIdAsync(id);
        if (existingProvider == null)
        {
            return NotFound();
        }

        // Update properties from DTO
        existingProvider.Name = providerDto.Name;
        existingProvider.Address = providerDto.Address;
        existingProvider.Number = providerDto.Number;
        existingProvider.TouristicOperatorPermit = providerDto.TouristicOperatorPermit;

        var updatedProvider = await _sharedExperiencesService.UpdateProviderAsync(id, existingProvider);
        if (updatedProvider == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/SharedExperiences/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteProvider(string id)
    {
        var deleted = await _sharedExperiencesService.DeleteProviderAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    // Table 1 - Get the data collected for each experience provider.
    [HttpGet("Table1")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<object>>> Table1()
    {
        var results = await _sharedExperiencesService.Table1();
        return Ok(results);
    }

    // Table 2 - List the experiences/services available in the system.
    [HttpGet("Table2")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<object>>> Table2()
    {
        var results = await _sharedExperiencesService.Table2();
        return Ok(results);
    }
    
    // Table 3 - Get the list of shared experiences and their date in the system in descending order
    [HttpGet("Table3")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<object>>> Table3()
    {
        var results = await _sharedExperiencesService.Table3();
        return Ok(results);
    }

    // Table 4 - Get guests registered for a shared experience
    [HttpGet("Table4")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<object>>> Table4(string sharedExperienceId)
    {
        var results = await _sharedExperiencesService.Table4(sharedExperienceId);
        return Ok(results);
    }   

    // Table 5 - Get experiences included in a shared experience
    [HttpGet("Table5")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<object>>> Table5(string sharedExperienceId)
    {
        var results = await _sharedExperiencesService.Table5(sharedExperienceId);
        return Ok(results);
    }

    // Table 6 - Get the guests registered for one of the experiences/services in a shared experience.
    [HttpGet("Table6")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<object>>> Table6(string serviceId)
    {
        var results = await _sharedExperiencesService.Table6(serviceId);
        return Ok(results);
    }

    // Table 7 - Get the minimum, average, and maximum price for the whole experience in the system.
    [HttpGet("Table7")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<object>>> Table7()
    {
        var results = await _sharedExperiencesService.Table7();
        return Ok(results);
    }

    // Table 8 - Get the number of guests and sum of sales for each experience available in the system.
    [HttpGet("Table8")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<object>>> Table8()
    {
        var results = await _sharedExperiencesService.Table8();
        return Ok(results);
    }

    // Table 9 - Get the total amount billed for each guest in the system.
    [HttpGet("Table9")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<object>>> Table9()
    {
        var results = await _sharedExperiencesService.Table9();
        return Ok(results);
    }
}