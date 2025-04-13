using ExperienceService.Services;
using Microsoft.AspNetCore.Mvc;
using SharedExperiences.DTO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using ExperienceService.Models;

namespace ExperienceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                
                return BadRequest(new { Errors = errors });
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (result == null)
            {
                return BadRequest("Username or email already exists.");
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(result);
        }
        
        [HttpPut("roles")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _authService.UpdateUserRoleAsync(updateRoleDto);
            
            if (!result)
            {
                return BadRequest("Failed to update user role. User might not exist or role is invalid.");
            }
            
            return Ok(new { Message = $"User {updateRoleDto.Username} role updated to {updateRoleDto.NewRole}" });
        }
    }
} 