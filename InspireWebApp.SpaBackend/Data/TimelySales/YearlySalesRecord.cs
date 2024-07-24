using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.TimelySales;

public record YearlySalesRecord : FinancialRecord
{
    
    public required int Year { get; init; }
}

public class YearlySalesDataEntityConfiguration : IEntityTypeConfiguration<YearlySalesRecord>
{
    public void Configure(EntityTypeBuilder<YearlySalesRecord> builder)
    {
        builder.HasNoKey();
    }
}