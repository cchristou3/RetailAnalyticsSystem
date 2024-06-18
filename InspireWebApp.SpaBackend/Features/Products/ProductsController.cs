using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using InspireWebApp.SpaBackend.Data;
using InspireWebApp.SpaBackend.Features.ProductCategories;
using InspireWebApp.SpaBackend.Features.PromotionTypes;
using InspireWebApp.SpaBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;

namespace InspireWebApp.SpaBackend.Features.Products;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/products")]
[Authorize]
[AutoConstructor]
public partial class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    #region Create

    [JsonSchema(Name = "ProductCreateModel")]
    public class CreateModel
    {
        public required int CategoryId { get; set; }

        [MaxLength(70)]
        [MinLength(3)]
        public required string Name { get; set; }

        public required decimal Price { get; set; }

        public required string? Description { get; set; }

        public required IList<PromotionTypeIdentifier> PromotionTypes { get; set; }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create(CreateModel model)
    {
        ProductCategory? category = await _dbContext.ProductCategories.FirstOrDefaultAsync(p => p.Id == model.CategoryId);
        if (category == null)
        {
            ModelState.AddModelError<CreateModel>(
                m => m.CategoryId,
                "Category not found"
            );
        }

        IDictionary<PromotionTypeIdentifier, PromotionType> promotionTypesByIdentifier = await _dbContext.PromotionTypes
            .GetForIdentifiers(
                model.PromotionTypes,
                identifier => p => p.Id == identifier.Id
            );

        ControllerHelpers.ValidateManyToMany(
            ModelState, model, promotionTypesByIdentifier,
            m => m.PromotionTypes, identifier => identifier,
            i => m => m.PromotionTypes[i]
        );

        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        Product product = _mapper.Map<Product>(model);

        product.PromotionTypes = model.PromotionTypes
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

        public required ProductCategoryReferenceModel Category { get; set; }

        [MaxLength(70)]
        [MinLength(3)]
        public required string Name { get; set; }

        public required decimal Price { get; set; }

        public required IList<PromotionTypeReferenceModel> PromotionTypes { get; set; }
    }

    #endregion

    #region ListForReference

    [HttpGet("for-reference")]
    public async Task<IEnumerable<ProductReferenceModel>> ListForReference()
    {
        return await _dbContext.Products
            .ProjectTo<ProductReferenceModel>(_mapper)
            .ToArrayAsync();
    }

    #endregion

    #region Get

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetailsModel>> Get(int id)
    {
        DetailsModel? model = await _dbContext.Products
            .Where(p => p.Id == id)
            .ProjectTo<DetailsModel>(_mapper)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [JsonSchema(Name = "ProductDetailsModel")]
    public class DetailsModel
    {
        public required int Id { get; set; }

        public required ProductCategoryReferenceModel Category { get; set; }

        [MaxLength(70)]
        [MinLength(3)]
        public required string Name { get; set; }

        public required decimal Price { get; set; }

        public required string? Description { get; set; }

        public required IList<PromotionTypeReferenceModel> PromotionTypes { get; set; }
    }

    #endregion

    #region Update

    [HttpGet("{id:int}/for-update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateModel>> GetForUpdate(int id)
    {
        UpdateModel? model = await _dbContext.Products
            .Where(p => p.Id == id)
            .ProjectTo<UpdateModel>(_mapper)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [JsonSchema(Name = "ProductUpdateModel")]
    public class UpdateModel
    {
        public required int CategoryId { get; set; }

        [MaxLength(70)]
        [MinLength(3)]
        public required string Name { get; set; }

        public required decimal Price { get; set; }

        public required string? Description { get; set; }

        public required IList<PromotionTypeIdentifier> PromotionTypes { get; set; }
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateModel>> Update(int id, UpdateModel model)
    {
        Product? product = await _dbContext.Products
            .Include(p => p.PromotionTypes)
            .AsSplitQuery()
            .SingleOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        ProductCategory? category = await _dbContext.ProductCategories.FirstOrDefaultAsync(p => p.Id == model.CategoryId);
        if (category == null)
        {
            ModelState.AddModelError<CreateModel>(
                m => m.CategoryId,
                "Category not found"
            );
        }

        IDictionary<PromotionTypeIdentifier, PromotionType> promotionTypesByIdentifier = await _dbContext.PromotionTypes
            .GetForIdentifiers(
                model.PromotionTypes,
                identifier => p => p.Id == identifier.Id
            );

        ControllerHelpers.ValidateManyToMany(
            ModelState, model, promotionTypesByIdentifier,
            m => m.PromotionTypes, identifier => identifier,
            i => m => m.PromotionTypes[i]
        );

        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        _mapper.Map(model, product);

        product.PromotionTypes = model.PromotionTypes
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

    #region Delete

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        Product? product = await _dbContext.Products
            .SingleOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();

        return Ok();
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
        IQueryable<Product> query = _dbContext.Products
            .Where(filter);

        if (currentId != null)
        {
            query = query
                .Where(p => p.Id != currentId);
        }

        return await query.AnyAsync()
            ? Conflict()
            : Ok();
    }

    #endregion
}