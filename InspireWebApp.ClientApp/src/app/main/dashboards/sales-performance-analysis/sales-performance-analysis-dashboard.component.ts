import {Component, inject} from '@angular/core';
import {CommonModule} from '@angular/common';
import {TilesGridComponent} from "../@infrastructure/tiles-grid/tiles-grid.component";
import {TileModel, TilePresentSpec} from "../@infrastructure/shared";
import {changeableFromConstValue} from "../../../@shared/utils/changeable";
import {of} from "rxjs";
import {CheckboxModule} from "primeng/checkbox";
import {FormsModule} from "@angular/forms";
import {PredefinedVisualizationType} from "../../../@core/app-api";
import {PredefinedVisSpecProvider} from "../configurable/tiles/predefined-vis/provider";
import {removeByCondition} from "../../../@shared/utils/collection.helpers";
import {ButtonModule} from "primeng/button";

@Component({
  standalone: true,
  imports: [
    CommonModule,
    TilesGridComponent,
    CheckboxModule,
    FormsModule,
    ButtonModule,
  ],
  templateUrl: './sales-performance-analysis-dashboard.component.html',
  styleUrls: ['./sales-performance-analysis-dashboard.component.scss'],
})
export class SalesPerformanceAnalysisDashboardComponent {
  private readonly predefinedVisSpecProvider = inject(PredefinedVisSpecProvider);

  tileModels: Readonly<TileModel<PredefinedVisualizationType | undefined>[]> = [
    {
      layoutItem: {id: '', x: 0, y: 0, w: 6, h: 3, minW: 2, minH: 3},
      data: PredefinedVisualizationType.SalesByDate,
    },
    {
      layoutItem: {id: '', x: 0, y: 7, w: 3, h: 4, minW: 2, minH: 3},
      data: PredefinedVisualizationType.SalesByQuarter,
    },
    {
      layoutItem: {id: '', x: 3, y: 7, w: 3, h: 4, minW: 2, minH: 3},
      data: PredefinedVisualizationType.SalesByYear,
    },
    {
      // AVG: People tend to buy more items the later they go to the retail.
      layoutItem: {id: '', x: 0, y: 3, w: 3, h: 4, minW: 2, minH: 3},
      data: PredefinedVisualizationType.SalesByMonth,
    },
    {
      layoutItem: {id: '', x: 3, y: 3, w: 3, h: 4, minW: 2, minH: 3},
      data: PredefinedVisualizationType.SalesForecasting,
    },
    {
      layoutItem: {id: '', x: 0, y: 11, w: 3, h: 4, minW: 2, minH: 3},
      data: PredefinedVisualizationType.SalesByDay,
    },
    {
      layoutItem: {id: '', x: 3, y: 11, w: 3, h: 4, minW: 2, minH: 3},
      data: PredefinedVisualizationType.SalesByHour,
    }
  ].map((currElement, index) => {
    currElement.layoutItem.id = index.toString()
    return currElement
  });

  manageMode = false;

  provideTileSpec(id: string, tileData: PredefinedVisualizationType | undefined): TilePresentSpec {
    if (tileData) {
      return {
        ...this.predefinedVisSpecProvider.provide({
          type: tileData,
        }),

        actions: [
          {
            icon: 'pi pi-file-edit',
            title: of('Edit'),
            callback: () => {
              console.log('Edit clicked');
            },
          },
          {
            icon: 'pi pi-trash',
            title: of('Remove tile'),
            callback: () => {
              this.removeTile(id);
            },
          },
        ],
      };
    }

    return {
      component: async () => {
        const compImport = await import('../product-analysis/simple-card.tile');

        return {
          componentType: compImport.SimpleCardComponent,
        };
      },
      data: changeableFromConstValue(() => of(undefined)),

      actions: [
        {
          icon: 'pi pi-file-edit',
          title: of('Edit'),
          callback: () => {
            console.log('Edit clicked');
          },
        },
      ],
    };
  }

  readonly provideTileSpecBound = this.provideTileSpec.bind(this);

  private removeTile(id: string): void {
    const newModels = [...this.tileModels];
    removeByCondition(newModels, element => element.layoutItem.id === id);

    this.tileModels = newModels;
  }
}
