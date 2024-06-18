using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InspireWebApp.SpaBackend.Features.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.PromotionTypes;

public record PromotionTypeIdentifier
{
    public required int Id { get; init; }

    public static implicit operator PromotionTypeIdentifier(PromotionType promotionType) => new()
    {
        Id = promotionType.Id,
    };
}

public class PromotionType
{
    public PromotionType()
    {
    }

    public PromotionType(PromotionTypeIdentifier identifier)
    {
        Id = identifier.Id;
    }

    public int Id { get; set; }

    [MaxLength(70)]
    [MinLength(3)]
    public required string Name { get; set; }

    public ICollection<Product>? Products { get; set; }
}

public class PromotionTypeEntityConfiguration : IEntityTypeConfiguration<PromotionType>
{
    public void Configure(EntityTypeBuilder<PromotionType> builder)
    {
        builder.HasIndex(entity => entity.Name)
            .IsUnique();
    }
}
