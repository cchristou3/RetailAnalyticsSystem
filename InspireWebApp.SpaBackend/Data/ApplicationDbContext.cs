using System;
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
    public required DbSet<AssociationRuleRecord> MinerAssocRules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}