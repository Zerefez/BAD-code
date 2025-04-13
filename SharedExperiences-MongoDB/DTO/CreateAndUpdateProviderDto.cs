using System.ComponentModel.DataAnnotations;

namespace SharedExperiences.DTO;

public class CreateAndUpdateProviderDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Address { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Number { get; set; } = null!;

    [StringLength(100)]
    public string? TouristicOperatorPermit { get; set; }
} 