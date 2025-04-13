using System.ComponentModel.DataAnnotations;

namespace SharedExperiences.DTO;

public class CreateAndUpdateSharedExperienceDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    public DateTime Date { get; set; }

    public List<string> ServiceIds { get; set; } = new();

    public List<string> GuestIds { get; set; } = new();
} 