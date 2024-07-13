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

namespace InspireWebApp.SpaBackend.Features.ProductTags;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/promotion-types")]
[Authorize]
[AutoConstructor]
public partial class ProductTagsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    #region ListForReference

    [HttpGet("for-reference")]
    public async Task<IEnumerable<ProductTagReferenceModel>> ListForReference()
    {
        return await _dbContext.ProductTags
            .ProjectTo<ProductTagReferenceModel>(_mapper)
            .ToArrayAsync();
    }

    #endregion

    #region Delete

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var promotionType = await _dbContext.ProductTags
            .SingleOrDefaultAsync(p => p.Id == id);

        if (promotionType == null) return NotFound();

        _dbContext.ProductTags.Remove(promotionType);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    #endregion

    #region Create

    [JsonSchema(Name = "ProductTagCreateModel")]
    public class CreateModel
    {
        public required int ProductId { get; set; }
        
        public required int TagId { get; set; }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create(CreateModel model)
    {
        var promotionType = _mapper.Map<ProductTag>(model);

        _dbContext.ProductTags.Add(promotionType);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = promotionType.Id }, promotionType.Id);
    }

    #endregion

    #region List

    [HttpGet]
    public async Task<IEnumerable<ListModel>> List()
    {
        return await _dbContext.ProductTags
            .ProjectTo<ListModel>(_mapper)
            .ToArrayAsync();
    }

    [JsonSchema(Name = "ProductTagsListModel")]
    public class ListModel
    {
        public required int Id { get; set; }

        public required int ProductId { get; set; }
        
        public required int TagId { get; set; }
    }

    #endregion

    #region Get

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetailsModel>> Get(int id)
    {
        var model = await _dbContext.ProductTags
            .Where(p => p.Id == id)
            .ProjectTo<DetailsModel>(_mapper)
            .SingleOrDefaultAsync();

        if (model == null) return NotFound();

        return Ok(model);
    }

    [JsonSchema(Name = "ProductTagDetailsModel")]
    public class DetailsModel
    {
        public required int Id { get; set; }

        public required int ProductId { get; set; }
        
        public required int TagId { get; set; }
    }

    #endregion

    #region Update

    [HttpGet("{id:int}/for-update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateModel>> GetForUpdate(int id)
    {
        var model = await _dbContext.ProductTags
            .Where(p => p.Id == id)
            .ProjectTo<UpdateModel>(_mapper)
            .SingleOrDefaultAsync();

        if (model == null) return NotFound();

        return Ok(model);
    }

    [JsonSchema(Name = "ProductTagUpdateModel")]
    public class UpdateModel
    {
        public required int ProductId { get; set; }
        
        public required int TagId { get; set; }
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateModel>> Update(int id, UpdateModel model)
    {
        var promotionType = await _dbContext.ProductTags
            .SingleOrDefaultAsync(p => p.Id == id);

        if (promotionType == null) return NotFound();

        _mapper.Map(model, promotionType);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }

        return Ok(_mapper.Map<UpdateModel>(promotionType));
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
        // TODO-CCH
        return CheckUniqueCommon(currentId, p => "" == name);
    }

    private async Task<ActionResult> CheckUniqueCommon(
        int? currentId,
        Expression<Func<ProductTag, bool>> filter
    )
    {
        var query = _dbContext.ProductTags
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