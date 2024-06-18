using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InspireWebApp.SpaBackend.Features.ProductCategories;
using InspireWebApp.SpaBackend.Features.PromotionTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.Products;

public record ProductIdentifier
{
    public required int Id { get; init; }

    public static implicit operator ProductIdentifier(Product product) => new()
    {
        Id = product.Id,
    };
}

public class Product
{
    public Product()
    {
    }

    public Product(ProductIdentifier identifier)
    {
        Id = identifier.Id;
    }

    public int Id { get; set; }

    public ProductCategory Category { get; set; } = null!;
    public int CategoryId { get; set; }

    [MaxLength(70)]
    [MinLength(3)]
    public required string Name { get; set; }

    public required decimal Price { get; set; }

    public required string? Description { get; set; }

    public ICollection<PromotionType>? PromotionTypes { get; set; }
}

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasIndex(entity => entity.Name)
            .IsUnique();

        builder.Property(p => p.Price)
            .HasPrecision(15, 3);
    }
}
