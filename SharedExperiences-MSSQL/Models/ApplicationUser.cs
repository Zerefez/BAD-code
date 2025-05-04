using Microsoft.AspNetCore.Identity;

namespace ExperienceService.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        // Navigation property - using nullable reference types
        public virtual Provider? Provider { get; set; }
        public virtual Guest? Guest { get; set; }
    }
} 