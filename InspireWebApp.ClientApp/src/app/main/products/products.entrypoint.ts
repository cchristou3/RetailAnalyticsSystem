import {Routes} from "@angular/router";
import {TITLE_PROVIDER_KEY} from "../../@shared/page-title/page-title-provider";
import {translatedScopedTitle} from "../../@shared/page-title/common-title-providers";
import {FormLossPreventionGuard} from "../../@shared/form-loss-prevention/form-loss-prevention.guard";
import {PRESENT_404_KEY} from "../../@shared/error-handling/resolver-error-options";
import {createTranslocoLoader} from "../../@transloco/transloco.helpers";
import {TRANSLOCO_SCOPE, TranslocoScope} from "@ngneat/transloco";
import {ListProductsComponent} from "./list-products/list-products.component";
import {ViewProductComponent, ViewProductTitleProvider} from "./view-product/view-product.component";
import {ManageProductComponent, EditProductTitleProvider} from "./manage-product/manage-product.component";
import {
  resolveProductsList,
  resolveProductDetails,
  resolveProductForUpdate,
} from "../../@core/products/products.resolvers";
import {resolveProductCategoriesDropdown} from "../../@core/product-categories/product-categories.resolvers";
import {resolvePromotionTypesDropdown} from "../../@core/promotion-types/promotion-types.resolvers";

const translocoLoader = createTranslocoLoader(
  // @ts-ignore
  () => import(/* webpackMode: "eager" */ './i18n-products/en.json'),
  lang => import(/* webpackChunkName: "products-i18n" */ `./i18n-products/${lang}.json`)
);

export const routes: Routes = [{
  path: '',
  providers: [
    {
      provide: TRANSLOCO_SCOPE,
      useValue: <TranslocoScope>{scope: 'products', loader: translocoLoader},
    },
  ],
  children: [
    {
      path: '',
      pathMatch: 'full',
      redirectTo: 'list',
    },
    {
      path: 'add',
      component: ManageProductComponent,
      data: {
        [TITLE_PROVIDER_KEY]: translatedScopedTitle('manage.header.add'),
      },
      resolve: {
        productCategories: resolveProductCategoriesDropdown,
        promotionTypes: resolvePromotionTypesDropdown,
      },
      canDeactivate: [
        FormLossPreventionGuard,
      ],
    },
    {
      path: 'edit/:id',
      component: ManageProductComponent,
      data: {
        [TITLE_PROVIDER_KEY]: EditProductTitleProvider,
        [PRESENT_404_KEY]: <string[]> ['item'],
      },
      resolve: {
        item: resolveProductForUpdate,
        productCategories: resolveProductCategoriesDropdown,
        promotionTypes: resolvePromotionTypesDropdown,
      },
      providers: [
        EditProductTitleProvider,
      ],
      canDeactivate: [
        FormLossPreventionGuard,
      ],
    },
    {
      path: 'list',
      component: ListProductsComponent,
      resolve: {
        items: resolveProductsList,
      },
      data: {
        [TITLE_PROVIDER_KEY]: translatedScopedTitle('list.header'),
      },
    },
    {
      path: 'view/:id',
      component: ViewProductComponent,
      data: {
        [TITLE_PROVIDER_KEY]: ViewProductTitleProvider,
        [PRESENT_404_KEY]: <string[]> ['item'],
      },
      resolve: {
        item: resolveProductDetails,
      },
      providers: [
        ViewProductTitleProvider,
      ],
    },
  ],
}];
