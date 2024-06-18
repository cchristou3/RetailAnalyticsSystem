using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace InspireWebApp.SpaBackend.Features.Suppliers;

public class Supplier
{
    public int Id { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    [MinLength(5)]
    public string? Info { get; set; }

    public required LocalDate ContractStartDate { get; set; }
    public LocalDate? ContractEndDate { get; set; }

    // public bool IsMarkedForContractTermination { get; set; }
}
