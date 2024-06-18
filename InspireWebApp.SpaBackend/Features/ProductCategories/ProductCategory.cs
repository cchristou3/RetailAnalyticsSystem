using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.ProductCategories;

public record ProductCategoryIdentifier
{
    public required int Id { get; init; }

    public static implicit operator ProductCategoryIdentifier(ProductCategory productCategory) => new()
    {
        Id = productCategory.Id,
    };
}

public class ProductCategory
{
    public ProductCategory()
    {
    }

    public ProductCategory(ProductCategoryIdentifier identifier)
    {
        Id = identifier.Id;
    }

    public int Id { get; set; }

    [MaxLength(70)]
    [MinLength(3)]
    public required string Name { get; set; }
}

public class ProductCategoryEntityConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasIndex(entity => entity.Name)
            .IsUnique();
    }
}
