import {ChangeDetectionStrategy, Component} from '@angular/core';
import {AbstractTableContentComponent, TableContentImportsModule} from "../../../abstract-table-content.component";

import {CommonModule} from "@angular/common";
import {
  IDashboardTableTopProfitableProductsPerPackType
} from "../../../../../../../../@core/dashboard/tables/top-products-per-pack-type";

@Component({
  standalone: true,
  imports: [TableContentImportsModule, CommonModule],
  templateUrl: './top-products-per-pack-type.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TopProfitableProductsPerPackTypeComponent extends AbstractTableContentComponent<IDashboardTableTopProfitableProductsPerPackType[]> {
}
