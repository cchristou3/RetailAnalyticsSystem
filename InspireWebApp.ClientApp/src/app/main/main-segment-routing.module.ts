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
          redirectTo: 'configurable',
          pathMatch: 'full',
        },
        {
          path: 'demo',
          loadChildren: () => import('./dashboards/demo/demo-dashboard.entrypoint')
            .then(i => i.routes),
        },
        {
          path: 'configurable',
          loadChildren: () => import('./dashboards/configurable/configurable-dashboard.entrypoint')
            .then(i => i.routes),
        },
        {
          path: 'predefined',
          children: [
            {
              path: 'market-basket',
              loadChildren: () => import('./dashboards/predefined/market-basket/market-basket.entrypoint')
                .then(i => i.routes),
            },
          ],
        },
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
