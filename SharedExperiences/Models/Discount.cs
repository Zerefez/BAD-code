namespace ExperienceService.Models
{
    public class Discount
    {
        public int DiscountId { get; set; }
        public decimal DiscountValue { get; set; }
        public int GuestCount { get; set; }
        
        // Navigation properties
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}