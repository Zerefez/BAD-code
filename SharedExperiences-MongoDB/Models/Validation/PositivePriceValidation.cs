using System.ComponentModel.DataAnnotations;

namespace ExperienceService.Models.Validators
{
    public class PositivePriceAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is decimal price)
            {
                if (price < 0)
                {
                    return new ValidationResult("Price must be a positive value.");
                }
            }
            else if (value is int intPrice)
            {
                if (intPrice < 0)
                {
                    return new ValidationResult("Price must be a positive value.");
                }
            }

            return ValidationResult.Success;
        }
    }
}

