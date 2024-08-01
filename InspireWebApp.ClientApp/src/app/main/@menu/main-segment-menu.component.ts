import {ChangeDetectionStrategy, Component} from '@angular/core';
import {MenuItem} from "primeng/api";
import {IsActiveMatchOptions} from "@angular/router";

const fallbackRouterMatchOptions: IsActiveMatchOptions = {
  // Most of our content is actually at sub-path, e.g.
  // `/app/countries` -> `/app/countries/list` or `/app/dashboard` -> `/app/dashboard/1`
  paths: 'subset',

  queryParams: 'ignored',
  matrixParams: 'ignored',
  fragment: 'ignored',
};

@Component({
  selector: 'app-main-segment-menu',
  templateUrl: './main-segment-menu.component.html',
  styleUrls: ['./main-segment-menu.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MainSegmentMenuComponent {
  protected readonly fallbackRouterMatchOptions = fallbackRouterMatchOptions;

  // Note that separators are not supported (no styling for them)
  items: MenuItem[] = [
    {
      label: 'Home',
      items: [
        // {label: 'Dashboard', icon: 'pi pi-fw pi-chart-bar', routerLink: '/app/dashboards/configurable'},
        {label: 'Sales Performance Analysis', icon: 'pi pi-fw pi-chart-bar', routerLink: '/app/dashboards/sales-performance-analysis'},
        {label: 'City Analysis', icon: 'pi pi-fw pi-chart-bar', routerLink: '/app/dashboards/city-analysis'},
        {label: 'Customer Analysis', icon: 'pi pi-fw pi-chart-bar', routerLink: '/app/dashboards/customer-analysis'},
        {label: 'Product Analysis', icon: 'pi pi-fw pi-chart-bar', routerLink: '/app/dashboards/product-analysis'},
        {label: 'Behavioural Analysis', icon: 'pi pi-fw pi-chart-bar', routerLink: '/app/dashboards/behavioural-analysis'},
      ],
    },
    // {
    //   label: 'Data management',
    //   items: [
    //     {label: 'Product categories', icon: 'pi pi-fw pi-briefcase', routerLink: '/app/product-categories'},
    //     {label: 'Promotion types', icon: 'pi pi-fw pi-briefcase', routerLink: '/app/promotion-types'},
    //     {label: 'Products', icon: 'pi pi-fw pi-briefcase', routerLink: '/app/products'},
    //     {label: 'Suppliers', icon: 'pi pi-fw pi-briefcase', routerLink: '/app/suppliers'},
    //   ],
    // },
  ];
}
