using System.ComponentModel.DataAnnotations;
using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Importing.Interfaces;
using BusinessCardManager.Api.Validators;

namespace BusinessCardManager.Api.Importing.Validators;

public class ImportedBusinessCardValidator(
    IBusinessCardValidator businessCardValidator) : IImportedBusinessCardValidator
{
    public IReadOnlyList<string> Validate(IReadOnlyList<BusinessCardCreateDto> cards)
    {
        var errors = new List<string>();

        for (var index = 0; index < cards.Count; index++)
        {
            var card = cards[index];
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(card);

            if (!Validator.TryValidateObject(card, validationContext, validationResults, validateAllProperties: true))
            {
                errors.AddRange(validationResults.Select(result =>
                    $"Card #{index + 1}: {result.ErrorMessage}"));
            }

            if (!businessCardValidator.IsValidBase64Photo(card.PhotoBase64, out var photoError))
            {
                errors.Add($"Card #{index + 1}: {photoError}");
            }
        }

        return errors;
    }
}
