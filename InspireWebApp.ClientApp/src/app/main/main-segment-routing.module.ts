import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {MainSegmentComponent} from "./main-segment.component";
import {MustLoginGuard} from "../@core/auth/must-login.guard";

const routes: Routes = [{
  path: '',
  component: MainSegmentComponent,
  canActivateChild: [
    MustLoginGuard,
  ],
  children: [
    {
      path: '',
      redirectTo: 'dashboards',
      pathMatch: 'full',
    },
    {
      path: 'dashboards',
      children: [
        {
          path: '',
          redirectTo: 'sales-performance-analysis',
          pathMatch: 'full',
        },
        // {
        //   path: 'demo',
        //   loadChildren: () => import('./dashboards/demo/demo-dashboard.entrypoint')
        //     .then(i => i.routes),
        // },
        {
          path: 'sales-performance-analysis',
          loadChildren: () => import('./dashboards/sales-performance-analysis/sales-performance-analysis-dashboard.entrypoint')
            .then(i => i.routes),
        },
        {
          path: 'city-analysis',
          loadChildren: () => import('./dashboards/city-analysis/city-analysis-dashboard.entrypoint')
            .then(i => i.routes),
        },
        {
          path: 'customer-analysis',
          loadChildren: () => import('./dashboards/customer-analysis/customer-analysis-dashboard.entrypoint')
            .then(i => i.routes),
        },
        {
          path: 'product-analysis',
          loadChildren: () => import('./dashboards/product-analysis/product-analysis-dashboard.entrypoint')
            .then(i => i.routes),
        },
        {
          path: 'behavioural-analysis',
          loadChildren: () => import('./dashboards/behavioural-analysis/behavioural-analysis-dashboard.entrypoint')
            .then(i => i.routes),
        },
        // {
        //   path: 'configurable',
        //   loadChildren: () => import('./dashboards/configurable/configurable-dashboard.entrypoint')
        //     .then(i => i.routes),
        // },
        // {
        //   path: 'predefined',
        //   children: [
        //     {
        //       path: 'market-basket',
        //       loadChildren: () => import('./dashboards/predefined/market-basket/market-basket.entrypoint')
        //         .then(i => i.routes),
        //     },
        //   ],
        // },
      ],
    },
    {
      path: 'account',
      loadChildren: () => import('./account/account.module')
        .then(m => m.AccountModule),
    },
    {
      path: 'product-categories',
      loadChildren: () => import('./product-categories/product-categories.entrypoint')
        .then(i => i.routes),
    },
    {
      path: 'products',
      loadChildren: () => import('./products/products.entrypoint')
        .then(i => i.routes),
    },
    {
      path: 'promotion-types',
      loadChildren: () => import('./promotion-types/promotion-types.entrypoint')
        .then(i => i.routes),
    },
    {
      path: 'suppliers',
      loadChildren: () => import('./suppliers/suppliers.entrypoint'),
    },
  ],
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MainSegmentRoutingModule {
}
