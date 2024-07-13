using System.ComponentModel.DataAnnotations;

namespace InspireWebApp.SpaBackend.Features.Products;

public class ProductReferenceModel
{
    public required int Id { get; set; }

    [MaxLength(70)] [MinLength(3)] public required string Name { get; set; }
}