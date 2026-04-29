using System.Globalization;
using System.Text;
using System.Xml.Linq;
using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Exporting.Interfaces;

namespace BusinessCardManager.Api.Exporting.Services;

public class XmlBusinessCardFileWriter : IBusinessCardFileWriter
{
    public string ContentType => "application/xml";

    public string FileExtension => "xml";

    public byte[] Write(IReadOnlyList<BusinessCardResponseDto> cards)
    {
        var document = new XDocument(
            new XElement(
                "BusinessCards",
                cards.Select(card =>
                    new XElement(
                        "BusinessCard",
                        new XElement("Id", card.Id),
                        new XElement("Name", card.Name),
                        new XElement("Gender", card.Gender),
                        new XElement(
                            "DateOfBirth",
                            card.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                        new XElement("Email", card.Email),
                        new XElement("Phone", card.Phone),
                        new XElement("PhotoBase64", card.PhotoBase64 ?? string.Empty),
                        new XElement("Address", card.Address),
                        new XElement(
                            "CreatedAtUtc",
                            card.CreatedAtUtc.ToString("O", CultureInfo.InvariantCulture))))));

        return Encoding.UTF8.GetBytes(document.ToString(SaveOptions.DisableFormatting));
    }
}
