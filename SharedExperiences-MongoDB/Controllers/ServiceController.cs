using ExperienceService.Models;
using ExperienceService.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
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
        public async Task<ActionResult<Service>> CreateService([FromBody] Service service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdService = await _serviceService.CreateServiceAsync(service);
            return CreatedAtAction(nameof(GetService), new { id = createdService.ServiceId }, createdService);
        }

        // PUT: api/Services/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] Service service)
        {
            if (id != service.ServiceId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedService = await _serviceService.UpdateServiceAsync(id, service);
            if (updatedService == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
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