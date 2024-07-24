using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.TimelySales;

public record DaySalesRecord : FinancialRecord
{
    public required string Day { get; init; }
    public required int DayNo { get; init; }
}

public class DaySalesDataEntityConfiguration : IEntityTypeConfiguration<DaySalesRecord>
{
    public void Configure(EntityTypeBuilder<DaySalesRecord> builder)
    {
        builder.HasNoKey();
    }
}