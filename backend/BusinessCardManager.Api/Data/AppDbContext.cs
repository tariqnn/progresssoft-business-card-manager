using BusinessCardManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardManager.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<BusinessCard> BusinessCards => Set<BusinessCard>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessCard>(entity =>
        {
            entity.HasKey(card => card.Id);

            entity.Property(card => card.Name).HasMaxLength(150).IsRequired();
            entity.Property(card => card.Gender).HasMaxLength(20).IsRequired();
            entity.Property(card => card.DateOfBirth).HasColumnType("date");
            entity.Property(card => card.Email).HasMaxLength(254).IsRequired();
            entity.Property(card => card.Phone).HasMaxLength(30).IsRequired();
            entity.Property(card => card.Address).HasMaxLength(500).IsRequired();
            entity.Property(card => card.PhotoBase64).HasColumnType("nvarchar(max)");
            entity.Property(card => card.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
