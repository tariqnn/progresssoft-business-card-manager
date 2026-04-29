using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Importing.Exceptions;
using BusinessCardManager.Api.Importing.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;

namespace BusinessCardManager.Api.Importing.Parsers;

public class BusinessCardPayloadParser : IBusinessCardPayloadParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IReadOnlyList<BusinessCardCreateDto> ParseCsv(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HeaderValidated = null,
            TrimOptions = TrimOptions.Trim
        });

        try
        {
            var records = csv.GetRecords<BusinessCardCsvRecord>().ToList();

            if (records.Count == 0)
            {
                throw new BusinessCardImportException("CSV import file does not contain any business cards.");
            }

            return records
                .Select((record, index) => ToDto(record, $"CSV row {index + 2}"))
                .ToList();
        }
        catch (BusinessCardImportException)
        {
            throw;
        }
        catch (Exception ex) when (ex is CsvHelperException or HeaderValidationException or CsvHelper.MissingFieldException)
        {
            throw new BusinessCardImportException("CSV import file is invalid.", [ex.Message]);
        }
    }

    public async Task<IReadOnlyList<BusinessCardCreateDto>> ParseXmlAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        try
        {
            var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
            var root = document.Root;

            if (root is null)
            {
                throw new BusinessCardImportException("XML import file is empty.");
            }

            var cardElements = string.Equals(root.Name.LocalName, "BusinessCard", StringComparison.OrdinalIgnoreCase)
                ? [root]
                : root.Elements()
                    .Where(element => string.Equals(element.Name.LocalName, "BusinessCard", StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (cardElements.Count == 0)
            {
                throw new BusinessCardImportException(
                    "XML import file must contain a BusinessCard element or BusinessCards root.");
            }

            return cardElements
                .Select((element, index) => ToDto(element, $"XML BusinessCard #{index + 1}"))
                .ToList();
        }
        catch (BusinessCardImportException)
        {
            throw;
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.Xml.XmlException)
        {
            throw new BusinessCardImportException("XML import file is invalid.", [ex.Message]);
        }
    }

    public BusinessCardCreateDto ParseQrPayload(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new BusinessCardImportException("QR code payload is empty.");
        }

        var card = TryParseJson(payload)
            ?? TryParseXml(payload)
            ?? TryParseVCard(payload)
            ?? TryParseKeyValuePayload(payload)
            ?? throw new BusinessCardImportException(
                "QR code payload format is not supported.",
                [
                    "Supported QR payloads: JSON object, XML BusinessCard, vCard, or key-value lines."
                ]);

        card.PhotoBase64 = null;
        return card;
    }

    private static BusinessCardCreateDto? TryParseJson(string payload)
    {
        try
        {
            return JsonSerializer.Deserialize<BusinessCardCreateDto>(payload, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private BusinessCardCreateDto? TryParseXml(string payload)
    {
        try
        {
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(payload));
            return ParseXmlAsync(stream, CancellationToken.None).GetAwaiter().GetResult().FirstOrDefault();
        }
        catch (BusinessCardImportException)
        {
            return null;
        }
    }

    private static BusinessCardCreateDto? TryParseVCard(string payload)
    {
        if (!payload.Contains("BEGIN:VCARD", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var values = payload
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(':', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(
                parts => parts[0].Split(';')[0].Trim(),
                parts => parts[1].Trim(),
                StringComparer.OrdinalIgnoreCase);

        return new BusinessCardCreateDto
        {
            Name = GetValue(values, "FN", "N"),
            Gender = GetValue(values, "GENDER", "X-GENDER"),
            DateOfBirth = ParseDate(GetValue(values, "BDAY", "DOB", "X-DOB"), "QR vCard date of birth"),
            Email = GetValue(values, "EMAIL"),
            Phone = GetValue(values, "TEL"),
            Address = GetValue(values, "ADR"),
            PhotoBase64 = null
        };
    }

    private static BusinessCardCreateDto? TryParseKeyValuePayload(string payload)
    {
        var values = payload
            .Split(["\r\n", "\n", ";"], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(['=', ':'], 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(
                parts => parts[0].Trim(),
                parts => parts[1].Trim(),
                StringComparer.OrdinalIgnoreCase);

        if (values.Count == 0)
        {
            return null;
        }

        return new BusinessCardCreateDto
        {
            Name = GetValue(values, "Name"),
            Gender = GetValue(values, "Gender"),
            DateOfBirth = ParseDate(GetValue(values, "DateOfBirth", "DOB"), "QR date of birth"),
            Email = GetValue(values, "Email"),
            Phone = GetValue(values, "Phone"),
            PhotoBase64 = null,
            Address = GetValue(values, "Address")
        };
    }

    private static BusinessCardCreateDto ToDto(BusinessCardCsvRecord record, string location)
    {
        return new BusinessCardCreateDto
        {
            Name = record.Name,
            Gender = record.Gender,
            DateOfBirth = ParseDate(record.DateOfBirth, $"{location} DateOfBirth"),
            Email = record.Email,
            Phone = record.Phone,
            PhotoBase64 = string.IsNullOrWhiteSpace(record.PhotoBase64) ? record.Photo : record.PhotoBase64,
            Address = record.Address
        };
    }

    private static BusinessCardCreateDto ToDto(XElement element, string location)
    {
        return new BusinessCardCreateDto
        {
            Name = ElementValue(element, "Name"),
            Gender = ElementValue(element, "Gender"),
            DateOfBirth = ParseDate(ElementValue(element, "DateOfBirth"), $"{location} DateOfBirth"),
            Email = ElementValue(element, "Email"),
            Phone = ElementValue(element, "Phone"),
            PhotoBase64 = ElementValue(element, "PhotoBase64", "Photo"),
            Address = ElementValue(element, "Address")
        };
    }

    private static DateOnly ParseDate(string? value, string fieldName)
    {
        var supportedFormats = new[] { "yyyy-MM-dd", "yyyyMMdd" };

        if (DateOnly.TryParseExact(value, supportedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
            || DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            return parsedDate;
        }

        throw new BusinessCardImportException($"{fieldName} must be a valid date in yyyy-MM-dd format.");
    }

    private static string ElementValue(XElement element, params string[] names)
    {
        return names
            .Select(name => element.Elements().FirstOrDefault(child =>
                string.Equals(child.Name.LocalName, name, StringComparison.OrdinalIgnoreCase)))
            .FirstOrDefault(child => child is not null)
            ?.Value
            .Trim() ?? string.Empty;
    }

    private static string GetValue(IReadOnlyDictionary<string, string> values, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (values.TryGetValue(key, out var value))
            {
                return value.Trim();
            }
        }

        return string.Empty;
    }

    private sealed class BusinessCardCsvRecord
    {
        public string Name { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string DateOfBirth { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? PhotoBase64 { get; set; }

        public string? Photo { get; set; }

        public string Address { get; set; } = string.Empty;
    }
}
