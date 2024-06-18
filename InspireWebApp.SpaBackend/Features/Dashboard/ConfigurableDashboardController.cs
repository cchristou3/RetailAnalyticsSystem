﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InspireWebApp.SpaBackend.Data;
using InspireWebApp.SpaBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;

namespace InspireWebApp.SpaBackend.Features.Dashboard;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/configurable-dashboards")]
[Authorize]
[AutoConstructor]
public partial class ConfigurableDashboardController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    #region Create

    [JsonSchema(Name = "ConfigurableDashboardCreateModel")]
    public class CreateModel
    {
        [MinLength(3)]
        [MaxLength(70)]
        public required string Name { get; set; }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create(CreateModel model)
    {
        Console.WriteLine("POST ConfigurableDashboard.Create");
        ConfigurableDashboard dashboard = _mapper.Map<ConfigurableDashboard>(model);

        _dbContext.ConfigurableDashboards.Add(dashboard);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = dashboard.Id }, dashboard.Id);
    }

    #endregion

    #region List

    [HttpGet]
    public async Task<IEnumerable<ListModel>> List()
    {
        Console.WriteLine("GET ConfigurableDashboard.List");
        return await _dbContext.ConfigurableDashboards
            .ProjectTo<ListModel>(_mapper)
            .ToArrayAsync();
    }

    [JsonSchema(Name = "ConfigurableDashboardListModel")]
    public class ListModel
    {
        public required int Id { get; set; }

        [MaxLength(70)]
        public required string Name { get; set; }
    }

    #endregion

    #region Get

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetailsModel>> Get(int id)
    {
        Console.WriteLine("GET ConfigurableDashboard.Get");
        DetailsModel? model = await _dbContext.ConfigurableDashboards
            .Where(a => a.Id == id)
            .ProjectTo<DetailsModel>(_mapper)
            .AsNoTrackingWithIdentityResolution()
            .SingleOrDefaultAsync();

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [JsonSchema(Name = "ConfigurableDashboardDetailsModel")]
    public class DetailsModel
    {
        public required int Id { get; set; }

        [MaxLength(70)]
        public required string Name { get; set; }

        public required IList<TileDetailsModel> Tiles { get; set; }
    }

    [JsonSchema("ConfigurableDashboardTileModel")]
    public class TileDetailsModel
    {
        public long Id { get; init; }

        public required int X { get; init; }
        public required int Y { get; init; }
        public required int Width { get; init; }
        public required int Height { get; init; }

        public required ConfDashboardTileType Type { get; init; }

        public required PredefinedVisualizationTileOptions? PredefinedVisualizationOptions { get; init; }
    }

    #endregion

    #region UpdateDashboardTiles

    /// <returns>A map of temporary IDs to permanent IDs (generated by the server).</returns>
    [HttpPatch("{id:int}/tiles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Dictionary<string, long>>> UpdateDashboardTiles(
        int id, IList<TileUpdateModel> newTiles
    )
    {
        Console.WriteLine("PATCH ConfigurableDashboard.UpdateDashboardTiles");
        // Cannot just use (Id, TempId) as TempId should be ignored when Id is set
        static (bool, long?, string?) GetIdentifierForValidation(TileUpdateModel tile)
            => tile.Id is not null ? (true, tile.Id, null) : (false, null, tile.TempId);

        ControllerHelpers.ValidateNoDuplicate(
            ModelState, newTiles,
            list => list,
            GetIdentifierForValidation,
            i => m => m[i]
        );

        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        ConfigurableDashboard? dashboard = await _dbContext.ConfigurableDashboards
            .Include(d => d.Tiles)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dashboard == null)
        {
            return NotFound();
        }

        CreatedTiles createdTiles = new();

        _mapper.Map(
            newTiles, dashboard.Tiles,
            options => options.Items[CreatedTilesMapKey] = createdTiles
        );
        await _dbContext.SaveChangesAsync();

        Dictionary<string, long> assignedTileIds = new();
        foreach ((TileUpdateModel dto, ConfigurableDashboardTile entity) in createdTiles)
        {
            assignedTileIds[dto.TempId!] = entity.Id;
        }

        return assignedTileIds;
    }

    internal static readonly string CreatedTilesMapKey =
        typeof(ConfigurableDashboardController).FullName + "::AssignedIds";

    internal class CreatedTiles : Dictionary<TileUpdateModel, ConfigurableDashboardTile>
    {
    }

    #endregion

    [JsonSchema("ConfigurableDashboardTileUpdateModel")]
    public class TileUpdateModel
    {
        /// <summary>
        /// Null = new tile.
        /// </summary>
        public required long? Id { get; init; }

        /// <summary>
        /// Must be set when (and only when) Id is null.
        /// Has no meaning outside of the API call which created the tile on the server.
        /// </summary>
        public required string? TempId { get; init; }

        public required int X { get; init; }
        public required int Y { get; init; }
        public required int Width { get; init; }
        public required int Height { get; init; }

        public required ConfDashboardTileType Type { get; init; }

        public required PredefinedVisualizationTileOptions? PredefinedVisualizationOptions { get; init; }
    }
}