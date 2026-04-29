using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Models;

namespace BusinessCardManager.Api.Mapping;

public static class BusinessCardMappings
{
    public static BusinessCard ToEntity(this BusinessCardCreateDto dto)
    {
        return new BusinessCard
        {
            Name = dto.Name.Trim(),
            Gender = dto.Gender.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Email = dto.Email.Trim(),
            Phone = dto.Phone.Trim(),
            PhotoBase64 = string.IsNullOrWhiteSpace(dto.PhotoBase64) ? null : dto.PhotoBase64.Trim(),
            Address = dto.Address.Trim()
        };
    }

    public static BusinessCardResponseDto ToResponse(this BusinessCard card)
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
