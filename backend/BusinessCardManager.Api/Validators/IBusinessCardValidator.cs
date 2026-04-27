namespace BusinessCardManager.Api.Validators;

public interface IBusinessCardValidator
{
    bool IsValidBase64Photo(string? photoBase64, out string? error);
}
