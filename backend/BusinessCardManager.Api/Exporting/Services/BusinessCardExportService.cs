using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Exporting.Interfaces;
using BusinessCardManager.Api.Services;

namespace BusinessCardManager.Api.Exporting.Services;

public class BusinessCardExportService(
    IBusinessCardService businessCardService,
    CsvBusinessCardFileWriter csvWriter,
    XmlBusinessCardFileWriter xmlWriter) : IBusinessCardExportService
{
    public async Task<ExportFileDto> ExportCsvAsync(
        BusinessCardQueryDto query,
        CancellationToken cancellationToken)
    {
        return await ExportAsync(query, csvWriter, cancellationToken);
    }

    public async Task<ExportFileDto> ExportXmlAsync(
        BusinessCardQueryDto query,
        CancellationToken cancellationToken)
    {
        return await ExportAsync(query, xmlWriter, cancellationToken);
    }

    public async Task<ExportFileDto?> ExportCardCsvAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await ExportCardAsync(id, csvWriter, cancellationToken);
    }

    public async Task<ExportFileDto?> ExportCardXmlAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await ExportCardAsync(id, xmlWriter, cancellationToken);
    }

    private async Task<ExportFileDto> ExportAsync(
        BusinessCardQueryDto query,
        IBusinessCardFileWriter writer,
        CancellationToken cancellationToken)
    {
        var cards = await businessCardService.GetBusinessCardsAsync(query, cancellationToken);

        return new ExportFileDto
        {
            Contents = writer.Write(cards),
            ContentType = writer.ContentType,
            FileName = $"business-cards-{DateTime.UtcNow:yyyyMMddHHmmss}.{writer.FileExtension}"
        };
    }

    private async Task<ExportFileDto?> ExportCardAsync(
        int id,
        IBusinessCardFileWriter writer,
        CancellationToken cancellationToken)
    {
        var card = await businessCardService.GetBusinessCardAsync(id, cancellationToken);

        if (card is null)
        {
            return null;
        }

        return new ExportFileDto
        {
            Contents = writer.Write([card]),
            ContentType = writer.ContentType,
            FileName = $"business-card-{card.Id}-{SanitizeFileName(card.Name)}.{writer.FileExtension}"
        };
    }

    private static string SanitizeFileName(string value)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var sanitized = new string(value
            .Select(character => invalidCharacters.Contains(character) ? '-' : character)
            .ToArray());

        return string.IsNullOrWhiteSpace(sanitized) ? "card" : sanitized.Trim();
    }
}
