using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data;

public record AssociationRuleRecord
{
    public required string Rule { get; init; }

    public required string LeftHand { get; init; }
    public required string RightHand { get; init; }

    public required decimal Confidence { get; init; }
    public required decimal Support { get; init; }
    public required decimal Lift { get; init; }
    
    public string? Segment { get; set; }
}

public record AssociationRule
{
    public int Id { get; set; }
    public required string LeftHand { get; init; }
    public required string RightHand { get; init; }

    public required decimal Confidence { get; init; }
    public required decimal Support { get; init; }
    public required decimal Lift { get; init; }
    
    public string Segment { get; set; }
}

public class RulesSupportConfidenceDataEntityConfiguration : IEntityTypeConfiguration<AssociationRule>
{
    public void Configure(EntityTypeBuilder<AssociationRule> builder)
    {
        builder.Property(entity => entity.LeftHand)
            .HasMaxLength(1000)
            .IsUnicode(false);
        
        builder.Property(entity => entity.RightHand)
            .HasMaxLength(1000)
            .IsUnicode(false);
        
        builder.Property(entity => entity.Segment)
            .HasMaxLength(150)
            .IsUnicode(false);

        builder.Property(entity => entity.Confidence)
            .HasPrecision(6, 2);
        
        builder.Property(entity => entity.Support)
            .HasPrecision(9, 5);
        
        builder.Property(entity => entity.Lift)
            .HasPrecision(6, 2);

    }
}