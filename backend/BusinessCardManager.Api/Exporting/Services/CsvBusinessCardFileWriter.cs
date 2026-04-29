using System.Globalization;
using System.Text;
using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Exporting.Interfaces;
using CsvHelper;

namespace BusinessCardManager.Api.Exporting.Services;

public class CsvBusinessCardFileWriter : IBusinessCardFileWriter
{
    public string ContentType => "text/csv";

    public string FileExtension => "csv";

    public byte[] Write(IReadOnlyList<BusinessCardResponseDto> cards)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(cards.Select(card => new BusinessCardExportRecord
        {
            Id = card.Id,
            Name = card.Name,
            Gender = card.Gender,
            DateOfBirth = card.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Email = card.Email,
            Phone = card.Phone,
            PhotoBase64 = card.PhotoBase64,
            Address = card.Address,
            CreatedAtUtc = card.CreatedAtUtc.ToString("O", CultureInfo.InvariantCulture)
        }));

        writer.Flush();
        return stream.ToArray();
    }

    private sealed class BusinessCardExportRecord
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string DateOfBirth { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? PhotoBase64 { get; set; }

        public string Address { get; set; } = string.Empty;

        public string CreatedAtUtc { get; set; } = string.Empty;
    }
}
