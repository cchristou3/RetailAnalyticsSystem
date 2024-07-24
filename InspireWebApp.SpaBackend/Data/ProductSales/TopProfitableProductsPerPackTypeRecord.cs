using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.ProductSales;

public record TopProfitableProductsPerPackTypeRecord : FinancialRecord
{
    public required string ProductPackType { get; init; }
    public required string ProductName { get; init; }
    public required int Rank { get; init; }
    public required decimal Contribution { get; init; }
}

public class TopProfitableProductsPerPackTypeDataEntityConfiguration : IEntityTypeConfiguration<TopProfitableProductsPerPackTypeRecord>
{
    public void Configure(EntityTypeBuilder<TopProfitableProductsPerPackTypeRecord> builder)
    {
        builder.HasNoKey();
    }
}