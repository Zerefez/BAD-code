using ExperienceService.Models;
using ExperienceService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedExperiences.DTO;

namespace ExperienceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly ServiceService _serviceService;

        public ServicesController(ServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        // GET: api/Services
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Service>> GetService(string id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            return service;
        }

        // POST: api/Services
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
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
                ProviderId = serviceDto.ProviderId.ToString()
                // Map other properties as needed
            };

            var createdService = await _serviceService.CreateServiceAsync(service);
            return CreatedAtAction(nameof(GetService), new { id = createdService.Id }, createdService);
        }

        // PUT: api/Services/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateService(string id, [FromBody] CreateAndUpdateServiceDto serviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the existing service first
            var existingService = await _serviceService.GetServiceByIdAsync(id);
            if (existingService == null)
            {
                return NotFound();
            }

            // Update properties from DTO
            existingService.Name = serviceDto.Name;
            existingService.Description = serviceDto.Description;
            existingService.Price = (int)serviceDto.Price;
            existingService.ProviderId = serviceDto.ProviderId.ToString();
            // Map other properties as needed

            var updatedService = await _serviceService.UpdateServiceAsync(id, existingService);
            if (updatedService == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteService(string id)
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