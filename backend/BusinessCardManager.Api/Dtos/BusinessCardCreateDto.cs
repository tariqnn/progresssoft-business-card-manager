using System.ComponentModel.DataAnnotations;

namespace BusinessCardManager.Api.Dtos;

public class BusinessCardCreateDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    public string? PhotoBase64 { get; set; }

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
}
