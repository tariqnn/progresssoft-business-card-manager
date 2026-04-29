namespace BusinessCardManager.Api.Importing.Interfaces;

public interface IQrCodeReader
{
    Task<string> ReadPayloadAsync(Stream imageStream, CancellationToken cancellationToken);
}
