namespace SharedExperiences.DTO;

public class SharedExperienceDateDto
{
    public DateTime Date { get; set; }
    public int SharedExperienceId { get; set; }
    public int GuestId { get; set; }
    public int ServiceId { get; set; }
    public int ProviderId { get; set; }
    public int? DiscountId { get; set; }
}