using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.ProductSales;

public record TopProfitableProductsPerTagsRecord : FinancialRecord
{
    public required string ProductTag { get; init; }
    public required string ProductName { get; init; }
    public required int Rank { get; init; }
    public required decimal Contribution { get; init; }
}

public class TopProfitableProductsPerTagsDataEntityConfiguration : IEntityTypeConfiguration<TopProfitableProductsPerTagsRecord>
{
    public void Configure(EntityTypeBuilder<TopProfitableProductsPerTagsRecord> builder)
    {
        builder.HasNoKey();
    }
}