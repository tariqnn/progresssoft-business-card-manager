using BusinessCardManager.Api.Data;
using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Mapping;
using BusinessCardManager.Api.Validators;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardManager.Api.Services;

public class BusinessCardService(
    AppDbContext dbContext,
    IBusinessCardValidator businessCardValidator) : IBusinessCardService
{
    public async Task<IReadOnlyList<BusinessCardResponseDto>> GetBusinessCardsAsync(
        BusinessCardQueryDto query,
        CancellationToken cancellationToken)
    {
        var cards = dbContext.BusinessCards.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            cards = cards.Where(card => card.Name.Contains(query.Name));
        }

        if (!string.IsNullOrWhiteSpace(query.Gender))
        {
            cards = cards.Where(card => card.Gender == query.Gender);
        }

        if (query.DateOfBirth.HasValue)
        {
            cards = cards.Where(card => card.DateOfBirth == query.DateOfBirth.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            cards = cards.Where(card => card.Email.Contains(query.Email));
        }

        if (!string.IsNullOrWhiteSpace(query.Phone))
        {
            cards = cards.Where(card => card.Phone.Contains(query.Phone));
        }

        return await cards
            .OrderBy(card => card.Name)
            .Select(card => card.ToResponse())
            .ToListAsync(cancellationToken);
    }

    public async Task<BusinessCardResponseDto?> GetBusinessCardAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var card = await dbContext.BusinessCards
            .AsNoTracking()
            .FirstOrDefaultAsync(card => card.Id == id, cancellationToken);

        return card?.ToResponse();
    }

    public async Task<BusinessCardResponseDto> CreateBusinessCardAsync(
        BusinessCardCreateDto dto,
        CancellationToken cancellationToken)
    {
        if (!businessCardValidator.IsValidBase64Photo(dto.PhotoBase64, out var photoError))
        {
            throw new BusinessCardValidationException(photoError ?? "Business card photo is invalid.");
        }

        var card = dto.ToEntity();

        dbContext.BusinessCards.Add(card);
        await dbContext.SaveChangesAsync(cancellationToken);

        return card.ToResponse();
    }

    public async Task<bool> DeleteBusinessCardAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var card = await dbContext.BusinessCards.FindAsync([id], cancellationToken);

        if (card is null)
        {
            return false;
        }

        dbContext.BusinessCards.Remove(card);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
