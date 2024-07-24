using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.CustomerAnalysis;

public record CustomerDistributionByCategoryRecord : CustomerDistributionRecord
{
    public required string CustomerCategoryName { get; init; }
}

public class CustomerDistributionByCategoryDataEntityConfiguration : IEntityTypeConfiguration<CustomerDistributionByCategoryRecord>
{
    public void Configure(EntityTypeBuilder<CustomerDistributionByCategoryRecord> builder)
    {
        builder.HasNoKey();
    }
}