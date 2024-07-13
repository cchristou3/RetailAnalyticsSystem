using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.Tags;

public record TagIdentifier
{
    public required int Id { get; init; }

    public static implicit operator TagIdentifier(Tag productTag)
    {
        return new TagIdentifier
        {
            Id = productTag.Id
        };
    }
}

public class Tag
{
    public Tag()
    {
    }

    public Tag(TagIdentifier identifier)
    {
        Id = identifier.Id;
    }

    public int Id { get; set; }

    [MaxLength(70)] [MinLength(3)] public required string Name { get; set; }
}

public class TagEntityConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasIndex(entity => entity.Name)
            .IsUnique();
    }
}