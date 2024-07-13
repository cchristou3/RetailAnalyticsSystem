using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InspireWebApp.SpaBackend.Features.ProductPackageTypes;
using InspireWebApp.SpaBackend.Features.ProductTags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.Products;

public record ProductIdentifier
{
    public required int Id { get; init; }

    public static implicit operator ProductIdentifier(Product product)
    {
        return new ProductIdentifier
        {
            Id = product.Id
        };
    }
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

    public ProductPackageType PackageType { get; set; } = null!;
    public int PackageTypeId { get; set; }

    [MaxLength(100)] [MinLength(3)] public required string Name { get; set; }

    public required decimal Weight { get; set; }

    public ICollection<ProductTag>? ProductTags { get; set; }
}

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasIndex(entity => entity.Name)
            .IsUnique();

        builder.Property(p => p.Weight)
            .HasPrecision(9, 2);
    }
}