using ExperienceService.Models;
using ExperienceService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExperienceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly ProviderService _providerService;

        public ProvidersController(ProviderService providerService)
        {
            _providerService = providerService;
        }

        // GET: api/Providers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Provider>>> GetProviders()
        {
            var providers = await _providerService.GetAllProvidersAsync();
            return Ok(providers);
        }

        // GET: api/Providers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Provider>> GetProvider(int id)
        {
            var provider = await _providerService.GetProviderByIdAsync(id);

            if (provider == null)
            {
                return NotFound();
            }

            return provider;
        }

        // GET: api/Providers/5/Services
        [HttpGet("{id}/Services")]
        public async Task<ActionResult<IEnumerable<Service>>> GetProviderServices(int id)
        {
            var services = await _providerService.GetProviderServicesAsync(id);
            return Ok(services);
        }

        // POST: api/Providers
        [HttpPost]
        public async Task<ActionResult<Provider>> CreateProvider([FromBody] Provider provider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProvider = await _providerService.CreateProviderAsync(provider);
            return CreatedAtAction(nameof(GetProvider), new { id = createdProvider.ProviderId }, createdProvider);
        }

        // PUT: api/Providers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProvider(int id, [FromBody] Provider provider)
        {
            if (id != provider.ProviderId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedProvider = await _providerService.UpdateProviderAsync(id, provider);
            if (updatedProvider == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Providers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvider(int id)
        {
            var result = await _providerService.DeleteProviderAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        
    }
}