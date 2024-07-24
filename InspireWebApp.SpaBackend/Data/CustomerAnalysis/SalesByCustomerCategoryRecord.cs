using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.CustomerAnalysis;

public record SalesByCustomerCategoryRecord : FinancialRecord
{
    public required string CustomerCategoryName { get; init; }
}

public class SalesByCustomerCategoryDataEntityConfiguration : IEntityTypeConfiguration<SalesByCustomerCategoryRecord>
{
    public void Configure(EntityTypeBuilder<SalesByCustomerCategoryRecord> builder)
    {
        builder.HasNoKey();
    }
}