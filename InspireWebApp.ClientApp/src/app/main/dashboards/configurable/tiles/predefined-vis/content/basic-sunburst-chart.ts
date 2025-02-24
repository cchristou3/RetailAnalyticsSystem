﻿import {
  CommonVizDataDescriptor,
  CommonVizDatasetType,
  DEFAULT_DATASET, DEFAULT_SERIES_VALUE
} from "../../../../../../@shared/charts/common-viz-data";
import {createConfigureChartFn} from "../../../../../../@shared/charts/factories/shared";
import {configureSunburstChart} from "../../../../../../@shared/charts/factories/sunburst.chart";

export const dataDescriptor: CommonVizDataDescriptor = {
  [DEFAULT_DATASET]: {
    type: CommonVizDatasetType.Hierarchical,
    keyField: 'category',
    childrenField: 'children',
    surrogateRoot: true,
    valueFields: {
      volume: {[DEFAULT_SERIES_VALUE]: 'volume'},
      value: {[DEFAULT_SERIES_VALUE]: 'value'},
    },
  },
};

export const configureFn = createConfigureChartFn(configureSunburstChart, {});
