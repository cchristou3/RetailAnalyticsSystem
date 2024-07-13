using System;
using System.Threading.Tasks;
using InspireWebApp.SpaBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InspireWebApp.SpaBackend;

public static class Program
{
    public const string ProjectName = "InspireWebApp";

    public static async Task Main(string[] args)
    {
        var app = Bootstrapper.BuildApp(args);

        await ApplyMigrations(app);

        app.Run();
    }

    private static async Task ApplyMigrations(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            Console.WriteLine("Migrations begin...");
            await using var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync(); // migrate database whenever we restart the application
            Console.WriteLine("Migrations completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migrations failed: {ex}");
        }
    }
}