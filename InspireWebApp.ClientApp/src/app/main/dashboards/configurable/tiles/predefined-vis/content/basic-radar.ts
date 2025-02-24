﻿import {
  CommonVizDataDescriptor,
  CommonVizDatasetType,
  DEFAULT_DATASET, DEFAULT_SERIES, DEFAULT_SERIES_VALUE
} from "../../../../../../@shared/charts/common-viz-data";
import {createConfigureChartFn} from "../../../../../../@shared/charts/factories/shared";
import {configureRadarChart} from "../../../../../../@shared/charts/factories/radar.chart";

export const dataDescriptor: CommonVizDataDescriptor = {
  [DEFAULT_DATASET]: {
    type: CommonVizDatasetType.Tabular,
    keyFields: ['outlet'],
    valueFields: {
      [DEFAULT_SERIES]: {
        [DEFAULT_SERIES_VALUE]: 'volume',
      },
    },
  },
};

export const configureFn = createConfigureChartFn(configureRadarChart, {});
