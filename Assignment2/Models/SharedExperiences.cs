public class SharedExperience
{
    public int ExperiencesID { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }

    // Relationships
    public ICollection<Guest> Guests { get; set; } = new List<Guest>();
}
