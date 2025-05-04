using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ExperienceService.Data;
using ExperienceService.DTO;
using ExperienceService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ExperienceService.Services
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SharedExperiencesDbContext _context;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            SharedExperiencesDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO model, string role = "Guest")
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "User already exists!"
                };

            ApplicationUser user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "User creation failed! Please check user details and try again."
                };

            // Ensure the role exists
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            // Add user to role
            await _userManager.AddToRoleAsync(user, role);

            // Reload the user with the updated ID
            user = await _userManager.FindByEmailAsync(user.Email);
            
            // Create corresponding entry in Provider or Guest table based on role
            if (role == "Provider")
            {
                var provider = new Provider
                {
                    ApplicationUserId = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    Number = user.PhoneNumber ?? "N/A",
                    Address = "Please update your address",
                    TouristicOperatorPermit = "Pending"
                };
                _context.Providers.Add(provider);
                await _context.SaveChangesAsync();
            }
            else if (role == "Guest")
            {
                var guest = new Guest
                {
                    ApplicationUserId = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    Number = user.PhoneNumber ?? "N/A",
                    Age = 0 // Default age, can be updated later
                };
                _context.Guests.Add(guest);
                await _context.SaveChangesAsync();
            }

            return new AuthResponseDTO
            {
                Success = true,
                Message = "User created successfully!"
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid email or password."
                };

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid email or password."
                };

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtToken(user, userRoles);

            return new AuthResponseDTO
            {
                Success = true,
                Token = token,
                UserName = user.UserName,
                Roles = userRoles.ToList(),
                Message = "Login successful"
            };
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 