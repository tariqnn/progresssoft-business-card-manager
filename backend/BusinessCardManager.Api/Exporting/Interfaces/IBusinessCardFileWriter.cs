using BusinessCardManager.Api.Dtos;

namespace BusinessCardManager.Api.Exporting.Interfaces;

public interface IBusinessCardFileWriter
{
    string ContentType { get; }

    string FileExtension { get; }

    byte[] Write(IReadOnlyList<BusinessCardResponseDto> cards);
}
