using System.ComponentModel.DataAnnotations;
using InspireWebApp.SpaBackend.Features.Cities;
using InspireWebApp.SpaBackend.Features.CustomerCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.Customers;

public class Customer
{
    public int Id { get; set; }

    public required City City { get; set; }
    public int CityId { get; set; }

    public required CustomerCategory CustomerCategory { get; set; }
    public int CustomerCategoryId { get; set; }
    
    public string? RfmScore { get; set; }
    
    public string? Segment { get; set; }

    // No name is provided

    // [MaxLength(100)]
    // public required string Name { get; set; }
}


public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(entity => entity.RfmScore)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsUnicode(false);
        
        builder.Property(entity => entity.Segment)
            .HasMaxLength(150)
            .IsUnicode(false);
    }
}