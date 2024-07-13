using System.ComponentModel.DataAnnotations;

namespace InspireWebApp.SpaBackend.Features.CustomerCategories;

public class CustomerCategory
{
    public int Id { get; set; }

    [MaxLength(100)] public required string Name { get; set; }
}