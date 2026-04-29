using BusinessCardManager.Api.Dtos;

namespace BusinessCardManager.Api.Exporting.Interfaces;

public interface IBusinessCardExportService
{
    Task<ExportFileDto> ExportCsvAsync(
        BusinessCardQueryDto query,
        CancellationToken cancellationToken);

    Task<ExportFileDto> ExportXmlAsync(
        BusinessCardQueryDto query,
        CancellationToken cancellationToken);

    Task<ExportFileDto?> ExportCardCsvAsync(
        int id,
        CancellationToken cancellationToken);

    Task<ExportFileDto?> ExportCardXmlAsync(
        int id,
        CancellationToken cancellationToken);
}
