import {ChangeDetectionStrategy, Component} from '@angular/core';
import {AbstractTableContentComponent, TableContentImportsModule} from "../../../abstract-table-content.component";

import {CommonModule} from "@angular/common";
import {
  IDashboardTableTopProfitableProductsPerPackType
} from "../../../../../../../../@core/dashboard/tables/top-products-per-pack-type";
import {
  IDashboardTableCustomerDistributionByCity
} from "../../../../../../../../@core/dashboard/tables/customer-distribution-by-city";

@Component({
  standalone: true,
  imports: [TableContentImportsModule, CommonModule],
  templateUrl: './customer-distribution-by-city.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CustomerDistributionByCityComponent extends AbstractTableContentComponent<IDashboardTableCustomerDistributionByCity[]> {
}
