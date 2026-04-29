using System.Text;
using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Exporting.Services;

namespace BusinessCardManager.Tests;

public class BusinessCardFileWriterTests
{
    [Fact]
    public void CsvWriter_IncludesBusinessCardFields()
    {
        var writer = new CsvBusinessCardFileWriter();

        var contents = Encoding.UTF8.GetString(writer.Write([CreateCard()]));

        Assert.Contains("Export User", contents);
        Assert.Contains("export@example.com", contents);
        Assert.Contains("1998-01-15", contents);
    }

    [Fact]
    public void XmlWriter_IncludesBusinessCardFields()
    {
        var writer = new XmlBusinessCardFileWriter();

        var contents = Encoding.UTF8.GetString(writer.Write([CreateCard()]));

        Assert.Contains("<BusinessCards>", contents);
        Assert.Contains("<Name>Export User</Name>", contents);
        Assert.Contains("<Email>export@example.com</Email>", contents);
    }

    private static BusinessCardResponseDto CreateCard()
    {
        return new BusinessCardResponseDto
        {
            Id = 1,
            Name = "Export User",
            Gender = "Male",
            DateOfBirth = new DateOnly(1998, 1, 15),
            Email = "export@example.com",
            Phone = "+962790000000",
            PhotoBase64 = null,
            Address = "Amman",
            CreatedAtUtc = new DateTime(2026, 4, 28, 12, 0, 0, DateTimeKind.Utc)
        };
    }
}
