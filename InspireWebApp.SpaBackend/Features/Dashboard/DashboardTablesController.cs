using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InspireWebApp.SpaBackend.Clients;
using InspireWebApp.SpaBackend.Data;
using InspireWebApp.SpaBackend.Data.ProductSales;
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
    public async Task<IList<AssociationRuleRecord>> AssociationRules()
    {
        _logger.Info("GET DashboardTables.AssociationRules");

        return await _dbContext.MinerAssocRules
	        .Select(x => new AssociationRuleRecord
	        {
		        Rule = x.LeftHand + " => " + x.RightHand,
		        LeftHand = x.LeftHand,
		        RightHand = x.RightHand,
		        Confidence = x.Confidence * 100,
		        Support = x.Support  * 100,
		        Lift = x.Lift,
		        Segment = x.Segment,
	        })
	        .ToListAsync();
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
    
    [HttpGet("top-profitable-products-per-product-pack-type")]
    public async Task<IList<TopProfitableProductsPerPackTypeRecord>> GetTopProfitableProductsPerProductPackType()
    {
        _logger.Info("GET DashboardTables.GetTopProfitableProductsPerProductPackType");
        
        return await _dbContext
            .TopProfitableProductsPerPackTypeRecords
            .FromSql(@$"
					;WITH CTE_AGGREGATE_INVOICES
					AS
					(
						SELECT 
							ProductName = P.[Name],
							P.PackageTypeId,
					        Value = SUM(ID.Quantity * ID.UnitPrice),
							Volume = SUM(ID.Quantity),
							[Rank] = ROW_NUMBER() OVER (PARTITION BY PackageTypeId ORDER BY SUM(ID.Quantity * ID.UnitPrice) DESC)
						FROM Invoices I
						INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
						INNER JOIN Products P			ON P.Id = ID.ProductId
						GROUP BY P.[Name], P.PackageTypeId
					)
					, CTE_TOTAL_SALES
					AS
					(
						SELECT TotalValue = SUM([Value]) FROM CTE_AGGREGATE_INVOICES
					)
					SELECT
						CTE.ProductName,
						ProductPackType = PPT.[Name],
					    CTE.[Value],
						Volume = CAST(CTE.Volume AS INT),
						Rank = CAST(CTE.Rank AS INT),
						Contribution = CONVERT(DECIMAL(9,2), (CTE.[Value] / (SELECT TotalValue FROM CTE_TOTAL_SALES)) * 100)
					FROM CTE_AGGREGATE_INVOICES CTE
					INNER JOIN ProductPackageTypes PPT	ON PPT.Id = CTE.PackageTypeId
					WHERE CTE.Rank <= 3
					ORDER BY PPT.[Name], CTE.Rank
                ")
            .ToArrayAsync();
    }
    
    [HttpGet("top-profitable-products-per-product-tag")]
    public async Task<IList<TopProfitableProductsPerTagsRecord>> GetTopProfitableProductsPerProductTag()
    {
	    _logger.Info("GET DashboardTables.GetTopProfitableProductsPerProductTag");
        
	    return await _dbContext
		    .TopProfitableProductsPerTagsRecords
		    .FromSql(@$"
					;WITH CTE_PRODUCT_TAGS
					AS
					(
						SELECT 
					        PT.ProductId, 
					        Tag = T.Name
						FROM ProductTags PT
						INNER JOIN Tags T				ON T.Id = PT.TagId
					),
					CTE_AGGREGATE_INVOICES
					AS
					(
						SELECT 
							ProductName = P.[Name],
							PT.Tag,
					        Value = SUM(ID.Quantity * ID.UnitPrice),
							Volume = SUM(ID.Quantity),
							[Rank] = ROW_NUMBER() OVER (PARTITION BY ISNULL(PT.Tag, 'Untagged') ORDER BY SUM(ID.Quantity * ID.UnitPrice) DESC)
						FROM Invoices I
						INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
						INNER JOIN Products P			ON P.Id = ID.ProductId
						LEFT JOIN CTE_PRODUCT_TAGS PT		ON P.Id = PT.ProductId
						GROUP BY P.[Name], PT.Tag
					)
					, CTE_TOTAL_SALES
					AS
					(
						SELECT TotalValue = SUM([Value]) FROM CTE_AGGREGATE_INVOICES
					)
					SELECT
						CTE.ProductName,
						ProductTag = ISNULL(CTE.Tag, 'Untagged'),
					    CTE.[Value],
						CTE.Volume,
						Rank = CAST(CTE.Rank AS INT),
						Contribution = CONVERT(DECIMAL(9,2), (CTE.[Value] / (SELECT TotalValue FROM CTE_TOTAL_SALES)) * 100)
					FROM CTE_AGGREGATE_INVOICES CTE
					WHERE CTE.Rank <= 3
					ORDER BY ISNULL(CTE.Tag, 'Untagged'), CTE.Rank
                ")
		    .ToArrayAsync();
    }
    
    [HttpGet("customer-distribution-by-city")]
    [ResponseCache(Duration = 5)]
    public async Task<object> GetCustomerDistributionByCity()
    {
	    _logger.Info("GET DashboardTables.GetCustomerDistributionByCity");

	    return await _dbContext
		    .CustomerDistributionByCityRecords
		    .FromSql(@$"
                        SELECT
	                    CityName = C.[Name], 
	                    NumberOfCustomers = COUNT(1)
                    FROM Cities C
                    INNER JOIN Customers CU	ON C.Id = CU.CityId
                    GROUP BY C.Name
                ")
		    .ToArrayAsync();
    }
    
    
    [HttpGet("segment-details")]
    [ResponseCache(Duration = 5)]
    public async Task<IReadOnlyList<AnalyticsBackendClient.CustomerSegment>> GetSegmentDetails()
    {
	    _logger.Info("GET DashboardTables.GetSegmentDetails");

	    return await new AnalyticsBackendClient().GetSegmentDetailsAsync();
    }
}