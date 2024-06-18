import {Routes} from "@angular/router";
import {TITLE_PROVIDER_KEY} from "../../@shared/page-title/page-title-provider";
import {translatedScopedTitle} from "../../@shared/page-title/common-title-providers";
import {FormLossPreventionGuard} from "../../@shared/form-loss-prevention/form-loss-prevention.guard";
import {PRESENT_404_KEY} from "../../@shared/error-handling/resolver-error-options";
import {createTranslocoLoader} from "../../@transloco/transloco.helpers";
import {TRANSLOCO_SCOPE, TranslocoScope} from "@ngneat/transloco";
import {ListProductCategoriesComponent} from "./list-product-categories/list-product-categories.component";
import {
  ViewProductCategoryComponent,
  ViewProductCategoryTitleProvider,
} from "./view-product-category/view-product-category.component";
import {
  ManageProductCategoryComponent,
  EditProductCategoryTitleProvider,
} from "./manage-product-category/manage-product-category.component";
import {
  resolveProductCategoriesList,
  resolveProductCategoryDetails,
  resolveProductCategoryForUpdate,
} from "../../@core/product-categories/product-categories.resolvers";

const translocoLoader = createTranslocoLoader(
  // @ts-ignore
  () => import(/* webpackMode: "eager" */ './i18n-product-categories/en.json'),
  lang => import(/* webpackChunkName: "product-categories-i18n" */ `./i18n-product-categories/${lang}.json`)
);

export const routes: Routes = [{
  path: '',
  providers: [
    {
      provide: TRANSLOCO_SCOPE,
      useValue: <TranslocoScope>{scope: 'productCategories', loader: translocoLoader},
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
      component: ManageProductCategoryComponent,
      data: {
        [TITLE_PROVIDER_KEY]: translatedScopedTitle('manage.header.add'),
      },
      canDeactivate: [
        FormLossPreventionGuard,
      ],
    },
    {
      path: 'edit/:id',
      component: ManageProductCategoryComponent,
      data: {
        [TITLE_PROVIDER_KEY]: EditProductCategoryTitleProvider,
        [PRESENT_404_KEY]: <string[]> ['item'],
      },
      resolve: {
        item: resolveProductCategoryForUpdate,
      },
      providers: [
        EditProductCategoryTitleProvider,
      ],
      canDeactivate: [
        FormLossPreventionGuard,
      ],
    },
    {
      path: 'list',
      component: ListProductCategoriesComponent,
      resolve: {
        items: resolveProductCategoriesList,
      },
      data: {
        [TITLE_PROVIDER_KEY]: translatedScopedTitle('list.header'),
      },
    },
    {
      path: 'view/:id',
      component: ViewProductCategoryComponent,
      data: {
        [TITLE_PROVIDER_KEY]: ViewProductCategoryTitleProvider,
        [PRESENT_404_KEY]: <string[]> ['item'],
      },
      resolve: {
        item: resolveProductCategoryDetails,
      },
      providers: [
        ViewProductCategoryTitleProvider,
      ],
    },
  ],
}];
