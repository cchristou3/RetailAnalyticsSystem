import {ChangeDetectionStrategy, Component} from '@angular/core';
import {AbstractTableContentComponent, TableContentImportsModule} from "../../../abstract-table-content.component";

import {CommonModule} from "@angular/common";
import {
  IDashboardTableTopProfitableProductsPerPackType
} from "../../../../../../../../@core/dashboard/tables/top-products-per-pack-type";
import {
  DashboardTableSegmentDetails,
  IDashboardTableSegmentDetails
} from "../../../../../../../../@core/dashboard/tables/segment-details";

@Component({
  standalone: true,
  imports: [TableContentImportsModule, CommonModule],
  templateUrl: './segment-details.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TopProfitableProductsPerPackTypeComponent extends AbstractTableContentComponent<IDashboardTableSegmentDetails[]> {
}
