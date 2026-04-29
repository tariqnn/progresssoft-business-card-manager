using BusinessCardManager.Api.Dtos;

namespace BusinessCardManager.Api.Importing.Interfaces;

public interface IBusinessCardImportService
{
    Task<BusinessCardImportResultDto> ImportCsvAsync(IFormFile file, CancellationToken cancellationToken);

    Task<BusinessCardImportResultDto> ImportXmlAsync(IFormFile file, CancellationToken cancellationToken);

    Task<BusinessCardImportResultDto> ImportQrCodeAsync(IFormFile file, CancellationToken cancellationToken);
}
