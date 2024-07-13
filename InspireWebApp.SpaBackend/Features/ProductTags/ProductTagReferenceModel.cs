using System.ComponentModel.DataAnnotations;

namespace InspireWebApp.SpaBackend.Features.ProductTags;

public class ProductTagReferenceModel
{
    public required int Id { get; set; }

    public required int ProductId { get; set; }
        
    public required int TagId { get; set; }
}