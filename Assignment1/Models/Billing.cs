public class Billing
{
    public int BillingID { get; set; }
    public decimal Amount { get; set; }

    // Relationships
    public int GuestID { get; set; }
    public Guest Guest { get; set; }

    public int ProviderID { get; set; }
    public Provider Provider { get; set; }
}
