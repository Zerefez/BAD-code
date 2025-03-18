namespace ExperienceService.Models
{
    public class Provider
    {
        public int ProviderId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Number { get; set; }
        public string CVR { get; set; }
        
        
        // Navigation properties
        public List<Service> Services { get; set; } = new();
        public List<Billing> Billings { get; set; } = new();
    }
}