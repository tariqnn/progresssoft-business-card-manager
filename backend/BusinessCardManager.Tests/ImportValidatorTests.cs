using System.Text;
using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Importing.Exceptions;
using BusinessCardManager.Api.Importing.Validators;
using BusinessCardManager.Api.Validators;
using Microsoft.AspNetCore.Http;

namespace BusinessCardManager.Tests;

public class ImportValidatorTests
{
    [Fact]
    public void ImportFileValidator_AllowsExpectedExtension()
    {
        var validator = new ImportFileValidator();
        var file = CreateFormFile("cards.csv", "Name,Email");

        var exception = Record.Exception(() => validator.Validate(file, ".csv"));

        Assert.Null(exception);
    }

    [Fact]
    public void ImportFileValidator_RejectsUnexpectedExtension()
    {
        var validator = new ImportFileValidator();
        var file = CreateFormFile("cards.txt", "Name,Email");

        var exception = Assert.Throws<BusinessCardImportException>(() => validator.Validate(file, ".csv"));

        Assert.Contains("Unsupported file type", exception.Message);
    }

    [Fact]
    public void ImportedBusinessCardValidator_ReturnsDtoAndPhotoErrors()
    {
        var validator = new ImportedBusinessCardValidator(new BusinessCardValidator());
        var cards = new[]
        {
            new BusinessCardCreateDto
            {
                Name = "",
                Gender = "Male",
                DateOfBirth = new DateOnly(1998, 1, 15),
                Email = "not-an-email",
                Phone = "+962790000000",
                PhotoBase64 = "not-base64",
                Address = "Amman"
            }
        };

        var errors = validator.Validate(cards);

        Assert.Contains(errors, error => error.Contains("Name", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(errors, error => error.Contains("Email", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(errors, error => error.Contains("Base64", StringComparison.OrdinalIgnoreCase));
    }

    private static IFormFile CreateFormFile(string fileName, string contents)
    {
        var bytes = Encoding.UTF8.GetBytes(contents);
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", fileName);
    }
}
