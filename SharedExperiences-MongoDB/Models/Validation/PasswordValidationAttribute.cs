using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SharedExperiences.Models.Validation
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly bool _requireUppercase;
        private readonly bool _requireLowercase;
        private readonly bool _requireDigit;
        private readonly bool _requireSpecialChar;

        public PasswordValidationAttribute(
            int minLength = 8, 
            bool requireUppercase = true, 
            bool requireLowercase = true, 
            bool requireDigit = true, 
            bool requireSpecialChar = true)
        {
            _minLength = minLength;
            _requireUppercase = requireUppercase;
            _requireLowercase = requireLowercase;
            _requireDigit = requireDigit;
            _requireSpecialChar = requireSpecialChar;
            
            // Set default error message
            ErrorMessage = "Password must be at least {0} characters long and include {1}.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;
            
            if (string.IsNullOrWhiteSpace(password))
            {
                return new ValidationResult("Password cannot be empty.");
            }

            var errors = new List<string>();

            // Check minimum length
            if (password.Length < _minLength)
            {
                errors.Add($"at least {_minLength} characters");
            }

            // Check for uppercase letters
            if (_requireUppercase && !Regex.IsMatch(password, "[A-Z]"))
            {
                errors.Add("at least one uppercase letter");
            }

            // Check for lowercase letters
            if (_requireLowercase && !Regex.IsMatch(password, "[a-z]"))
            {
                errors.Add("at least one lowercase letter");
            }

            // Check for digits
            if (_requireDigit && !Regex.IsMatch(password, "[0-9]"))
            {
                errors.Add("at least one digit");
            }

            // Check for special characters
            if (_requireSpecialChar && !Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                errors.Add("at least one special character");
            }

            if (errors.Count > 0)
            {
                var requirements = string.Join(", ", errors);
                return new ValidationResult(string.Format(ErrorMessage, _minLength, requirements));
            }

            return ValidationResult.Success;
        }
    }
} 