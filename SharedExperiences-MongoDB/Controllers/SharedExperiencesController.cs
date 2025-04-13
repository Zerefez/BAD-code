using ExperienceService.Models;
using ExperienceService.Services;
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
    public async Task<ActionResult<IEnumerable<Provider>>> GetProviders()
    {
        var providers = await _sharedExperiencesService.GetAllProvidersAsync();
        return Ok(providers);
    }

    // GET: api/SharedExperiences/5
    [HttpGet("{id}")]
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
    public async Task<IActionResult> DeleteProvider(string id)
    {
        var deleted = await _sharedExperiencesService.DeleteProviderAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("Table1")]
    public async Task<ActionResult<IEnumerable<object>>> Table1()
    {
        var results = await _sharedExperiencesService.Table1();
        return Ok(results);
    }

    [HttpGet("Table2")]
    public async Task<ActionResult<IEnumerable<object>>> Table2()
    {
        var results = await _sharedExperiencesService.Table2();
        return Ok(results);
    }
    
    [HttpGet("Table3")]
    public async Task<ActionResult<IEnumerable<object>>> Table3()
    {
        var results = await _sharedExperiencesService.Table3();
        return Ok(results);
    }

    [HttpGet("Table4")]
    public async Task<ActionResult<IEnumerable<object>>> Table4(string sharedExperienceId)
    {
        var results = await _sharedExperiencesService.Table4(sharedExperienceId);
        return Ok(results);
    }   

    [HttpGet("Table5")]
    public async Task<ActionResult<IEnumerable<object>>> Table5(string sharedExperienceId)
    {
        var results = await _sharedExperiencesService.Table5(sharedExperienceId);
        return Ok(results);
    }

    [HttpGet("Table6")]
    public async Task<ActionResult<IEnumerable<object>>> Table6(string serviceId)
    {
        var results = await _sharedExperiencesService.Table6(serviceId);
        return Ok(results);
    }

    [HttpGet("Table7")]
    public async Task<ActionResult<IEnumerable<object>>> Table7()
    {
        var results = await _sharedExperiencesService.Table7();
        return Ok(results);
    }

    [HttpGet("Table8")]
    public async Task<ActionResult<IEnumerable<object>>> Table8()
    {
        var results = await _sharedExperiencesService.Table8();
        return Ok(results);
    }

    [HttpGet("Table9")]
    public async Task<ActionResult<IEnumerable<object>>> Table9()
    {
        var results = await _sharedExperiencesService.Table9();
        return Ok(results);
    }
}