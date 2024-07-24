using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.ProductSales;

public record SalesByProductPackTypeRecord : FinancialRecord
{
    public required string ProductPackTypeName { get; init; }
}

public class ProductPackTypeDataEntityConfiguration : IEntityTypeConfiguration<SalesByProductPackTypeRecord>
{
    public void Configure(EntityTypeBuilder<SalesByProductPackTypeRecord> builder)
    {
        builder.HasNoKey();
    }
}