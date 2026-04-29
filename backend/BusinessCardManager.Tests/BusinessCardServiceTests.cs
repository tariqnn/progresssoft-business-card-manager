using BusinessCardManager.Api.Data;
using BusinessCardManager.Api.Dtos;
using BusinessCardManager.Api.Services;
using BusinessCardManager.Api.Validators;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardManager.Tests;

public class BusinessCardServiceTests
{
    [Fact]
    public async Task CreateBusinessCardAsync_SavesCard()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var created = await service.CreateBusinessCardAsync(CreateDto("save@example.com"), CancellationToken.None);

        Assert.True(created.Id > 0);
        Assert.Single(dbContext.BusinessCards);
    }

    [Fact]
    public async Task GetBusinessCardsAsync_AppliesEmailFilter()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        await service.CreateBusinessCardAsync(CreateDto("first@example.com"), CancellationToken.None);
        await service.CreateBusinessCardAsync(CreateDto("second@example.com"), CancellationToken.None);

        var results = await service.GetBusinessCardsAsync(
            new BusinessCardQueryDto { Email = "second" },
            CancellationToken.None);

        var card = Assert.Single(results);
        Assert.Equal("second@example.com", card.Email);
    }

    [Fact]
    public async Task DeleteBusinessCardAsync_RemovesExistingCard()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var created = await service.CreateBusinessCardAsync(CreateDto("delete@example.com"), CancellationToken.None);

        var deleted = await service.DeleteBusinessCardAsync(created.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Empty(dbContext.BusinessCards);
    }

    private static BusinessCardService CreateService(AppDbContext dbContext)
    {
        return new BusinessCardService(dbContext, new BusinessCardValidator());
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static BusinessCardCreateDto CreateDto(string email)
    {
        return new BusinessCardCreateDto
        {
            Name = "Service User",
            Gender = "Male",
            DateOfBirth = new DateOnly(1998, 1, 15),
            Email = email,
            Phone = "+962790000000",
            PhotoBase64 = null,
            Address = "Amman"
        };
    }
}
