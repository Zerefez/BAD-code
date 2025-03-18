public class Service
{
    public int ServiceID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<decimal> Prices { get; set; } = new List<decimal>();

    // Relationships
    public ICollection<Provider> Providers { get; set; } = new List<Provider>();
    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
