using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data;

public class SalesRecord
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Quarter { get; set; }

    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public int ProductId { get; set; }
    public string? ProductName { get; set; }

    public double? Size { get; set; }
    public double? Price { get; set; }
    public double? Quantity { get; set; }
    public double? SalesValue { get; set; }
    public double? SalesVolume { get; set; }
}

internal class SalesRecordEntityTypeConfiguration : IEntityTypeConfiguration<SalesRecord>
{
    public void Configure(EntityTypeBuilder<SalesRecord> builder)
    {
        builder.ToTable("Sales");
        builder.HasNoKey();

        builder.Property(r => r.Year).HasColumnName("YEAR");
        builder.Property(r => r.Month).HasColumnName("MONTH");
        builder.Property(r => r.Day).HasColumnName("DAY");
        builder.Property(r => r.Quarter).HasColumnName("QUARTER");
        builder.Property(r => r.CategoryId).HasColumnName("CATEGORY_ID");
        builder.Property(r => r.CategoryName).HasColumnName("CATEGORY_NAME");
        builder.Property(r => r.ProductId).HasColumnName("PRODUCT_ID");
        builder.Property(r => r.ProductName).HasColumnName("PRODUCT_NAME");
        builder.Property(r => r.Size).HasColumnName("M_SIZE");
        builder.Property(r => r.Price).HasColumnName("M_PRICE");
        builder.Property(r => r.Quantity).HasColumnName("M_QUANTITY");
        builder.Property(r => r.SalesValue).HasColumnName("M_SALES_VALUE");
        builder.Property(r => r.SalesVolume).HasColumnName("M_SALES_VOLUME");

        foreach (var property in builder.Metadata.GetProperties())
        {
            if (property.ClrType != typeof(string)) continue;

            property.SetMaxLength(150);
        }
    }
}