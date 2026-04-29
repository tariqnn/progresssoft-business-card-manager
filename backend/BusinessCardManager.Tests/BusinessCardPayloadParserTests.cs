using System.Text;
using BusinessCardManager.Api.Importing.Parsers;

namespace BusinessCardManager.Tests;

public class BusinessCardPayloadParserTests
{
    private readonly BusinessCardPayloadParser parser = new();

    [Fact]
    public void ParseCsv_ReturnsBusinessCards()
    {
        const string csv = """
            Name,Gender,DateOfBirth,Email,Phone,PhotoBase64,Address
            Test User,Male,1998-01-15,test@example.com,+962790000000,,Amman
            """;

        using var stream = ToStream(csv);

        var cards = parser.ParseCsv(stream);

        Assert.Single(cards);
        Assert.Equal("Test User", cards[0].Name);
        Assert.Equal(new DateOnly(1998, 1, 15), cards[0].DateOfBirth);
        Assert.Equal("test@example.com", cards[0].Email);
    }

    [Fact]
    public async Task ParseXmlAsync_ReturnsBusinessCards()
    {
        const string xml = """
            <BusinessCards>
              <BusinessCard>
                <Name>XML User</Name>
                <Gender>Female</Gender>
                <DateOfBirth>1997-02-20</DateOfBirth>
                <Email>xml@example.com</Email>
                <Phone>+962790000001</Phone>
                <Address>Irbid</Address>
              </BusinessCard>
            </BusinessCards>
            """;

        using var stream = ToStream(xml);

        var cards = await parser.ParseXmlAsync(stream, CancellationToken.None);

        Assert.Single(cards);
        Assert.Equal("XML User", cards[0].Name);
        Assert.Equal(new DateOnly(1997, 2, 20), cards[0].DateOfBirth);
        Assert.Equal("xml@example.com", cards[0].Email);
    }

    [Fact]
    public void ParseQrPayload_WithJson_ReturnsBusinessCardWithoutPhoto()
    {
        const string payload = """
            {
              "name": "QR User",
              "gender": "Male",
              "dateOfBirth": "1996-03-25",
              "email": "qr@example.com",
              "phone": "+962790000002",
              "photoBase64": "ignored-here",
              "address": "Zarqa"
            }
            """;

        var card = parser.ParseQrPayload(payload);

        Assert.Equal("QR User", card.Name);
        Assert.Equal(new DateOnly(1996, 3, 25), card.DateOfBirth);
        Assert.Equal("qr@example.com", card.Email);
        Assert.Null(card.PhotoBase64);
    }

    private static MemoryStream ToStream(string value)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(value));
    }
}
