using System.ComponentModel.DataAnnotations;
using SharedExperiences.Models.Validation;

namespace SharedExperiences.DTO
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [PasswordValidation(minLength: 8, requireUppercase: true, requireLowercase: true, requireDigit: true, requireSpecialChar: true)]
        public string Password { get; set; }
        
        public string Role { get; set; } = "Guest"; // Default role
    }
    
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
    
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Expiration { get; set; }
    }
    
    public class UpdateRoleDto
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string NewRole { get; set; }
    }
} 