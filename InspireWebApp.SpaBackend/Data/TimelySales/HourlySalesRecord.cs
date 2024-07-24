using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.TimelySales;

public record HourlySalesRecord : FinancialRecord
{
    
    public required int Hour { get; init; }
}

public class HourlySalesDataEntityConfiguration : IEntityTypeConfiguration<HourlySalesRecord>
{
    public void Configure(EntityTypeBuilder<HourlySalesRecord> builder)
    {
        builder.HasNoKey();
    }
}