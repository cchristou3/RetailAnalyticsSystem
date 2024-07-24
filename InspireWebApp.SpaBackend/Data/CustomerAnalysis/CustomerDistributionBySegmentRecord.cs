using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.CustomerAnalysis;

public record CustomerDistributionBySegmentRecord : CustomerDistributionRecord
{
    public required string SegmentName { get; init; }
}

public class CustomerDistributionBySegmentDataEntityConfiguration : IEntityTypeConfiguration<CustomerDistributionBySegmentRecord>
{
    public void Configure(EntityTypeBuilder<CustomerDistributionBySegmentRecord> builder)
    {
        builder.HasNoKey();
    }
}