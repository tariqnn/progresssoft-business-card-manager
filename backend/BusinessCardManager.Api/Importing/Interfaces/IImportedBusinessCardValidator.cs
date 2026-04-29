using BusinessCardManager.Api.Dtos;

namespace BusinessCardManager.Api.Importing.Interfaces;

public interface IImportedBusinessCardValidator
{
    IReadOnlyList<string> Validate(IReadOnlyList<BusinessCardCreateDto> cards);
}
