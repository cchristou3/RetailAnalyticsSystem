import {
  CommonVizDataDescriptor,
  CommonVizDatasetType, DATE_FORMAT_DATE_ONLY,
  DEFAULT_DATASET, DEFAULT_SERIES_VALUE
} from "../../../../../../@shared/charts/common-viz-data";
import {createConfigureChartFn} from "../../../../../../@shared/charts/factories/shared";
import {configureLineChart} from "../../../../../../@shared/charts/factories/line.chart";

const dates = generateDates();
const dataset: { date: string, [key: string]: any }[] = [];
prepareDataset();

function generateDates() {
  const dates = [];

  const firstDate = new Date();
  firstDate.setDate(firstDate.getDate() - 100);
  firstDate.setHours(0, 0, 0, 0);

  for (let i = 0; i < 100; i++) {
    const newDate = new Date(firstDate);
    newDate.setDate(newDate.getDate() + i);

    dates.push(newDate);
  }

  return dates;
}

function prepareDataset(): void {
  for (const date of dates) {
    dataset.push({date: `${date.getFullYear()}-${date.getMonth()}-${date.getDate()}`});
  }
}

function populateSeriesData(key: string, value: number) {
  for (let i = 0; i < dataset.length; i++) {
    value = Math.round((Math.random() * 10 - 4.2) + value);

    dataset[i][key] = value;
  }
}

populateSeriesData('cyprus', 10);
populateSeriesData('europe', 20);

export const data = {
  "DEFAULT": dataset,
};

export const dataDescriptor: CommonVizDataDescriptor = {
  [DEFAULT_DATASET]: {
    type: CommonVizDatasetType.Tabular,
    keyFields: ['date'],
    valueFields: {
      cyprus: {[DEFAULT_SERIES_VALUE]: 'cyprus'},
      europe: {[DEFAULT_SERIES_VALUE]: 'europe'},
    },
    dateFields: {
      'date': {
        format: DATE_FORMAT_DATE_ONLY,
        baseInterval: {
          timeUnit: 'day',
          count: 1,
        },
      },
    },
  },
};

export const configureFn = createConfigureChartFn(configureLineChart, {
  series: [
    {descriptorName: 'cyprus', displayName: 'Cyprus'},
    {descriptorName: 'europe', displayName: 'EC (average)'},
  ],
});



