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
    public async Task<ActionResult<Provider>> GetProvider(int id)
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
        return CreatedAtAction(nameof(GetProvider), new { id = createdProvider.ProviderId }, createdProvider);
    }

    // PUT: api/SharedExperiences/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProvider(int id, [FromBody] Provider provider)
    {
        if (id != provider.ProviderId)
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
    public async Task<IActionResult> DeleteProvider(int id)
    {
        var deleted = await _sharedExperiencesService.DeleteProviderAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}