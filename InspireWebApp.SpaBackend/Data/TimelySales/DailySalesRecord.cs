using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data.TimelySales;

public record DailySalesRecord : FinancialRecord
{
    public required DateTime Date { get; init; }
}

public class DailySalesDataEntityConfiguration : IEntityTypeConfiguration<DailySalesRecord>
{
    public void Configure(EntityTypeBuilder<DailySalesRecord> builder)
    {
        builder.HasNoKey();
    }
}