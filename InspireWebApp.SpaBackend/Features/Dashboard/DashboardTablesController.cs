using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InspireWebApp.SpaBackend.Data;
using InspireWebApp.SpaBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NLog;

namespace InspireWebApp.SpaBackend.Features.Dashboard;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/dashboard-tables")]
[Authorize]
[AutoConstructor]
[ResponseCache(NoStore = true)]
public partial class DashboardTablesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    protected static ILogger _logger = LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();

    #region AssociationRules

    [HttpGet("association-rules")]
    public async Task<IList<AssociationRule>> AssociationRules()
    {
        _logger.Info("GET DashboardTables.AssociationRules");
        return await _dbContext.MinerAssocRules
            .Select(record => new AssociationRule
            {
                Rule = record.Rule,
                Confidence = record.Confidence,
                Support = record.Support,
                Lift = record.Lift
            })
            .ToArrayAsync();
    }

    [JsonSchema("DashboardTableAssociationRule")]
    public record AssociationRule
    {
        public required string Rule { get; init; }

        public required double Confidence { get; init; }
        public required double Support { get; init; }
        public required double Lift { get; init; }
    }

    #endregion
}