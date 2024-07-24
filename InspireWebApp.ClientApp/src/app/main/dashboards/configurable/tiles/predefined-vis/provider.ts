import {
  DashboardTablesClient,
  IPredefinedVisualizationTileOptions,
  PredefinedVisualizationType
} from "../../../../../@core/app-api";
import {changeableFromConstValue} from "../../../../../@shared/utils/changeable";
import {
  CommonVizDataDescriptor,
  CommonVizDatasetType,
  DEFAULT_DATASET
} from "../../../../../@shared/charts/common-viz-data";
import {DynamicComponentBlueprint} from "../../../../../@shared/dynamic-component/dynamic-component.blueprint";
import {
  CONFIGURE_FN_INPUT,
  DESCRIPTOR_INPUT,
  PredefinedChartComponentInputs
} from "../../../@infrastructure/predefined-chart-tile/predefined-chart.inputs";
import {CommonChartConfigureFn} from "../../../../../@shared/charts/common-chart.structures";
import {LoadableValueSource} from "../../../../../@shared/loadables/loadable";
import {TileManageHeaderInputs} from "../../../@infrastructure/shared";
import {Injectable, Type} from "@angular/core";
import {DashboardChartsService} from "../../../../../@core/dashboard/dashboard-charts.service";
import {
  CHART_TYPE_INPUT,
  PredefinedVisManageHeaderComponent,
  PredefinedVisManageHeaderInputs
} from "./predefined-vis-manage-header.component";
import {TileBodySpec} from "../tile-body-spec-provider";
import {
  SimpleTableComponentInputs,
  TABLE_BLUEPRINT_INPUT
} from "../../../@infrastructure/simple-table-tile/simple-table.inputs";
import {type AbstractTableContentComponent} from "./abstract-table-content.component";

@Injectable({
  providedIn: 'root',
})
export class PredefinedVisSpecProvider {
  constructor(
    private readonly chartsService: DashboardChartsService,
    private readonly tablesClient: DashboardTablesClient,
  ) {
  }

  // TODO: better args (+ name?)
  public provide(options: Readonly<IPredefinedVisualizationTileOptions>): TileBodySpec {
    switch (options.type) {

      //* City Analytics *//

      case PredefinedVisualizationType.CustomerDistributionByCity:
        return this.setupTableTileSpec(
          () => import('./content/city-analysis/customer-distribution-by-city/customer-distribution-by-city'),
          () => this.tablesClient.getCustomerDistributionByCity(),
          PredefinedVisualizationType.BasicPie
        );

      case PredefinedVisualizationType.TopRevenueGeneratingCities:
        return this.setupChartTileSpec(
          () => import('./content/city-analysis/top-revenue-generating-cities/basic-bar'),
          () => this.chartsService.getCommonVizData('top-revenue-generating-cities'),
          PredefinedVisualizationType.BasicPie
        );

      //* Customer Analytics *//

      case PredefinedVisualizationType.CustomerDistributionBySegment:
        return this.setupChartTileSpec(
          () => import('./content/customer-analytics/customer-distribution-by-customer-segment/basic-pie'),
          () => this.chartsService.getCommonVizData('customer-distribution-by-segment'),
          PredefinedVisualizationType.BasicPie
        );

      case PredefinedVisualizationType.CustomerDistributionByCategory:
        return this.setupChartTileSpec(
          () => import('./content/customer-analytics/customer-distribution-by-customer-category/basic-pie'),
          () => this.chartsService.getCommonVizData('customer-distribution-by-category'),
          PredefinedVisualizationType.BasicPie
        );

      case PredefinedVisualizationType.SalesByCustomerCategory:
        return this.setupChartTileSpec(
          () => import('./content/customer-analytics/sales-by-customer-category/basic-pie'),
          () => this.chartsService.getCommonVizData('sales-by-customer-category'),
          PredefinedVisualizationType.BasicPie
        );

      case PredefinedVisualizationType.SegmentDetails:
        return this.setupTableTileSpec(
          () => import('./content/customer-analytics/segment-details/segment-details'),
          () => this.tablesClient.getSegmentDetails(),
          PredefinedVisualizationType.BasicTimeLine,
        );

      //** Behavioural Analytics **//
      case PredefinedVisualizationType.SalesForecasting:
        return this.setupChartTileSpec(
          () => import('./content/behavioural-analytics/sales-forecasting/basic-line'),
          () => this.chartsService.getCommonVizData('sales/forecasting'),
          PredefinedVisualizationType.BasicLine
        );

      case PredefinedVisualizationType.SalesByHour:
        return this.setupChartTileSpec(
          () => import('./content/behavioural-analytics/sales-by-hour/basic-line'),
          () => this.chartsService.getCommonVizData('sales-by-hour'),
          PredefinedVisualizationType.BasicLine
        );

      case PredefinedVisualizationType.SalesByDay:
        return this.setupChartTileSpec(
          () => import('./content/behavioural-analytics/sales-by-day/basic-line'),
          () => this.chartsService.getCommonVizData('sales-by-day'),
          PredefinedVisualizationType.BasicTimeLine,
        );

      case PredefinedVisualizationType.SalesByDate:
        return this.setupChartTileSpec(
          () => import('./content/behavioural-analytics/sales-by-date/basic-time-line'),
          () => this.chartsService.getCommonVizData('sales-by-date'),
          PredefinedVisualizationType.BasicTimeLine,
        );

      case PredefinedVisualizationType.SalesByMonth:
        return this.setupChartTileSpec(
          () => import('./content/behavioural-analytics/sales-by-month/basic-line'),
          () => this.chartsService.getCommonVizData('sales-by-month'),
          PredefinedVisualizationType.BasicLine
        );

      case PredefinedVisualizationType.SalesByQuarter:
        return this.setupChartTileSpec(
          () => import('./content/behavioural-analytics/sales-by-quarter/basic-line'),
          () => this.chartsService.getCommonVizData('sales-by-quarter'),
          PredefinedVisualizationType.BasicLine
        );

      case PredefinedVisualizationType.SalesByYear:
        return this.setupChartTileSpec(
          () => import('./content/behavioural-analytics/sales-by-year/basic-line'),
          () => this.chartsService.getCommonVizData('sales-by-year'),
          PredefinedVisualizationType.BasicLine
        );

      //** Product Analytics **//

      case PredefinedVisualizationType.TopRevenueGeneratingProducts:
        return this.setupChartTileSpec(
          () => import('./content/product-analytics/top-revenue-generating-products/basic-bar'),
          () => this.chartsService.getCommonVizData('top-revenue-generating-products'),
          PredefinedVisualizationType.BasicBar
        );

      case PredefinedVisualizationType.SalesByProductPackType:
        return this.setupChartTileSpec(
          () => import('./content/product-analytics/sales-by-product-pack-type/basic-pie'),
          () => this.chartsService.getCommonVizData('sales-by-product-pack-type'),
          PredefinedVisualizationType.BasicPie
        );

      case PredefinedVisualizationType.TopProfitableProductsPerPackType:
        return this.setupTableTileSpec(
          () => import('././content/product-analytics/top-products-per-pack-type/top-products-per-pack-type'),
          () => this.tablesClient.getTopProfitableProductsByProductPackType(),
          options.type
        );

      case PredefinedVisualizationType.SalesByProductTag:
        return this.setupChartTileSpec(
          () => import('./content/product-analytics/sales-by-product-tag/basic-pie'),
          () => this.chartsService.getCommonVizData('sales-by-product-tag'),
          PredefinedVisualizationType.BasicPie
        );

      case PredefinedVisualizationType.TopProfitableProductsPerTag:
        return this.setupTableTileSpec(
          () => import('././content/product-analytics/top-products-per-tag/top-products-per-tag'),
          () => this.tablesClient.getTopProfitableProductsByProductTag(),
          options.type
        );

      case PredefinedVisualizationType.BasicPie:
        return this.setupChartTileSpec(
          () => import('./content/basic-pie'),
          () => this.chartsService.getCommonVizData('sales-volume-by-outlet-type'),
          options.type
        );

      case PredefinedVisualizationType.BasicDonut:
        return this.setupChartTileSpec(
          () => import('./content/basic-donut'),
          () => this.chartsService.getCommonVizData('sales-volume-by-outlet-type'),
          options.type
        );

      case PredefinedVisualizationType.BasicBar:
        return this.setupChartTileSpec(
          () => import('./content/basic-bar'),
          () => this.chartsService.getCommonVizData('sales-volume-by-outlet-type'),
          options.type
        );

      case PredefinedVisualizationType.HorizontalBar:
        return this.setupChartTileSpec(
          () => import('./content/horizontal-bar'),
          () => this.chartsService.getCommonVizData('sales-volume-by-outlet-type'),
          options.type
        );

      case PredefinedVisualizationType.BasicHeatMap:
        return this.setupChartTileSpec(
          () => import('./content/basic-heat-map'),
          () => this.chartsService.getCommonVizData('sales-by-pack-type-area'),
          options.type
        );

      case PredefinedVisualizationType.SemiPie:
        return this.setupChartTileSpec(
          () => import('./content/semi-pie'),
          () => this.chartsService.getCommonVizData('sales-volume-by-outlet-type'),
          options.type
        );

      case PredefinedVisualizationType.BasicRadar:
        return this.setupChartTileSpec(
          () => import('./content/basic-radar'),
          () => this.chartsService.getCommonVizData('sales-volume-by-outlet-type'),
          options.type
        );

      case PredefinedVisualizationType.BasicLine:
        return this.setupChartTileSpec(
          () => import('./content/basic-line'),
          () => this.chartsService.getCommonVizData('sales-by-year'),
          options.type
        );

      case PredefinedVisualizationType.DualLine:
        return this.setupChartTileSpec(
          () => import('./content/dual-line'),
          () => this.chartsService.getCommonVizData('sales-by-year'),
          options.type
        );

      case PredefinedVisualizationType.NestedDonut:
        return this.setupChartTileSpec(
          () => import('./content/nested-donut'),
          () => this.chartsService.getCommonVizData('sales-by-year'),
          options.type
        );

      case PredefinedVisualizationType.MultiBar:
        return this.setupChartTileSpec(
          () => import('./content/multi-bar'),
          () => this.chartsService.getCommonVizData('sales-by-year'),
          options.type
        );

      case PredefinedVisualizationType.LayeredBar:
        return this.setupChartTileSpec(
          () => import('./content/layered-bar'),
          () => this.chartsService.getCommonVizData('sales-by-year'),
          options.type
        );

      case PredefinedVisualizationType.BasicBoxplot:
        return this.setupChartTileSpec(
          () => import('./content/basic-boxplot'),
          () => this.chartsService.getCommonVizData('sales-by-year-for-boxplot'),
          options.type
        );

      case PredefinedVisualizationType.BasicTimeLine:
        return this.setupChartTileSpec(
          () => import('./content/basic-time-line'),
          () => this.chartsService.getCommonVizData('sales-by-date'),
          options.type
        );

      case PredefinedVisualizationType.BasicCyprusMap:
        return this.setupChartTileSpec(
          () => import('./content/basic-cyprus-map'),
          () => this.chartsService.getCommonVizData('sales-by-district'),
          options.type
        );

      case PredefinedVisualizationType.BasicBullet:
        return this.setupChartTileSpec(
          () => import('./content/basic-bullet'),
          () => this.chartsService.getCommonVizData('dummy-target-completion'),
          options.type
        );

      case PredefinedVisualizationType.BasicPareto:
        return this.setupChartTileSpec(
          () => import('./content/basic-pareto'),
          () => this.chartsService.getCommonVizData('sales-volume-by-outlet-type'),
          options.type
        );

      case PredefinedVisualizationType.StackedBar:
        return this.setupChartTileSpec(
          () => import('./content/stacked-bar'),
          () => this.chartsService.getCommonVizData('sales-by-year'),
          options.type
        );

      case PredefinedVisualizationType.ComboLineBar:
        return this.setupChartTileSpec(
          () => import('./content/combo-line-bar'),
          () => this.chartsService.getCommonVizData('sales-by-year'),
          options.type
        );

      case PredefinedVisualizationType.ComboLineBarRotated:
        return this.setupChartTileSpec(
          () => import('./content/combo-line-bar-rotated'),
          () => this.chartsService.getCommonVizData('sales-by-year'),
          options.type
        );

      case PredefinedVisualizationType.HorizontalBoxplot:
        return this.setupChartTileSpec(
          () => import('./content/horizontal-boxplot'),
          () => this.chartsService.getCommonVizData('sales-by-year-for-boxplot'),
          options.type
        );

      case PredefinedVisualizationType.ComboLineBox:
        return this.setupChartTileSpec(
          () => import('./content/combo-line-box'),
          () => this.chartsService.getCommonVizData('sales-by-year-for-boxplot'),
          options.type
        );

      case PredefinedVisualizationType.BasicSunburstChart:
        return this.setupChartTileSpec(
          () => import('./content/basic-sunburst-chart'),
          () => this.chartsService.getCommonVizData('calendar-sales-hierarchical'),
          options.type
        );

      case PredefinedVisualizationType.BasicTreeMap:
        return this.setupChartTileSpec(
          () => import('./content/basic-tree-map'),
          () => this.chartsService.getCommonVizData('calendar-sales-hierarchical'),
          options.type
        );

      case PredefinedVisualizationType.BasicPartitionChart:
        return this.setupChartTileSpec(
          () => import('./content/basic-partition-chart'),
          () => this.chartsService.getCommonVizData('calendar-sales-hierarchical'),
          options.type
        );

      case PredefinedVisualizationType.RulesSupportConfidence:
        return this.setupChartTileSpec(
          () => import('./content/rules-confidence'),
          () => this.chartsService.getCommonVizData('rules-support-confidence'),
          options.type
        );

      case PredefinedVisualizationType.RulesSupportConfidenceLift:
        return this.setupChartTileSpec(
          () => import('./content/rules-confidence-lift'),
          () => this.chartsService.getCommonVizData('rules-support-confidence'),
          options.type
        );

      case PredefinedVisualizationType.RulesMatrix:
        return this.setupChartTileSpec(
          () => import('./content/rules-matrix'),
          () => this.chartsService.getCommonVizData('rules-support-confidence'),
          options.type
        );

      case PredefinedVisualizationType.AssociationRules:
        return this.setupTableTileSpec(
          () => import('./content/association-rules'),
          () => this.tablesClient.associationRules(),
          options.type
        );

      case PredefinedVisualizationType.PriceHistory:
        return this.setupChartTileSpec(
          () => import('./content/price-history'),
          () => import('./content/price-history').then(m => m.data),
          options.type
        );

      case PredefinedVisualizationType.FuelPie:
        return this.setupChartTileSpec(
          () => import('./content/fuel-pie'),
          () => import('./content/fuel-pie').then(m => m.data),
          options.type
        );

      case PredefinedVisualizationType.FuelRadar:
        return this.setupChartTileSpec(
          () => import('./content/fuel-radar'),
          () => import('./content/fuel-radar').then(m => m.data),
          options.type
        );
    }

    throw 'PredefinedVisSpecProvider - unreachable code';

  }

  private setupChartTileSpec(
    importFactory: () => Promise<CommonChartSource>,
    dataSource: LoadableValueSource<unknown>,
    type: PredefinedVisualizationType
  ): TileBodySpec {
    return {
      component: prepareCommonChartSource(importFactory),

      ...this.setupSharedSpecOptions(dataSource, type),
    };
  }

  private setupTableTileSpec<TContent>(
    importFactory: () => Promise<SimpleTableSource<TContent>>,
    dataSource: LoadableValueSource<TContent>,
    type: PredefinedVisualizationType
  ): TileBodySpec {
    return {
      component: prepareSimpleTableSource(importFactory),

      ...this.setupSharedSpecOptions(dataSource, type),
    };
  }

  private setupSharedSpecOptions(
    dataSource: LoadableValueSource<unknown>,
    type: PredefinedVisualizationType
  ): Pick<TileBodySpec, 'data' | 'manageHeaderDescription'> {
    return {
      data: changeableFromConstValue(dataSource),

      manageHeaderDescription: {
        componentType: PredefinedVisManageHeaderComponent,
        initSetInputs: {
          [CHART_TYPE_INPUT]: type,
        } satisfies PredefinedVisManageHeaderInputs as Partial<TileManageHeaderInputs>,
      },
    };
  }
}

type CommonChartSource = {
  configureFn: CommonChartConfigureFn,
  dataDescriptor: CommonVizDataDescriptor,
};

type SimpleTableSource<TContent> = {
  default: Type<AbstractTableContentComponent<TContent>>,
};

function prepareCommonChartSource(
  importFactory: () => Promise<CommonChartSource>,
): LoadableValueSource<DynamicComponentBlueprint<any, PredefinedChartComponentInputs>> {
  return async () => {
    // Start loading at the same time
    const chartImportPromise = importFactory();

    const componentImport = await import('../../../@infrastructure/predefined-chart-tile/predefined-chart.component');
    const chartImport = await chartImportPromise;

    return {
      componentType: componentImport.PredefinedChartComponent,
      initSetInputs: {
        [CONFIGURE_FN_INPUT]: chartImport.configureFn,
        [DESCRIPTOR_INPUT]: chartImport.dataDescriptor,
      },
    };
  };
}


function prepareSimpleTableSource(
  importFactory: () => Promise<SimpleTableSource<unknown>>,
): LoadableValueSource<DynamicComponentBlueprint<any, SimpleTableComponentInputs>> {
  return async () => {
    // Start loading at the same time
    const tableImportPromise = importFactory();

    const wrapperImport = await import('../../../@infrastructure/simple-table-tile/simple-table.component');
    const table = await tableImportPromise;

    return {
      componentType: wrapperImport.SimpleTableComponent,
      initSetInputs: {
        [TABLE_BLUEPRINT_INPUT]: {
          componentType: table.default,
        },
      },
    };
  };
}
