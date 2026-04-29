using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Exporting.Interfaces;
using BusinessCardManager.Api.Importing.Exceptions;
using BusinessCardManager.Api.Importing.Interfaces;
using BusinessCardManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCardManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessCardsController(
    IBusinessCardService businessCardService,
    IBusinessCardImportService businessCardImportService,
    IBusinessCardExportService businessCardExportService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BusinessCardResponseDto>>> GetBusinessCards(
        [FromQuery] BusinessCardQueryDto query,
        CancellationToken cancellationToken)
    {
        var cards = await businessCardService.GetBusinessCardsAsync(query, cancellationToken);
        return Ok(cards);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BusinessCardResponseDto>> GetBusinessCard(
        int id,
        CancellationToken cancellationToken)
    {
        var card = await businessCardService.GetBusinessCardAsync(id, cancellationToken);

        return card is null ? NotFound() : Ok(card);
    }

    [HttpPost]
    public async Task<ActionResult<BusinessCardResponseDto>> CreateBusinessCard(
        BusinessCardCreateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await businessCardService.CreateBusinessCardAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetBusinessCard), new { id = response.Id }, response);
        }
        catch (BusinessCardValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("import/csv")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BusinessCardImportResultDto>> ImportCsv(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        return await ImportAsync(
            () => businessCardImportService.ImportCsvAsync(file, cancellationToken));
    }

    [HttpPost("import/xml")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BusinessCardImportResultDto>> ImportXml(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        return await ImportAsync(
            () => businessCardImportService.ImportXmlAsync(file, cancellationToken));
    }

    [HttpPost("import/qr")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BusinessCardImportResultDto>> ImportQrCode(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        return await ImportAsync(
            () => businessCardImportService.ImportQrCodeAsync(file, cancellationToken));
    }

    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportCsv(
        [FromQuery] BusinessCardQueryDto query,
        CancellationToken cancellationToken)
    {
        var exportFile = await businessCardExportService.ExportCsvAsync(query, cancellationToken);
        return File(exportFile.Contents, exportFile.ContentType, exportFile.FileName);
    }

    [HttpGet("export/xml")]
    public async Task<IActionResult> ExportXml(
        [FromQuery] BusinessCardQueryDto query,
        CancellationToken cancellationToken)
    {
        var exportFile = await businessCardExportService.ExportXmlAsync(query, cancellationToken);
        return File(exportFile.Contents, exportFile.ContentType, exportFile.FileName);
    }

    [HttpGet("{id:int}/export/csv")]
    public async Task<IActionResult> ExportCardCsv(
        int id,
        CancellationToken cancellationToken)
    {
        var exportFile = await businessCardExportService.ExportCardCsvAsync(id, cancellationToken);
        return exportFile is null
            ? NotFound()
            : File(exportFile.Contents, exportFile.ContentType, exportFile.FileName);
    }

    [HttpGet("{id:int}/export/xml")]
    public async Task<IActionResult> ExportCardXml(
        int id,
        CancellationToken cancellationToken)
    {
        var exportFile = await businessCardExportService.ExportCardXmlAsync(id, cancellationToken);
        return exportFile is null
            ? NotFound()
            : File(exportFile.Contents, exportFile.ContentType, exportFile.FileName);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBusinessCard(int id, CancellationToken cancellationToken)
    {
        var deleted = await businessCardService.DeleteBusinessCardAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    private async Task<ActionResult<BusinessCardImportResultDto>> ImportAsync(
        Func<Task<BusinessCardImportResultDto>> importAction)
    {
        try
        {
            var result = await importAction();
            return CreatedAtAction(nameof(GetBusinessCards), result);
        }
        catch (BusinessCardImportException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                errors = ex.Errors
            });
        }
    }

}
