public class Guest
{
    public int GuestID { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public int Age { get; set; }

    // Relationships
    public ICollection<Billing> Billings { get; set; } = new List<Billing>();
    public ICollection<SharedExperience> SharedExperiences { get; set; } = new List<SharedExperience>();
}
