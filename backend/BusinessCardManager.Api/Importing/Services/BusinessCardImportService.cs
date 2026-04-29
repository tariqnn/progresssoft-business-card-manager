using BusinessCardManager.Api.Data;
using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Importing.Exceptions;
using BusinessCardManager.Api.Importing.Interfaces;
using BusinessCardManager.Api.Mapping;

namespace BusinessCardManager.Api.Importing.Services;

public class BusinessCardImportService(
    AppDbContext dbContext,
    IBusinessCardPayloadParser payloadParser,
    IQrCodeReader qrCodeReader,
    IImportFileValidator importFileValidator,
    IImportedBusinessCardValidator importedBusinessCardValidator) : IBusinessCardImportService
{
    public async Task<BusinessCardImportResultDto> ImportCsvAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        importFileValidator.Validate(file, ".csv");

        await using var stream = file.OpenReadStream();
        var cards = payloadParser.ParseCsv(stream);

        return await SaveCardsAsync(cards, cancellationToken);
    }

    public async Task<BusinessCardImportResultDto> ImportXmlAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        importFileValidator.Validate(file, ".xml");

        await using var stream = file.OpenReadStream();
        var cards = await payloadParser.ParseXmlAsync(stream, cancellationToken);

        return await SaveCardsAsync(cards, cancellationToken);
    }

    public async Task<BusinessCardImportResultDto> ImportQrCodeAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        importFileValidator.Validate(file, ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".webp");

        await using var stream = file.OpenReadStream();
        var payload = await qrCodeReader.ReadPayloadAsync(stream, cancellationToken);
        var card = payloadParser.ParseQrPayload(payload);

        card.PhotoBase64 = null;

        return await SaveCardsAsync([card], cancellationToken);
    }

    private async Task<BusinessCardImportResultDto> SaveCardsAsync(
        IReadOnlyList<BusinessCardCreateDto> cardDtos,
        CancellationToken cancellationToken)
    {
        if (cardDtos.Count == 0)
        {
            throw new BusinessCardImportException("Import file does not contain any business cards.");
        }

        var validationErrors = importedBusinessCardValidator.Validate(cardDtos);

        if (validationErrors.Count > 0)
        {
            throw new BusinessCardImportException("Imported business card data is invalid.", validationErrors);
        }

        var cards = cardDtos.Select(dto => dto.ToEntity()).ToList();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        dbContext.BusinessCards.AddRange(cards);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new BusinessCardImportResultDto
        {
            ImportedCount = cards.Count,
            Cards = cards.Select(card => card.ToResponse()).ToList()
        };
    }

}
