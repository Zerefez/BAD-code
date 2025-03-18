public class Provider
{
    public int ProviderID { get; set; }
    public string Name { get; set; }
    public string CVR { get; set; }
    public string Address { get; set; }
    public string Number { get; set; }

    // Relationships
    public ICollection<Billing> Billings { get; set; } = new List<Billing>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
