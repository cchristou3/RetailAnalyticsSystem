using System;
using InspireWebApp.SpaBackend.Data.CityAnalysis;
using InspireWebApp.SpaBackend.Data.CustomerAnalysis;
using InspireWebApp.SpaBackend.Data.ProductSales;
using InspireWebApp.SpaBackend.Data.TimelySales;
using InspireWebApp.SpaBackend.Features.Cities;
using InspireWebApp.SpaBackend.Features.CustomerCategories;
using InspireWebApp.SpaBackend.Features.Customers;
using InspireWebApp.SpaBackend.Features.Dashboard;
using InspireWebApp.SpaBackend.Features.Employees;
using InspireWebApp.SpaBackend.Features.Identity;
using InspireWebApp.SpaBackend.Features.Invoices;
using InspireWebApp.SpaBackend.Features.ProductPackageTypes;
using InspireWebApp.SpaBackend.Features.Products;
using InspireWebApp.SpaBackend.Features.ProductTags;
using InspireWebApp.SpaBackend.Features.Tags;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InspireWebApp.SpaBackend.Data;

public class ApplicationDbContext : IdentityUserContext<ApplicationUser, Guid>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ProductTag> ProductTags { get; set; } = null!;
    
    public DbSet<Tag> Tags { get; set; } = null!;
    
    public DbSet<ProductPackageType> ProductPackageTypes { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<Customer> Customers { get; set; } = null!;
    
    public DbSet<Employee> Employees { get; set; } = null!;

    public DbSet<Invoice> Invoices { get; set; } = null!;

    public DbSet<City> Cities { get; set; } = null!;

    public DbSet<CustomerCategory> CustomerCategories { get; set; } = null!;


    public DbSet<ConfigurableDashboard> ConfigurableDashboards { get; set; } = null!;
    public DbSet<ConfigurableDashboardTile> ConfigurableDashboardTiles { get; set; } = null!;

    public DbSet<SalesRecord> Sales { get; set; } = null!;
    public required DbSet<AssociationRule> MinerAssocRules { get; set; }
    
    
    public required DbSet<CustomerDistributionByCityRecord> CustomerDistributionByCityRecords { get; set; }
    public required DbSet<TopSellingCitiesRecord> TopSellingCitiesRecords { get; set; }
    public required DbSet<SalesByCustomerCategoryRecord> SalesByCustomerCategoryRecords { get; set; }
    
    public required DbSet<CustomerDistributionByCategoryRecord> CustomerDistributionByCategoryRecords { get; set; }
    public required DbSet<CustomerDistributionBySegmentRecord> CustomerDistributionBySegmentRecords { get; set; }
    public required DbSet<TopSellingProductsRecord> TopSellingProducts { get; set; }
    public required DbSet<TopProfitableProductsPerPackTypeRecord> TopProfitableProductsPerPackTypeRecords { get; set; }
    public required DbSet<TopProfitableProductsPerTagsRecord> TopProfitableProductsPerTagsRecords { get; set; }
    
    public required DbSet<SalesByProductTagsRecord> SalesByProductTagsRecords { get; set; }
    public required DbSet<SalesByProductPackTypeRecord> SalesByProductPackTypeRecords { get; set; }
    
    public required DbSet<YearlySalesRecord> YearlySalesRecords { get; set; }
    
    public required DbSet<DaySalesRecord> DaySalesRecords { get; set; }
    public required DbSet<DailySalesRecord> DailySalesRecords { get; set; }
    public required DbSet<HourlySalesRecord> HourlySalesRecords { get; set; }
    public required DbSet<MonthlySalesRecord> MonthlySalesRecords { get; set; }
    
    public required DbSet<QuarterlySalesRecord> QuarterlySalesRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}