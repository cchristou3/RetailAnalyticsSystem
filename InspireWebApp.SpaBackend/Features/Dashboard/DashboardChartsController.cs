using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using InspireWebApp.SpaBackend.Data;
using InspireWebApp.SpaBackend.DataVisualization;
using InspireWebApp.SpaBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using NodaTime;

namespace InspireWebApp.SpaBackend.Features.Dashboard;

[ApiController]
[Route(RoutingHelpers.ApiRoutePrefix + "/dashboard-charts")]
[Authorize]
[AutoConstructor]
public partial class DashboardChartsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    protected static ILogger _logger = LogManager.LoadConfiguration("NLog.config").GetCurrentClassLogger();

    [HttpGet("sales-volume-by-outlet-type")]
    [ResponseCache(NoStore = true)]
    public async Task<object> GetSalesVolumeByOutletType()
    {
        _logger.Info("GET DashboardCharts.GetSalesVolumeByOutletType");
        return DataVisualizationHelpers.WrapDefaultDataset(
            await _dbContext.Sales
                .GroupBy(
                    record => record.ProductName,
                    (productName, records) => new
                    {
                        Outlet = productName,
                        Volume = records.Sum(record => record.SalesVolume)
                    }
                )
                // .OrderByDescending(arg => arg.SalesVolume)
                .Take(3)
                .ToArrayAsync()
        );
    }

    [HttpGet("sales-by-pack-type-area")]
    [ResponseCache(NoStore = true)]
    public async Task<object> GetSalesByPackTypeArea()
    {
        _logger.Info("GET DashboardCharts.GetSalesByPackTypeArea");
        return DataVisualizationHelpers.WrapDefaultDataset(
            new object()
        );
    }

    [HttpGet("sales-by-year")]
    [ResponseCache(NoStore = true)]
    public async Task<object> GetSalesByYear()
    {
        _logger.Info("GET DashboardCharts.GetSalesByYear");
        return DataVisualizationHelpers.WrapDefaultDataset(
            await _dbContext.Sales
                .GroupBy(
                    record => record.Year,
                    (year, records) => new
                    {
                        year,
                        Volume = records.Sum(record => record.SalesVolume),
                        Value = records.Sum(record => record.SalesValue ?? 0)
                    }
                )
                .OrderBy(arg => arg.year)
                .ToArrayAsync()
        );
    }

    [HttpGet("sales-by-year-for-boxplot")]
    [ResponseCache(NoStore = true)]
    public async Task<object> GetSalesByYearForBoxplot()
    {
        _logger.Info("GET DashboardCharts.GetSalesByYearForBoxplot");
        // Loosely based on
        // https://www.seancarney.ca/2021/01/31/calculating-medians-and-quartiles-across-groups-in-sql/
        // and https://dataschool.com/how-to-teach-people-sql/how-to-find-outliers-with-sql/
        // TODO: some of these inner joins should probably become left
        const string shared = """
                              WITH Quartilized AS (
                                  SELECT
                                      [YEAR],
                                      M_SALES_VALUE,
                                      M_SALES_VOLUME, -- Does not participate in quartiles
                                      NTILE(4) OVER (PARTITION BY [YEAR] ORDER BY M_SALES_VALUE) AS Quartile
                                  FROM Sales
                                  WHERE M_SALES_VALUE IS NOT NULL
                              ), Quartilies AS (
                                  SELECT
                                      [YEAR],
                                      MAX(CASE WHEN Quartile = 1 THEN M_SALES_VALUE END) [Q1],
                                      MAX(CASE WHEN Quartile = 2 THEN M_SALES_VALUE END) [Median],
                                      MAX(CASE WHEN Quartile = 3 THEN M_SALES_VALUE END) [Q3],
                                      SUM(M_SALES_VOLUME) [TotalVolume],
                                      COUNT(1) AS [Count]
                                  FROM Quartilized
                                  GROUP BY YEAR
                              ), Thresholds AS (
                                  SELECT
                                      YEAR,
                                      Q1 - (Q3 - Q1) * 1.5 AS Lower,
                                      Q3 + (Q3 - Q1) * 1.5 AS Upper
                                  FROM Quartilies
                              ), CappedMinMax AS (
                                  SELECT
                                      s.YEAR,
                                      MIN(s.M_SALES_VALUE) AS [Min],
                                      MAX(s.M_SALES_VALUE) AS [Max]
                                  FROM Sales s
                                      INNER JOIN Thresholds t ON t.YEAR = s.YEAR
                                  WHERE
                                      M_SALES_VALUE IS NOT NULL AND
                                      s.M_SALES_VALUE >= t.Lower AND
                                      s.M_SALES_VALUE <= t.Upper
                                  GROUP BY s.YEAR
                              )

                              """;

        var main = await _dbContext.Set<SalesByYearBoxplotData>()
            .FromSqlRaw(shared +
                        """
                        SELECT
                            qs.[YEAR],
                            Q1, Median, Q3, [Count],
                            [TotalVolume],
                            cmm.Min, cmm.Max
                        FROM Quartilies qs
                            INNER JOIN Thresholds t ON t.YEAR = qs.YEAR
                            INNER JOIN CappedMinMax cmm ON cmm.YEAR = qs.YEAR
                        ORDER BY YEAR
                        """
            )
            .ToArrayAsync();

        var outliers = await _dbContext.Set<SalesByYearOutlierData>()
            .FromSqlRaw(shared +
                        """
                        SELECT DISTINCT s.YEAR, s.M_SALES_VALUE AS [Value]
                        FROM Sales s
                            INNER JOIN Thresholds t ON t.YEAR = s.YEAR
                        WHERE s.M_SALES_VALUE < t.Lower OR s.M_SALES_VALUE > t.Upper
                        ORDER BY s.YEAR, [Value]
                        """
            )
            .ToArrayAsync();

        return new Dictionary<string, object>
        {
            [DataVisualizationConstants.DefaultDatasetKey] = main,
            ["outliers"] = outliers
        };
    }

    [HttpGet("sales-by-date")]
    [ResponseCache(NoStore = true)]
    public async Task<object> GetSalesByDate()
    {
        _logger.Info("GET DashboardCharts.GetSalesByDate");
        var result = await _dbContext.Sales
            .GroupBy(
                record => new LocalDate(record.Year, record.Month, record.Day),
                (date, records) => new
                {
                    date,
                    Volume = records.Sum(record => record.SalesVolume),
                    Value = records.Sum(record => record.SalesValue ?? 0)
                }
            )
            .ToArrayAsync();

        return DataVisualizationHelpers.WrapDefaultDataset(result.OrderBy(tuple => tuple.date).ToArray());
    }

    [HttpGet("sales-by-district")]
    [ResponseCache(NoStore = true)]
    public async Task<object> GetSalesByArea()
    {
        _logger.Info("GET DashboardCharts.GetSalesByArea");
        return DataVisualizationHelpers.WrapDefaultDataset(
            new object()
        );
    }

    [HttpGet("dummy-target-completion")]
    [ResponseCache(NoStore = true)]
    public object GetDummyTargetCompletion()
    {
        return DataVisualizationHelpers.WrapDefaultDataset(new object[]
        {
            new { Series = "DEFAULT", Actual = 60, Target = 80 }
        });
    }

    [HttpGet("rules-support-confidence")]
    [ResponseCache(NoStore = true)]
    public async Task<object> GetRulesSupportConfidence()
    {
        _logger.Info("GET DashboardCharts.GetRulesSupportConfidence");
        var result = await _dbContext.MinerAssocRules
            .ToArrayAsync();

        return DataVisualizationHelpers.WrapDefaultDataset(result);
    }

    [HttpGet("calendar-sales-hierarchical")]
    [ResponseCache(NoStore = true)]
    public async Task<object> GetCalendarSalesHierarchical()
    {
        _logger.Info("GET DashboardCharts.GetCalendarSalesHierarchical");
        var result = await _dbContext.Sales
            .GroupBy(
                record => new { record.Year, record.Quarter, record.Month },
                (group, records) => new
                {
                    group.Year,
                    group.Quarter,
                    group.Month,
                    Volume = records.Sum(record => record.SalesVolume),
                    Value = records.Sum(record => record.SalesValue ?? 0)
                }
            )
            .ToArrayAsync();

        // This is not pretty and I'm not proud, but it's probably getting reworked soon anyway
        var result2 = result
            .Select(row => new Dictionary<string, object>
            {
                ["Year"] = row.Year,
                ["Quarter"] = row.Quarter,
                ["Month"] = row.Month,
                ["Volume"] = row.Volume,
                ["Value"] = row.Value
            })
            .ToArray();

        return DataVisualizationHelpers.WrapDefaultDataset(
            HierarchicalData.TabularToHierarchical<Dictionary<string, object>, Dictionary<string, object>>(
                result2,
                (level, levelValue, rows, childNodes) => new Dictionary<string, object>
                {
                    ["category"] = level != 2
                        ? levelValue
                        : CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName((int)levelValue),

                    ["volume"] = rows.Sum(row => (double)row["Volume"]),
                    ["value"] = rows.Sum(row => (double)row["Value"]),

                    ["children"] = childNodes
                },
                true,
                row => row["Year"],
                row => row["Quarter"],
                row => row["Month"]
            )
        );
    }
}