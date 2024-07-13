using InspireWebApp.SpaBackend.Features.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.Invoices;

public record InvoiceDetailIdentifier
{
    public required int Id { get; init; }

    public static implicit operator InvoiceDetailIdentifier(InvoiceDetail invoice)
    {
        return new InvoiceDetailIdentifier
        {
            Id = invoice.Id
        };
    }
}

public class InvoiceDetail
{
    public InvoiceDetail()
    {
    }

    public InvoiceDetail(InvoiceDetailIdentifier identifier)
    {
        Id = identifier.Id;
    }

    public int Id { get; set; }

    public required Invoice Invoice { get; set; }
    public required int InvoiceId { get; set; }

    public required Product Product { get; set; }
    public required int ProductId { get; set; }

    public required decimal UnitPrice { get; set; }

    public required int Quantity { get; set; }
}

public class InvoiceDetailEntityConfiguration : IEntityTypeConfiguration<InvoiceDetail>
{
    public void Configure(EntityTypeBuilder<InvoiceDetail> builder)
    {
        builder.Property(p => p.UnitPrice)
            .HasPrecision(9, 2);
    }
}