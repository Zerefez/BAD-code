namespace ExperienceService.Models
{
    public class Billing
    {
        public int BillingId { get; set; }
        public decimal Amount { get; set; }
        
        // Navigation properties
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        
        public int GuestId { get; set; }
        public Guest Guest { get; set; }
    }
}
