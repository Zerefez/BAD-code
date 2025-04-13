using ExperienceService.Models;
using ExperienceService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedExperiences.DTO;

namespace ExperienceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
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
        public async Task<ActionResult<Provider>> GetProvider(string id)
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
        public async Task<ActionResult<IEnumerable<Service>>> GetProviderServices(string id)
        {
            var services = await _providerService.GetProviderServicesAsync(id);
            return Ok(services);
        }

        // POST: api/Providers
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

            var createdProvider = await _providerService.CreateProviderAsync(provider);
            return CreatedAtAction(nameof(GetProvider), new { id = createdProvider.Id }, createdProvider);
        }

        // PUT: api/Providers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProvider(string id, [FromBody] CreateAndUpdateProviderDto providerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the existing provider first
            var existingProvider = await _providerService.GetProviderByIdAsync(id);
            if (existingProvider == null)
            {
                return NotFound();
            }

            // Update properties from DTO
            existingProvider.Name = providerDto.Name;
            existingProvider.Address = providerDto.Address;
            existingProvider.Number = providerDto.Number;
            existingProvider.TouristicOperatorPermit = providerDto.TouristicOperatorPermit;

            var updatedProvider = await _providerService.UpdateProviderAsync(id, existingProvider);
            if (updatedProvider == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Providers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvider(string id)
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