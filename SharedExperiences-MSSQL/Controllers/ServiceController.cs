using ExperienceService.Models;
using ExperienceService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedExperiences.DTO;

namespace ExperienceService.Controllers
{
    /// <summary>
    /// Controller for managing services
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication by default
    public class ServicesController : ControllerBase
    {
        private readonly ServiceService _serviceService;

        public ServicesController(ServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        /// <summary>
        /// Get all services (Admin/Manager/Provider)
        /// </summary>
        /// <returns>List of all services</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Provider")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }

        /// <summary>
        /// Get service by ID (Admin/Manager/Provider)
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <returns>Service details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Provider")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            return service;
        }

        /// <summary>
        /// Create a new service (Admin/Manager/Provider)
        /// </summary>
        /// <param name="serviceDto">Service data</param>
        /// <returns>Created service details</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Provider")]
        public async Task<ActionResult<Service>> CreateService([FromBody] CreateAndUpdateServiceDto serviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var service = new Service
            {
                Name = serviceDto.Name,
                Description = serviceDto.Description,
                Price = (int)serviceDto.Price,
                ProviderId = serviceDto.ProviderId,
            };

            var createdService = await _serviceService.CreateServiceAsync(service);
            return CreatedAtAction(nameof(GetService), new { id = createdService.ServiceId }, createdService);
        }

        /// <summary>
        /// Update an existing service (Admin/Manager/Provider)
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <param name="serviceDto">Updated service data</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Provider")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] CreateAndUpdateServiceDto serviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var service = new Service
            {
                ServiceId = id,
                Name = serviceDto.Name,
                Description = serviceDto.Description,
                Price = (int)serviceDto.Price,
                ProviderId = serviceDto.ProviderId,
            };

            var updatedService = await _serviceService.UpdateServiceAsync(id, service);
            if (updatedService == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a service (Admin/Manager only)
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _serviceService.DeleteServiceAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}