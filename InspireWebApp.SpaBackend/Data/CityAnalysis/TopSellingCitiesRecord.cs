using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.CityAnalysis;

public record TopSellingCitiesRecord : FinancialRecord
{
    public required string CityName { get; init; }
}

public class TopSellingCitiesDataEntityConfiguration : IEntityTypeConfiguration<TopSellingCitiesRecord>
{
    public void Configure(EntityTypeBuilder<TopSellingCitiesRecord> builder)
    {
        builder.HasNoKey();
    }
}