using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.CityAnalysis;

public record CustomerDistributionByCityRecord
{
    public required string CityName { get; init; }
    public required int NumberOfCustomers { get; init; }
}

public class CustomerDistributionByCityDataEntityConfiguration : IEntityTypeConfiguration<CustomerDistributionByCityRecord>
{
    public void Configure(EntityTypeBuilder<CustomerDistributionByCityRecord> builder)
    {
        builder.HasNoKey();
    }
}