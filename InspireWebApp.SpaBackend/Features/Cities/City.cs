using System.ComponentModel.DataAnnotations;

namespace InspireWebApp.SpaBackend.Features.Cities;

public class City
{
    public int Id { get; set; }

    [MaxLength(100)] public required string Name { get; set; }
}