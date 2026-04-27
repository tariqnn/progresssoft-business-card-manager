namespace BusinessCardManager.Api.Models;

public class BusinessCard
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string? PhotoBase64 { get; set; }

    public string Address { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
