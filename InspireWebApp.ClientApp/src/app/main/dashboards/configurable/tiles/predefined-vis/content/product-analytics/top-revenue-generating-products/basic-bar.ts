import {
  CommonVizDataDescriptor,
  CommonVizDatasetType,
  DEFAULT_DATASET, DEFAULT_SERIES, DEFAULT_SERIES_VALUE
} from "../../../../../../../../@shared/charts/common-viz-data";
import {createConfigureChartFn} from "../../../../../../../../@shared/charts/factories/shared";
import {BarChartOptions, configureBarChart} from "../../../../../../../../@shared/charts/factories/bar.chart";

export const dataDescriptor: CommonVizDataDescriptor = {
  [DEFAULT_DATASET]: {
    type: CommonVizDatasetType.Tabular,
    keyFields: ['productName'],
    valueFields: {
      [DEFAULT_SERIES]: {
        [DEFAULT_SERIES_VALUE]: 'value',
      },
    },
  },
};

const options: BarChartOptions = {
  title: 'Top Revenue-Generating Products'
}

export const configureFn = createConfigureChartFn(configureBarChart, options);
