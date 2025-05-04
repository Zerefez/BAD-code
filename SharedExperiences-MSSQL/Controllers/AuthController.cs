using System.Threading.Tasks;
using ExperienceService.DTO;
using ExperienceService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExperienceService.Controllers
{
    /// <summary>
    /// Controller for authentication and user management
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user (Public)
        /// </summary>
        /// <remarks>
        /// Registers a new user with Guest role by default.
        /// </remarks>
        /// <param name="model">Registration details</param>
        /// <returns>Result of registration attempt</returns>
        [HttpPost("register/guest")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Register a new admin user
        /// </summary>
        /// <remarks>
        /// Registers a new user with Admin role.
        /// </remarks>
        /// <param name="model">Registration details</param>
        /// <returns>Result of registration attempt</returns>
        [HttpPost("register/admin")]
        [Authorize(Roles = "Admin")] // Only existing admins can create new admins
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model, "Admin");
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Register a new manager user
        /// </summary>
        /// <remarks>
        /// Registers a new user with Manager role.
        /// </remarks>
        /// <param name="model">Registration details</param>
        /// <returns>Result of registration attempt</returns>
        [HttpPost("register/manager")]
        [Authorize(Roles = "Admin")] // Only admins can create managers
        public async Task<IActionResult> RegisterManager([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model, "Manager");
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Register a new provider user
        /// </summary>
        /// <remarks>
        /// Registers a new user with Provider role.
        /// </remarks>
        /// <param name="model">Registration details</param>
        /// <returns>Result of registration attempt</returns>
        [HttpPost("register/provider")]
        [Authorize(Roles = "Admin,Manager")] // Only admins and managers can create providers
        public async Task<IActionResult> RegisterProvider([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model, "Provider");
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Login to the application (Public)
        /// </summary>
        /// <remarks>
        /// Returns a JWT token for authentication if credentials are valid.
        /// </remarks>
        /// <param name="model">Login credentials</param>
        /// <returns>JWT token and user information</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(model);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
} 