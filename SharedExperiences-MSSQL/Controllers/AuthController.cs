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
        [HttpPost("register")]
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