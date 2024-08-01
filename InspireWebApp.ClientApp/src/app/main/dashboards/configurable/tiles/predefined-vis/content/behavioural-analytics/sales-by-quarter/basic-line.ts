import {
  CommonVizDataDescriptor,
  CommonVizDatasetType,
  DEFAULT_DATASET, DEFAULT_SERIES_VALUE
} from "../../../../../../../../@shared/charts/common-viz-data";
import {createConfigureChartFn} from "../../../../../../../../@shared/charts/factories/shared";
import {configureLineChart, LineChartOptions} from "../../../../../../../../@shared/charts/factories/line.chart";

export const dataDescriptor: CommonVizDataDescriptor = {
  [DEFAULT_DATASET]: {
    type: CommonVizDatasetType.Tabular,
    keyFields: ['quarter'],
    valueFields: {
      volume: {[DEFAULT_SERIES_VALUE]: 'volume'},
      value: {[DEFAULT_SERIES_VALUE]: 'value'},
    },
  },
};

const options: LineChartOptions = {
  enableArea: true,
  title: 'Quarterly Sales (Mean)'
}

export const configureFn = createConfigureChartFn(configureLineChart, options);
