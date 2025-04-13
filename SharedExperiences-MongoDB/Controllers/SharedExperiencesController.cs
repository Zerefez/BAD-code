using ExperienceService.Models;
using ExperienceService.Services;


using Microsoft.AspNetCore.Mvc;

namespace ExperienceService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SharedExperiencesController :  ControllerBase
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
    public async Task<ActionResult<Provider>> CreateProvider([FromBody] Provider provider)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdProvider = await _sharedExperiencesService.CreateProviderAsync(provider);
        return CreatedAtAction(nameof(GetProvider), new { id = createdProvider.Id }, createdProvider);
    }

    // PUT: api/SharedExperiences/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProvider(string id, [FromBody] Provider provider)
    {
        if (id != provider.Id)
        {
            return BadRequest();
        }

        var updatedProvider = await _sharedExperiencesService.UpdateProviderAsync(id, provider);
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
        var providers = await _sharedExperiencesService.Table1();
        return Ok(providers);
    }

    [HttpGet("Table2")]
    public async Task<ActionResult<IEnumerable<object>>> Table2()
    {
        var providers = await _sharedExperiencesService.Table2();
        return Ok(providers);
    }
    
    [HttpGet("Table3")]
    public async Task<ActionResult<IEnumerable<object>>> Table3()
    {
        var providers = await _sharedExperiencesService.Table3();
        return Ok(providers);
    }

    [HttpGet("Table4")]
    public async Task<ActionResult<IEnumerable<object>>> Table4(string sharedExperienceId)
    {
        var providers = await _sharedExperiencesService.Table4(sharedExperienceId);
        return Ok(providers);
    }   

    [HttpGet("Table5")]
    public async Task<ActionResult<IEnumerable<object>>> Table5(string sharedExperienceId)
    {
        var providers = await _sharedExperiencesService.Table5(sharedExperienceId);
        return Ok(providers);
    }

    [HttpGet("Table6")]
    public async Task<ActionResult<IEnumerable<object>>> Table6(string serviceId)
    {
        var providers = await _sharedExperiencesService.Table6(serviceId);
        return Ok(providers);
    }

    [HttpGet("Table7")]
    public async Task<ActionResult<IEnumerable<object>>> Table7()
    {
        var providers = await _sharedExperiencesService.Table7();
        return Ok(providers);
    }

    [HttpGet("Table8")]
    public async Task<ActionResult<IEnumerable<object>>> Table8()
    {
        var providers = await _sharedExperiencesService.Table8();
        return Ok(providers);
    }

    [HttpGet("Table9")]
    public async Task<ActionResult<IEnumerable<object>>> Table9()
    {
        var providers = await _sharedExperiencesService.Table9();
        return Ok(providers);
    }

}