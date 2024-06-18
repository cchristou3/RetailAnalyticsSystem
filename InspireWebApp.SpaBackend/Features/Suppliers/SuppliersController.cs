using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using InspireWebApp.SpaBackend.Data;
using InspireWebApp.SpaBackend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NodaTime;

namespace InspireWebApp.SpaBackend.Features.Suppliers;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/suppliers")]
[AutoConstructor]
public partial class SuppliersController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DetailsModel>> Get(int id)
    {
        DetailsModel? model = await _dbContext.Suppliers
            .Select(s => new DetailsModel
            {
                Id = s.Id,
                Name = s.Name,
                Info = s.Info,
                ContractStartDate = s.ContractStartDate,
                ContractEndDate = s.ContractEndDate,
            })
            .SingleOrDefaultAsync(s => s.Id == id);

        if (model == null) return NotFound();
        return model;
        
        // http://localhost:4202/swagger
    }

    public class DetailsModel
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        [MinLength(5)]
        public string? Info { get; set; }

        public required LocalDate ContractStartDate { get; set; }
        public LocalDate? ContractEndDate { get; set; }
    }
    
    [JsonSchema("SupplierCreateModel")]
    public class CreateModel
    {
        [MaxLength(100)]
        public required string Name { get; set; }

        [MinLength(5)]
        public string? Info { get; set; }

        public required LocalDate ContractStartDate { get; set; }
        public LocalDate? ContractEndDate { get; set; }
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateModel model)
    {
        Supplier supplier = new Supplier()
        {
            Name = model.Name,
            Info = model.Info,
            ContractStartDate = model.ContractStartDate,
            ContractEndDate = model.ContractEndDate,
        };
        
        _dbContext.Suppliers.Add(supplier);
        await _dbContext.SaveChangesAsync();

        return supplier.Id;
    }
}