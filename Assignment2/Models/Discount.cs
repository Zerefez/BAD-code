public class Discount
{
    public int DiscountID { get; set; }
    public int GuestCount { get; set; }
    public decimal DiscountAmount { get; set; }

    // Relationships
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
