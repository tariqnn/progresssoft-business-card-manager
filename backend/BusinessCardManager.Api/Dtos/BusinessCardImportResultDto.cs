namespace BusinessCardManager.Api.Dtos;

public class BusinessCardImportResultDto
{
    public int ImportedCount { get; set; }

    public IReadOnlyList<BusinessCardResponseDto> Cards { get; set; } = [];
}
