using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using InspireWebApp.SpaBackend.Clients;
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

    #region City Analysis

    [HttpGet("top-revenue-generating-cities")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetTopSellingCities()
    {
        _logger.Info("GET DashboardCharts.GetTopSellingCities");

        var topProducts = await _dbContext
            .TopSellingCitiesRecords
            .FromSql(@$"
                        ;WITH CTE_AGGREGATE_INVOICES_BY_CITY
                        AS
                        (
	                        SELECT 
		                        CityName = CI.[Name],
                                Value = SUM(ID.Quantity * ID.UnitPrice),
		                        Volume = SUM(ID.Quantity)
	                        FROM Invoices I
	                        INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
	                        INNER JOIN Customers C			ON I.CustomerId = C.Id
	                        INNER JOIN Cities CI			ON CI.Id = C.CityId
	                        GROUP BY CI.[Name]
                        )
                        SELECT TOP 10
	                        CTE.CityName,
                            CTE.Value,
	                        CTE.Volume
                        FROM CTE_AGGREGATE_INVOICES_BY_CITY CTE
                        ORDER BY CTE.Value DESC
                ")
            .ToArrayAsync();
        
        return DataVisualizationHelpers.WrapDefaultDataset(topProducts);
    }


    #endregion

    #region Customer Analytics

    [HttpGet("sales-by-customer-category")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByCustomerCategory()
    {
        _logger.Info("GET DashboardCharts.GetSalesByCustomerCategory");

        var topProducts = await _dbContext
            .SalesByCustomerCategoryRecords
            .FromSql(@$"
                        ;WITH CTE_AGGREGATE_INVOICES_BY_CUSTOMER_CATEGORY
                        AS
                        (
	                        SELECT 
		                        CustomerCategoryName = CI.[Name],
                                Value = SUM(ID.Quantity * ID.UnitPrice),
		                        Volume = SUM(ID.Quantity)
	                        FROM Invoices I
	                        INNER JOIN InvoiceDetail ID			ON I.Id = ID.InvoiceId
	                        INNER JOIN Customers C				ON I.CustomerId = C.Id
	                        INNER JOIN CustomerCategories CI	ON CI.Id = C.CustomerCategoryId
	                        GROUP BY CI.[Name]
                        )
                        SELECT
	                        CTE.CustomerCategoryName,
                            CTE.Value,
	                        CTE.Volume
                        FROM CTE_AGGREGATE_INVOICES_BY_CUSTOMER_CATEGORY CTE
                        ORDER BY CTE.Value DESC
                ")
            .ToArrayAsync();
        
        return DataVisualizationHelpers.WrapDefaultDataset(topProducts);
    }
    
    [HttpGet("customer-distribution-by-category")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetCustomerDistributionByCategory()
    {
        _logger.Info("GET DashboardCharts.GetSalesByCustomerCategory");

        var topProducts = await _dbContext
            .CustomerDistributionByCategoryRecords
            .FromSql(@$"
                    SELECT 
	                    CustomerCategoryName = C.[Name], 
	                    NumberOfCustomers = COUNT(1)
                    FROM CustomerCategories C
                    INNER JOIN Customers CU			ON C.Id = CU.CustomerCategoryId
                    GROUP BY C.Name
                    ORDER BY COUNT(1) DESC
                ")
            .ToArrayAsync();
        
        return DataVisualizationHelpers.WrapDefaultDataset(topProducts);
    }
    
    [HttpGet("customer-distribution-by-segment")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetCustomerDistributionBySegment()
    {
        _logger.Info("GET DashboardCharts.GetCustomerDistributionBySegment");

        var topProducts = await _dbContext
            .CustomerDistributionBySegmentRecords
            .FromSql(@$"
                    SELECT 
                            SegmentName = Segment, 
                            NumberOfCustomers = COUNT(1) 
                    FROM Customers
                    GROUP BY Segment
                    ORDER BY COUNT(1) DESC
                ")
            .ToArrayAsync();
        
        return DataVisualizationHelpers.WrapDefaultDataset(topProducts);
    }

    #endregion

    #region Product Analysis

    [HttpGet("top-revenue-generating-products")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetTopSellingProducts()
    {
        _logger.Info("GET DashboardCharts.GetTopSellingProducts");

        var topProducts = await _dbContext
            .TopSellingProducts
            .FromSql(@$"
                        ;WITH CTE_AGGREGATE_INVOICES_BY_PRODUCT
                        AS
                        (
	                        SELECT 
		                        ProductName = P.[Name],
                                Value = SUM(ID.Quantity * ID.UnitPrice),
		                        Volume = SUM(ID.Quantity)
	                        FROM Invoices I
	                        INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
	                        INNER JOIN Products P			ON ID.ProductId = P.Id
	                        GROUP BY P.[Name]
                        )
                        SELECT TOP 5
	                        CTE.ProductName,
                            CTE.Value,
	                        CTE.Volume
                        FROM CTE_AGGREGATE_INVOICES_BY_PRODUCT CTE
                        ORDER BY CTE.Value DESC
                ")
            .ToArrayAsync();
        
        return DataVisualizationHelpers.WrapDefaultDataset(topProducts);
    }
    
    [HttpGet("sales-by-product-pack-type")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByProductPackType()
    {
        _logger.Info("GET DashboardCharts.GetSalesByProductPackType");

        var productsPackTypes = await _dbContext
            .SalesByProductPackTypeRecords
            .FromSql(@$"
                ;WITH CTE_AGGREGATE_INVOICES_BY_PRODUCT_PACK_TYPE
                AS
                (
	                SELECT 
		                ProductPackTypeName = PPT.[Name],
		                Value = SUM(ID.Quantity * ID.UnitPrice),
                        Volume = SUM(ID.Quantity)
	                FROM Invoices I
	                INNER JOIN InvoiceDetail ID			ON I.Id = ID.InvoiceId
	                INNER JOIN Products P				ON ID.ProductId = P.Id
	                INNER JOIN ProductPackageTypes PPT	ON PPT.Id = P.PackageTypeId
	                GROUP BY PPT.[Name]
                )
                SELECT 
	                CTE.ProductPackTypeName,
	                CTE.Value,
                    CTE.Volume
                FROM CTE_AGGREGATE_INVOICES_BY_PRODUCT_PACK_TYPE CTE
                ORDER BY CTE.Value DESC
                ")
            .ToArrayAsync();
        
        return DataVisualizationHelpers.WrapDefaultDataset(productsPackTypes);
    }
    
    [HttpGet("sales-by-product-tag")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByProductTag()
    {
        _logger.Info("GET DashboardCharts.GetSalesByProductTag");

        var productsPackTypes = await _dbContext
            .SalesByProductTagsRecords
            .FromSql(@$"
                    ;WITH CTE_PRODUCT_TAGS
                    AS
                    (
	                    SELECT 
                            PT.ProductId, 
                            Tags = T.Name
	                    FROM ProductTags PT
	                    INNER JOIN Tags T				ON T.Id = PT.TagId
                    ),
                    CTE_AGGREGATE_INVOICES
                    AS
                    (
	                    SELECT 
		                    PT.Tags,
                            Value = SUM(ID.Quantity * ID.UnitPrice),
		                    Volume = SUM(ID.Quantity)
	                    FROM Invoices I
	                    INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
	                    INNER JOIN Products P			ON P.Id = ID.ProductId
	                    LEFT JOIN CTE_PRODUCT_TAGS PT		ON P.Id = PT.ProductId
	                    GROUP BY PT.Tags
                    )
                    SELECT
	                    ProductTagName = ISNULL(CTE.Tags, 'Untagged'),
                        CTE.[Value],
	                    CTE.Volume
                    FROM CTE_AGGREGATE_INVOICES CTE
                    ORDER BY ISNULL(CTE.Tags, 'Untagged')
                ")
            .ToArrayAsync();
        
        return DataVisualizationHelpers.WrapDefaultDataset(productsPackTypes);
    }

    #endregion

    #region Timely Sales
    
    [HttpGet("sales/forecasting")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesForecast()
    {
        _logger.Info("GET DashboardCharts.GetSalesForecast");
        var result = await new AnalyticsBackendClient().GetSalesForecasting();
        return DataVisualizationHelpers.WrapDefaultDataset(result);
    }
    
    [HttpGet("sales-by-hour")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByHour()
    {
        _logger.Info("GET DashboardCharts.GetSalesByHour");
        var result = await _dbContext.HourlySalesRecords
            .FromSql($@"
                SELECT
	                [Hour] = DATEPART(HOUR, I.[Date]),
	                [Value] = AVG(ID.Quantity * ID.UnitPrice), 
	                [Volume] = AVG(Quantity)
                FROM Invoices I
                INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
                GROUP BY DATEPART(HOUR, I.[Date])
                ORDER BY DATEPART(HOUR, I.[Date])
            ")
            .ToArrayAsync();

        return DataVisualizationHelpers.WrapDefaultDataset(result);
    }
    
    [HttpGet("sales-by-day")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByDay()
    {
        _logger.Info("GET DashboardCharts.GetSalesByDay");
        var result = await _dbContext
            .DaySalesRecords
            .FromSql($@"
                SELECT
	                [Day] = DATENAME(DW, I.[Date]),
	                [DayNo] = DATEPART(DW, I.[Date]),
	                Value = AVG(ID.Quantity * ID.UnitPrice), 
	                Volume = AVG(Quantity)
                FROM Invoices I
                INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
                GROUP BY DATENAME(DW, I.[Date]), DATEPART(DW, I.[Date])
                ORDER BY DATEPART(DW, I.[Date])
            ")
            .ToArrayAsync();

        return DataVisualizationHelpers.WrapDefaultDataset(result);
    }
    
    [HttpGet("sales-by-date")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByDate()
    {
        _logger.Info("GET DashboardCharts.GetSalesByDate");
        var result = await _dbContext.DailySalesRecords
            .FromSql($@"
                SELECT
	                I.[Date],
	                Value = AVG(ID.Quantity * ID.UnitPrice), 
	                Volume = AVG(Quantity)
                FROM Invoices I
                INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
                GROUP BY I.[Date]
                ORDER BY I.[Date]
            ")
            .ToArrayAsync();

        return DataVisualizationHelpers.WrapDefaultDataset(result);
    }
    
    [HttpGet("sales-by-month")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByMonth()
    {

        await Task.Delay(5000);
        _logger.Info("GET DashboardCharts.GetSalesByMonth");
        var result = await _dbContext.MonthlySalesRecords
            .FromSql($@"
                SELECT
	                [Month] = DATEPART(MONTH, I.[Date]),
	                [Value] = AVG(ID.Quantity * ID.UnitPrice), 
	                [Volume] = AVG(Quantity)
                FROM Invoices I
                INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
                GROUP BY DATEPART(MONTH, I.[Date])
                ORDER BY DATEPART(MONTH, I.[Date])
            ")
            .ToArrayAsync();

        return DataVisualizationHelpers.WrapDefaultDataset(result);
    }
    
    [HttpGet("sales-by-quarter")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByQuarter()
    {
        _logger.Info("GET DashboardCharts.GetSalesByQuarter");
        var result = await _dbContext.QuarterlySalesRecords
            .FromSql($@"
                SELECT
	                [Quarter] = DATEPART(QUARTER, I.[Date]),
	                [Value] = AVG(ID.Quantity * ID.UnitPrice), 
	                [Volume] = AVG(Quantity)
                FROM Invoices I
                INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
                WHERE YEAR(I.[Date]) < 2016
                GROUP BY DATEPART(QUARTER, I.[Date])
                ORDER BY DATEPART(QUARTER, I.[Date])
            ")
            .ToArrayAsync();

        return DataVisualizationHelpers.WrapDefaultDataset(result);
    }

    #endregion

    #region Other

    [HttpGet("sales-volume-by-outlet-type")]
    [ResponseCache(Duration = 5)]
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
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByPackTypeArea()
    {
        _logger.Info("GET DashboardCharts.GetSalesByPackTypeArea");
        return DataVisualizationHelpers.WrapDefaultDataset(
            new object()
        );
    }

    [HttpGet("sales-by-year")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByYear()
    {
        _logger.Info("GET DashboardCharts.GetSalesByYear");
        return DataVisualizationHelpers.WrapDefaultDataset(
            await _dbContext.YearlySalesRecords
                .FromSql($@"
                    SELECT
	                    [Year] = YEAR(I.[Date]),
	                    Value = SUM(ID.Quantity * ID.UnitPrice), 
	                    Volume = SUM(Quantity)
	                    /* ,AvgNumOfSKUs = COUNT(DISTINCT ID.ProductId) */
                    FROM Invoices I
                    INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
                    GROUP BY YEAR(I.[Date])
                    ORDER BY YEAR(I.[Date])
                ")
                .ToArrayAsync()
        );
    }

    [HttpGet("sales-by-year-for-boxplot")]
    [ResponseCache(Duration = 5)]
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

    [HttpGet("sales-by-district")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetSalesByArea()
    {
        _logger.Info("GET DashboardCharts.GetSalesByArea");
        return DataVisualizationHelpers.WrapDefaultDataset(
            new object()
        );
    }

    [HttpGet("dummy-target-completion")]
    [ResponseCache(Duration = 5)]
    public object GetDummyTargetCompletion()
    {
        return DataVisualizationHelpers.WrapDefaultDataset(new object[]
        {
            new { Series = "DEFAULT", Actual = 60, Target = 80 }
        });
    }

    [HttpGet("rules-support-confidence")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetRulesSupportConfidence()
    {
        _logger.Info("GET DashboardCharts.GetRulesSupportConfidence");


        var result = new[]
        {
            new AssociationRuleRecord
            {
                Confidence = 80,
                Lift = new decimal(1.5),
                Support = 70,
                Rule = "A => B",
                LeftHand = "A",
                RightHand = "B"
            },
            new AssociationRuleRecord
            {
                Confidence = 74,
                Lift = new decimal(6.5),
                Support = 85,
                Rule = "D => B",
                LeftHand = "D",
                RightHand = "B"
            }
        };
        
        // var result = await _dbContext.MinerAssocRules
        //     .ToArrayAsync();

        return DataVisualizationHelpers.WrapDefaultDataset(result);
    }

    [HttpGet("calendar-sales-hierarchical")]
    [ResponseCache(Duration = 5)]
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
    
    #endregion
}