using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.ProductSales;

public record SalesByProductTagsRecord : FinancialRecord
{
    public required string ProductTagName { get; init; }
}

public class SalesByProductTagsDataEntityConfiguration : IEntityTypeConfiguration<SalesByProductTagsRecord>
{
    public void Configure(EntityTypeBuilder<SalesByProductTagsRecord> builder)
    {
        builder.HasNoKey();
    }
}