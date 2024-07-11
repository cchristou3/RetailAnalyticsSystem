using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InspireWebApp.SpaBackend.Features.PromotionTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.Invoices;

public record InvoiceIdentifier
{
    public required int Id { get; init; }

    public static implicit operator InvoiceIdentifier(Invoice invoice) => new()
    {
        Id = invoice.Id,
    };
}

public class Invoice
{
    public Invoice()
    {
    }

    public Invoice(InvoiceIdentifier identifier)
    {
        Id = identifier.Id;
    }

    public int Id { get; set; }

    [MaxLength(70)]
    [MinLength(3)]
    public required string Name { get; set; }

    public required decimal Price { get; set; }

    public required string? Description { get; set; }

    public ICollection<PromotionType>? PromotionTypes { get; set; }
}

public class InvoiceEntityConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasIndex(entity => entity.Name)
            .IsUnique();

        builder.Property(p => p.Price)
            .HasPrecision(15, 3);
    }
}
