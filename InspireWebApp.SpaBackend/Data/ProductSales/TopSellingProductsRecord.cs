using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.ProductSales;

public record TopSellingProductsRecord : FinancialRecord
{
    public required string ProductName { get; init; }
}

public class TopProductsDataEntityConfiguration : IEntityTypeConfiguration<TopSellingProductsRecord>
{
    public void Configure(EntityTypeBuilder<TopSellingProductsRecord> builder)
    {
        builder.HasNoKey();
    }
}