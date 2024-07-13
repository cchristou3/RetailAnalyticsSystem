using InspireWebApp.SpaBackend.Features.Products;
using InspireWebApp.SpaBackend.Features.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.ProductTags;

public record ProductTagIdentifier
{
    public required int Id { get; init; }

    public static implicit operator ProductTagIdentifier(ProductTag productTag)
    {
        return new ProductTagIdentifier
        {
            Id = productTag.Id
        };
    }
}

public class ProductTag
{
    public ProductTag()
    {
    }

    public ProductTag(ProductTagIdentifier identifier)
    {
        Id = identifier.Id;
    }

    public int Id { get; set; }

    public Product Product { get; set; }
    public int ProductId { get; set; }
    
    public Tag Tag { get; set; }
    public int TagId { get; set; }
}

public class ProductTagEntityConfiguration : IEntityTypeConfiguration<ProductTag>
{
    public void Configure(EntityTypeBuilder<ProductTag> builder)
    {
        builder.HasIndex(entity => new { entity.TagId, entity.ProductId })
            .IsUnique();
    }
}