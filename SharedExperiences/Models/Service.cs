using ExperienceService.Models.Validators;
using System.ComponentModel.DataAnnotations;

namespace ExperienceService.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        
        [Required]
        [PositivePrice(ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; } // Changed from decimal to int in Migration3

        public DateTime Date { get; set; } // Added in Migration2

        // Navigation properties
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        public List<SharedExperience> SharedExperiences { get; set; } = new();
        public Discount Discount { get; set; }
    }
}