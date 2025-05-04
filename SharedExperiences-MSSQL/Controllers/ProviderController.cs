using ExperienceService.Models;
using ExperienceService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExperienceService.DTO;
using SharedExperiences.DTO;

namespace ExperienceService.Controllers
{
    /// <summary>
    /// Controller for managing providers and their operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication by default
    public class ProvidersController : ControllerBase
    {
        private readonly ProviderService _providerService;

        public ProvidersController(ProviderService providerService)
        {
            _providerService = providerService;
        }

        /// <summary>
        /// Get all providers (Admin/Manager/Provider)
        /// </summary>
        /// <returns>List of all providers</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Provider")]
        public async Task<ActionResult<IEnumerable<Provider>>> GetProviders()
        {
            var providers = await _providerService.GetAllProvidersAsync();
            return Ok(providers);
        }

        /// <summary>
        /// Get provider by ID (Admin/Manager/Provider)
        /// </summary>
        /// <param name="id">Provider ID</param>
        /// <returns>Provider details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Provider")]
        public async Task<ActionResult<Provider>> GetProvider(int id)
        {
            var provider = await _providerService.GetProviderByIdAsync(id);

            if (provider == null)
            {
                return NotFound();
            }

            return provider;
        }

        /// <summary>
        /// Get services offered by a provider (Admin/Manager/Provider)
        /// </summary>
        /// <param name="id">Provider ID</param>
        /// <returns>List of services offered by the provider</returns>
        [HttpGet("{id}/Services")]
        [Authorize(Roles = "Admin,Manager,Provider")]
        public async Task<ActionResult<IEnumerable<Service>>> GetProviderServices(int id)
        {
            var services = await _providerService.GetProviderServicesAsync(id);
            return Ok(services);
        }

        /// <summary>
        /// Create a new provider (Admin/Manager only)
        /// </summary>
        /// <param name="providerDto">Provider data</param>
        /// <returns>Created provider details</returns>
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

            var createdProvider = await _providerService.CreateProviderAsync(provider);
            return CreatedAtAction(nameof(GetProvider), new { id = createdProvider.ProviderId }, createdProvider);
        }

        /// <summary>
        /// Update an existing provider (Admin/Manager only)
        /// </summary>
        /// <param name="id">Provider ID</param>
        /// <param name="providerDto">Updated provider data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateProvider(int id, [FromBody] CreateAndUpdateProviderDto providerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var provider = new Provider
            {
                ProviderId = id,
                Name = providerDto.Name,
                Address = providerDto.Address,
                Number = providerDto.Number,
                TouristicOperatorPermit = providerDto.TouristicOperatorPermit
            };

            var updatedProvider = await _providerService.UpdateProviderAsync(id, provider);
            if (updatedProvider == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a provider (Admin/Manager only)
        /// </summary>
        /// <param name="id">Provider ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
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