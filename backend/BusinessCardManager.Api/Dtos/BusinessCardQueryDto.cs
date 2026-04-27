namespace BusinessCardManager.Api.Dtos;

public class BusinessCardQueryDto
{
    public string? Name { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
