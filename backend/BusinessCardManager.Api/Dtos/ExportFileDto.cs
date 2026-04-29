namespace BusinessCardManager.Api.Dtos;

public class ExportFileDto
{
    public required byte[] Contents { get; init; }

    public required string ContentType { get; init; }

    public required string FileName { get; init; }
}
