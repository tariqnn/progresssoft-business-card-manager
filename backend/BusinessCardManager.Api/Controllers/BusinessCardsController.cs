using BusinessCardManager.Api.Data;
using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Models;
using BusinessCardManager.Api.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessCardsController(
    AppDbContext dbContext,
    IBusinessCardValidator businessCardValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BusinessCardResponseDto>>> GetBusinessCards(
        [FromQuery] BusinessCardQueryDto query,
        CancellationToken cancellationToken)
    {
        var cards = dbContext.BusinessCards.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            cards=cards.Where(card => card.Name.Contains(query.Name));
        }

        if (!string.IsNullOrWhiteSpace(query.Gender))
        {
            cards = cards.Where(card => card.Gender == query.Gender);
        }

        if (query.DateOfBirth.HasValue)
        {
            cards = cards.Where(card => card.DateOfBirth == query.DateOfBirth.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            cards = cards.Where(card => card.Email.Contains(query.Email));
        }

        if (!string.IsNullOrWhiteSpace(query.Phone))
        {
            cards = cards.Where(card => card.Phone.Contains(query.Phone));
        }

        var results = await cards
            .OrderBy(card => card.Name)
            .Select(card => ToResponse(card))
            .ToListAsync(cancellationToken);

        return Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BusinessCardResponseDto>> GetBusinessCard(
        int id,
        CancellationToken cancellationToken)
    {
        var card = await dbContext.BusinessCards
            .AsNoTracking()
            .FirstOrDefaultAsync(card => card.Id == id, cancellationToken);

        return card is null ? NotFound() : Ok(ToResponse(card));
    }

    [HttpPost]
    public async Task<ActionResult<BusinessCardResponseDto>> CreateBusinessCard(
        BusinessCardCreateDto dto,
        CancellationToken cancellationToken)
    {
        if (!businessCardValidator.IsValidBase64Photo(dto.PhotoBase64, out var photoError))
        {
            return BadRequest(new { message = photoError });
        }

        var card = new BusinessCard
        {
            Name = dto.Name.Trim(),
            Gender = dto.Gender.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Email = dto.Email.Trim(),
            Phone = dto.Phone.Trim(),
            PhotoBase64 = string.IsNullOrWhiteSpace(dto.PhotoBase64) ? null : dto.PhotoBase64.Trim(),
            Address = dto.Address.Trim()
        };

        dbContext.BusinessCards.Add(card);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = ToResponse(card);
        return CreatedAtAction(nameof(GetBusinessCard), new { id = card.Id }, response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBusinessCard(int id, CancellationToken cancellationToken)
    {
        var card = await dbContext.BusinessCards.FindAsync([id], cancellationToken);

        if (card is null)
        {
            return NotFound();
        }

        dbContext.BusinessCards.Remove(card);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static BusinessCardResponseDto ToResponse(BusinessCard card)
    {
        return new BusinessCardResponseDto
        {
            Id = card.Id,
            Name = card.Name,
            Gender = card.Gender,
            DateOfBirth = card.DateOfBirth,
            Email = card.Email,
            Phone = card.Phone,
            PhotoBase64 = card.PhotoBase64,
            Address = card.Address,
            CreatedAtUtc = card.CreatedAtUtc
        };
    }

}
