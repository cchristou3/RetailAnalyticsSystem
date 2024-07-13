using InspireWebApp.SpaBackend.Features.Cities;
using InspireWebApp.SpaBackend.Features.CustomerCategories;

namespace InspireWebApp.SpaBackend.Features.Customers;

public class Customer
{
    public int Id { get; set; }

    public required City City { get; set; }
    public int CityId { get; set; }

    public required CustomerCategory CustomerCategory { get; set; }
    public int CustomerCategoryId { get; set; }

    // No name is provided

    // [MaxLength(100)]
    // public required string Name { get; set; }
}