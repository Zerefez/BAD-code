
using ExperienceService.Models.Validators;


namespace SharedExperiences.DTO;

public class CreateAndUpdateServiceDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int ProviderId { get; set; }
    public int? DiscountId { get; set; }
}