using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InspireWebApp.SpaBackend.Features.Dashboard;

public class ConfigurableDashboard
{
    public int Id { get; init; }

    [MaxLength(50)] public required string Name { get; set; }

    public ICollection<ConfigurableDashboardTile>? Tiles { get; set; }
}