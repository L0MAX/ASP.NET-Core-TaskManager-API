using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.TokenHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.HasIndex(rt => rt.TokenHash).IsUnique();

        builder.Property(rt => rt.ExpiresAt).IsRequired();
        builder.Property(rt => rt.ReplacedByTokenHash).HasMaxLength(512);
        builder.Property(rt => rt.CreatedAt).IsRequired();
    }
}
