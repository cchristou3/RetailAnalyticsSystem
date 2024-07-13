using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using InspireWebApp.SpaBackend.Data;
using InspireWebApp.SpaBackend.Features.ProductTags;
using InspireWebApp.SpaBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NLog;

namespace InspireWebApp.SpaBackend.Features.Products;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/products")]
[Authorize]
[AutoConstructor]
public partial class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    protected static ILogger _logger = LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();

    #region ListForReference

    [HttpGet("for-reference")]
    public async Task<IEnumerable<ProductReferenceModel>> ListForReference()
    {
        _logger.Info("GET ListForReference");
            
        return await _dbContext.Products
            .ProjectTo<ProductReferenceModel>(_mapper)
            .ToArrayAsync();
    }

    #endregion

    #region Delete

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _dbContext.Products
            .SingleOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    #endregion

    #region Create

    [JsonSchema(Name = "ProductCreateModel")]
    public class CreateModel
    {
        [MaxLength(70)] [MinLength(3)] public required string Name { get; set; }

        public required IList<ProductTagIdentifier> ProductTags { get; set; }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create(CreateModel model)
    {
        var promotionTypesByIdentifier = await _dbContext.ProductTags
            .GetForIdentifiers(
                model.ProductTags,
                identifier => p => p.Id == identifier.Id
            );

        ControllerHelpers.ValidateManyToMany(
            ModelState, model, promotionTypesByIdentifier,
            m => m.ProductTags, identifier => identifier,
            i => m => m.ProductTags[i]
        );

        if (!ModelState.IsValid) return ValidationProblem();

        var product = _mapper.Map<Product>(model);

        product.ProductTags = model.ProductTags
            .Select(identifier => promotionTypesByIdentifier[identifier])
            .ToHashSet();

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = product.Id }, product.Id);
    }

    #endregion

    #region List

    [HttpGet]
    public async Task<IEnumerable<ListModel>> List()
    {
        return await _dbContext.Products
            .ProjectTo<ListModel>(_mapper)
            .AsSplitQuery()
            .ToArrayAsync();
    }

    [JsonSchema(Name = "ProductsListModel")]
    public class ListModel
    {
        public required int Id { get; set; }

        [MaxLength(70)] [MinLength(3)] public required string Name { get; set; }

        public required IList<ProductTagReferenceModel> ProductTags { get; set; }
    }

    #endregion

    #region Get

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetailsModel>> Get(int id)
    {
        var model = await _dbContext.Products
            .Where(p => p.Id == id)
            .ProjectTo<DetailsModel>(_mapper)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (model == null) return NotFound();

        return Ok(model);
    }

    [JsonSchema(Name = "ProductDetailsModel")]
    public class DetailsModel
    {
        public required int Id { get; set; }

        [MaxLength(70)] [MinLength(3)] public required string Name { get; set; }

        public required IList<ProductTagReferenceModel> ProductTags { get; set; }
    }

    #endregion

    #region Update

    [HttpGet("{id:int}/for-update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateModel>> GetForUpdate(int id)
    {
        var model = await _dbContext.Products
            .Where(p => p.Id == id)
            .ProjectTo<UpdateModel>(_mapper)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (model == null) return NotFound();

        return Ok(model);
    }

    [JsonSchema(Name = "ProductUpdateModel")]
    public class UpdateModel
    {

        [MaxLength(70)] [MinLength(3)] public required string Name { get; set; }

        public required IList<ProductTagIdentifier> ProductTags { get; set; }
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateModel>> Update(int id, UpdateModel model)
    {
        var product = await _dbContext.Products
            .Include(p => p.ProductTags)
            .AsSplitQuery()
            .SingleOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        var promotionTypesByIdentifier = await _dbContext.ProductTags
            .GetForIdentifiers(
                model.ProductTags,
                identifier => p => p.Id == identifier.Id
            );

        ControllerHelpers.ValidateManyToMany(
            ModelState, model, promotionTypesByIdentifier,
            m => m.ProductTags, identifier => identifier,
            i => m => m.ProductTags[i]
        );

        if (!ModelState.IsValid) return ValidationProblem();

        _mapper.Map(model, product);

        product.ProductTags = model.ProductTags
            .Select(identifier => promotionTypesByIdentifier[identifier])
            .ToHashSet();

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }

        return Ok(_mapper.Map<UpdateModel>(product));
    }

    #endregion

    #region CheckUnique

    [HttpPost("check-unique/name")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<ActionResult> CheckUniqueName(
        int? currentId, [BindRequired] string name
    )
    {
        return CheckUniqueCommon(currentId, p => p.Name == name);
    }

    private async Task<ActionResult> CheckUniqueCommon(
        int? currentId,
        Expression<Func<Product, bool>> filter
    )
    {
        var query = _dbContext.Products
            .Where(filter);

        if (currentId != null)
            query = query
                .Where(p => p.Id != currentId);

        return await query.AnyAsync()
            ? Conflict()
            : Ok();
    }

    #endregion
}