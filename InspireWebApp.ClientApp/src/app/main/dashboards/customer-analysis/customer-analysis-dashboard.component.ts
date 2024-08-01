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
  templateUrl: './customer-analysis-dashboard.component.html',
  styleUrls: ['./customer-analysis-dashboard.component.scss'],
})
export class CustomerAnalysisDashboardComponent {
  private readonly predefinedVisSpecProvider = inject(PredefinedVisSpecProvider);

  tileModels: Readonly<TileModel<PredefinedVisualizationType | undefined>[]> = [
    {
      layoutItem: {id: '', x :0, y :0, w :3, h :3, minW :2, minH :3},
      data: PredefinedVisualizationType.SalesByCustomerCategory,
    },
    {
      layoutItem: {id: '', x :3, y :0, w :3, h :3, minW :2, minH :3},
      data: PredefinedVisualizationType.CustomerDistributionByCategory,
    },
    {
      layoutItem: {id: '', x :0, y :3, w :6, h :3, minW :2, minH :3},
      data: PredefinedVisualizationType.CustomerDistributionBySegment,
    },
    {
      layoutItem: {id: '', x :0, y :3, w :6, h :3, minW :2, minH :3},
      data: PredefinedVisualizationType.SegmentDetails,
    }
  ].map((currElement, index) => {
    currElement.layoutItem.id  = index.toString()
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
