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

namespace InspireWebApp.SpaBackend.Features.PromotionTypes;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/promotion-types")]
[Authorize]
[AutoConstructor]
public partial class PromotionTypesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    #region Create

    [JsonSchema(Name = "PromotionTypeCreateModel")]
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
        PromotionType promotionType = _mapper.Map<PromotionType>(model);

        _dbContext.PromotionTypes.Add(promotionType);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = promotionType.Id }, promotionType.Id);
    }

    #endregion

    #region List

    [HttpGet]
    public async Task<IEnumerable<ListModel>> List()
    {
        return await _dbContext.PromotionTypes
            .ProjectTo<ListModel>(_mapper)
            .ToArrayAsync();
    }

    [JsonSchema(Name = "PromotionTypesListModel")]
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
    public async Task<IEnumerable<PromotionTypeReferenceModel>> ListForReference()
    {
        return await _dbContext.PromotionTypes
            .ProjectTo<PromotionTypeReferenceModel>(_mapper)
            .ToArrayAsync();
    }

    #endregion

    #region Get

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetailsModel>> Get(int id)
    {
        DetailsModel? model = await _dbContext.PromotionTypes
            .Where(p => p.Id == id)
            .ProjectTo<DetailsModel>(_mapper)
            .SingleOrDefaultAsync();

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [JsonSchema(Name = "PromotionTypeDetailsModel")]
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
        UpdateModel? model = await _dbContext.PromotionTypes
            .Where(p => p.Id == id)
            .ProjectTo<UpdateModel>(_mapper)
            .SingleOrDefaultAsync();

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [JsonSchema(Name = "PromotionTypeUpdateModel")]
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
        PromotionType? promotionType = await _dbContext.PromotionTypes
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

    #region Delete

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        PromotionType? promotionType = await _dbContext.PromotionTypes
            .SingleOrDefaultAsync(p => p.Id == id);

        if (promotionType == null)
        {
            return NotFound();
        }

        _dbContext.PromotionTypes.Remove(promotionType);
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
        Expression<Func<PromotionType, bool>> filter
    )
    {
        IQueryable<PromotionType> query = _dbContext.PromotionTypes
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