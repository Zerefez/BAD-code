namespace ExperienceService.Models
{
    public class Guest
    {
        public int GuestId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public int Age { get; set; }
        
        // Identity relationship - using ApplicationUserId instead of UserId to avoid conflicts
        public string? ApplicationUserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        
        // Navigation properties
        public List<SharedExperience> SharedExperiences { get; set; } = new();

        public List<Service> Services { get; set; } = new();

        public List<Billing> Billings { get; set; } = new();
    }
}