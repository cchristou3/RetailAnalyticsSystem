using System;
using InspireWebApp.SpaBackend.Features.Dashboard;
using InspireWebApp.SpaBackend.Features.Identity;
using InspireWebApp.SpaBackend.Features.ProductCategories;
using InspireWebApp.SpaBackend.Features.Products;
using InspireWebApp.SpaBackend.Features.PromotionTypes;
using InspireWebApp.SpaBackend.Features.Suppliers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InspireWebApp.SpaBackend.Data;

public class ApplicationDbContext : IdentityUserContext<ApplicationUser, Guid>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
    public DbSet<PromotionType> PromotionTypes { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;

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