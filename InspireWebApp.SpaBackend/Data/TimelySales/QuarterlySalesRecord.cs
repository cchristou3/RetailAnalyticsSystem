using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.TimelySales;

public record QuarterlySalesRecord : FinancialRecord
{
    
    public required int Quarter { get; init; }
}

public class QuarterlySalesDataEntityConfiguration : IEntityTypeConfiguration<QuarterlySalesRecord>
{
    public void Configure(EntityTypeBuilder<QuarterlySalesRecord> builder)
    {
        builder.HasNoKey();
    }
}