import {
  CommonVizDataDescriptor,
  CommonVizDatasetType,
  DEFAULT_DATASET, DEFAULT_SERIES, DEFAULT_SERIES_VALUE
} from "../../../../../../../../@shared/charts/common-viz-data";
import {createConfigureChartFn} from "../../../../../../../../@shared/charts/factories/shared";
import {configurePieChart, PieChartOptions} from "../../../../../../../../@shared/charts/factories/pie.chart";

export const dataDescriptor: CommonVizDataDescriptor = {
  [DEFAULT_DATASET]: {
    type: CommonVizDatasetType.Tabular,
    keyFields: ['productPackTypeName'],
    valueFields: {
      [DEFAULT_SERIES]: {
        [DEFAULT_SERIES_VALUE]: 'volume',
      },
    },
  },
};

const options: PieChartOptions = {
  title: 'Sales by Product Pack Type',
}

export const configureFn = createConfigureChartFn(configurePieChart, options);
