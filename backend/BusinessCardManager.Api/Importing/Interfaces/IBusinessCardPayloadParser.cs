using BusinessCardManager.Api.Dtos;

namespace BusinessCardManager.Api.Importing.Interfaces;

public interface IBusinessCardPayloadParser
{
    IReadOnlyList<BusinessCardCreateDto> ParseCsv(Stream stream);

    Task<IReadOnlyList<BusinessCardCreateDto>> ParseXmlAsync(Stream stream, CancellationToken cancellationToken);

    BusinessCardCreateDto ParseQrPayload(string payload);
}
