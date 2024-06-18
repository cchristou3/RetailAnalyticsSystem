using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using InspireWebApp.SpaBackend.Data;
using InspireWebApp.SpaBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;

namespace InspireWebApp.SpaBackend.Features.ProductCategories;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/product-categories")]
[Authorize]
[AutoConstructor]
public partial class ProductCategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    #region Create

    [JsonSchema(Name = "ProductCategoryCreateModel")]
    public class CreateModel
    {
        [MaxLength(70)]
        [MinLength(3)]
        public required string Name { get; set; }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create(CreateModel model)
    {
        ProductCategory productCategory = _mapper.Map<ProductCategory>(model);

        _dbContext.ProductCategories.Add(productCategory);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = productCategory.Id }, productCategory.Id);
    }

    #endregion

    #region List

    [HttpGet]
    public async Task<IEnumerable<ListModel>> List()
    {
        return await _dbContext.ProductCategories
            .ProjectTo<ListModel>(_mapper)
            .ToArrayAsync();
    }

    [JsonSchema(Name = "ProductCategoriesListModel")]
    public class ListModel
    {
        public required int Id { get; set; }

        [MaxLength(70)]
        [MinLength(3)]
        public required string Name { get; set; }
    }

    #endregion

    #region ListForReference

    [HttpGet("for-reference")]
    public async Task<IEnumerable<ProductCategoryReferenceModel>> ListForReference()
    {
        return await _dbContext.ProductCategories
            .ProjectTo<ProductCategoryReferenceModel>(_mapper)
            .ToArrayAsync();
    }

    #endregion

    #region Get

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetailsModel>> Get(int id)
    {
        DetailsModel? model = await _dbContext.ProductCategories
            .Where(p => p.Id == id)
            .ProjectTo<DetailsModel>(_mapper)
            .SingleOrDefaultAsync();

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [JsonSchema(Name = "ProductCategoryDetailsModel")]
    public class DetailsModel
    {
        public required int Id { get; set; }

        [MaxLength(70)]
        [MinLength(3)]
        public required string Name { get; set; }
    }

    #endregion

    #region Update

    [HttpGet("{id:int}/for-update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateModel>> GetForUpdate(int id)
    {
        UpdateModel? model = await _dbContext.ProductCategories
            .Where(p => p.Id == id)
            .ProjectTo<UpdateModel>(_mapper)
            .SingleOrDefaultAsync();

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [JsonSchema(Name = "ProductCategoryUpdateModel")]
    public class UpdateModel
    {
        [MaxLength(70)]
        [MinLength(3)]
        public required string Name { get; set; }
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateModel>> Update(int id, UpdateModel model)
    {
        ProductCategory? productCategory = await _dbContext.ProductCategories
            .SingleOrDefaultAsync(p => p.Id == id);

        if (productCategory == null) return NotFound();

        _mapper.Map(model, productCategory);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }

        return Ok(_mapper.Map<UpdateModel>(productCategory));
    }

    #endregion

    #region Delete

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        ProductCategory? productCategory = await _dbContext.ProductCategories
            .SingleOrDefaultAsync(p => p.Id == id);

        if (productCategory == null)
        {
            return NotFound();
        }

        _dbContext.ProductCategories.Remove(productCategory);
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
        Expression<Func<ProductCategory, bool>> filter
    )
    {
        IQueryable<ProductCategory> query = _dbContext.ProductCategories
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