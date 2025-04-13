using ExperienceService.Data;
using ExperienceService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SharedExperiences.DTO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceService.Services
{
    public class AuthService
    {
        private readonly MongoDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(MongoDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Find(u => u.Username == loginDto.Username)
                .FirstOrDefaultAsync();

            if (user == null || !VerifyPasswordHash(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            return new AuthResponseDto
            {
                Username = user.Username,
                Email = user.Email,
                Roles = user.Roles,
                Token = GenerateJwtToken(user),
                Expiration = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"]))
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username exists
            if (await _context.Users.Find(u => u.Username == registerDto.Username).AnyAsync())
            {
                return null;
            }

            // Check if email exists
            if (await _context.Users.Find(u => u.Email == registerDto.Email).AnyAsync())
            {
                return null;
            }

            // Check if role is valid
            if (!UserRoles.AllRoles.Contains(registerDto.Role))
            {
                registerDto.Role = UserRoles.Guest; // Default to Guest if invalid role
            }

            // Note: Password validation is handled by data annotations in RegisterDto
            
            var user = new ApplicationUser
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                Roles = new List<string> { registerDto.Role }
            };

            await _context.Users.InsertOneAsync(user);

            return new AuthResponseDto
            {
                Username = user.Username,
                Email = user.Email,
                Roles = user.Roles,
                Token = GenerateJwtToken(user),
                Expiration = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"]))
            };
        }

        private string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Create the PBKDF2 hash
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                byte[] hashBytes = new byte[48]; // 16 bytes for salt, 32 bytes for hash
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 32);
                
                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            
            // Extract the salt (first 16 bytes)
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            
            // Create the hash with the same salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                
                // Compare the computed hash with the stored hash
                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        return false;
                    }
                }
                
                return true;
            }
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add role claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> UpdateUserRoleAsync(UpdateRoleDto updateRoleDto)
        {
            // Validate that the role exists
            if (!UserRoles.AllRoles.Contains(updateRoleDto.NewRole))
            {
                return false;
            }
            
            // Find the user
            var user = await _context.Users
                .Find(u => u.Username == updateRoleDto.Username)
                .FirstOrDefaultAsync();
                
            if (user == null)
            {
                return false;
            }
            
            // Update the user's role
            user.Roles = new List<string> { updateRoleDto.NewRole };
            
            // Update in database
            var updateResult = await _context.Users.ReplaceOneAsync(
                u => u.Username == updateRoleDto.Username,
                user);
                
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
} 