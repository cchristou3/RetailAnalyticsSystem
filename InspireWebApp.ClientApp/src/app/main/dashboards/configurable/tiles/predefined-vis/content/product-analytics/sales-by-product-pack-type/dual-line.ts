﻿import {
  CommonVizDataDescriptor,
  CommonVizDatasetType,
  DEFAULT_DATASET, DEFAULT_SERIES_VALUE
} from "../../../../../../../../@shared/charts/common-viz-data";
import {createConfigureChartFn} from "../../../../../../../../@shared/charts/factories/shared";
import {configureLineChart} from "../../../../../../../../@shared/charts/factories/line.chart";

export const dataDescriptor: CommonVizDataDescriptor = {
  [DEFAULT_DATASET]: {
    type: CommonVizDatasetType.Tabular,
    keyFields: ['productPackTypeName'],
    valueFields: {
      volume: {[DEFAULT_SERIES_VALUE]: 'volume'},
      value: {[DEFAULT_SERIES_VALUE]: 'value'},
    },
  },
};

export const configureFn = createConfigureChartFn(configureLineChart, {
  series: [
    {descriptorName: 'volume', displayName: 'Volume', enableArea: true},
    {descriptorName: 'value', displayName: 'Value'},
  ],
});
