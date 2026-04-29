using BusinessCardManager.Api.Dtos;

namespace BusinessCardManager.Api.Services;

public interface IBusinessCardService
{
    Task<IReadOnlyList<BusinessCardResponseDto>> GetBusinessCardsAsync(
        BusinessCardQueryDto query,
        CancellationToken cancellationToken);

    Task<BusinessCardResponseDto?> GetBusinessCardAsync(
        int id,
        CancellationToken cancellationToken);

    Task<BusinessCardResponseDto> CreateBusinessCardAsync(
        BusinessCardCreateDto dto,
        CancellationToken cancellationToken);

    Task<bool> DeleteBusinessCardAsync(
        int id,
        CancellationToken cancellationToken);
}
