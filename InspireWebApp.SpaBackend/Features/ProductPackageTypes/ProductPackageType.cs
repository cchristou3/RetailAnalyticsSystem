using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.ProductPackageTypes;

public record ProductPackageTypeIdentifier
{
    public required int Id { get; init; }

    public static implicit operator ProductPackageTypeIdentifier(ProductPackageType productCategory)
    {
        return new ProductPackageTypeIdentifier
        {
            Id = productCategory.Id
        };
    }
}

public class ProductPackageType
{
    public ProductPackageType()
    {
    }

    public ProductPackageType(ProductPackageTypeIdentifier identifier)
    {
        Id = identifier.Id;
    }

    public int Id { get; set; }

    [MaxLength(70)] [MinLength(3)] public required string Name { get; set; }
}

public class ProductPackageTypeEntityConfiguration : IEntityTypeConfiguration<ProductPackageType>
{
    public void Configure(EntityTypeBuilder<ProductPackageType> builder)
    {
        builder.HasIndex(entity => entity.Name)
            .IsUnique();
    }
}