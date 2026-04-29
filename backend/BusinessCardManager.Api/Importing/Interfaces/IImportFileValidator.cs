namespace BusinessCardManager.Api.Importing.Interfaces;

public interface IImportFileValidator
{
    void Validate(IFormFile? file, params string[] allowedExtensions);
}
