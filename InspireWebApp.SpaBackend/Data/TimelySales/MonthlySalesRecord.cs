using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.TimelySales;

public record MonthlySalesRecord : FinancialRecord
{
    
    public required int Month { get; init; }
}

public class MonthlySalesDataEntityConfiguration : IEntityTypeConfiguration<MonthlySalesRecord>
{
    public void Configure(EntityTypeBuilder<MonthlySalesRecord> builder)
    {
        builder.HasNoKey();
    }
}