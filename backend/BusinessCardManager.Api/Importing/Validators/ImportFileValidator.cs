using BusinessCardManager.Api.Importing.Exceptions;
using BusinessCardManager.Api.Importing.Interfaces;

namespace BusinessCardManager.Api.Importing.Validators;

public class ImportFileValidator : IImportFileValidator
{
    private const long MaxImportFileBytes = 5 * 1024 * 1024;

    public void Validate(IFormFile? file, params string[] allowedExtensions)
    {
        if (file is null || file.Length == 0)
        {
            throw new BusinessCardImportException("Please upload a non-empty file.");
        }

        if (file.Length > MaxImportFileBytes)
        {
            throw new BusinessCardImportException("Import files must not exceed 5MB.");
        }

        var extension = Path.GetExtension(file.FileName);

        if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessCardImportException(
                $"Unsupported file type. Allowed file types: {string.Join(", ", allowedExtensions)}.");
        }
    }
}
