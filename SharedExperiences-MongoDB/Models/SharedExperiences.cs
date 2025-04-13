namespace ExperienceService.Models
{
    public class SharedExperience
    {
        public int SharedExperienceId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        
        // Navigation properties
        public List<Service> Services { get; set; } = new();
        public List<Guest> Guests { get; set; } = new();
    }
}