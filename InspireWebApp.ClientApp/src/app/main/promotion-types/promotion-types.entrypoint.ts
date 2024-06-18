import {Routes} from "@angular/router";
import {TITLE_PROVIDER_KEY} from "../../@shared/page-title/page-title-provider";
import {translatedScopedTitle} from "../../@shared/page-title/common-title-providers";
import {FormLossPreventionGuard} from "../../@shared/form-loss-prevention/form-loss-prevention.guard";
import {PRESENT_404_KEY} from "../../@shared/error-handling/resolver-error-options";
import {createTranslocoLoader} from "../../@transloco/transloco.helpers";
import {TRANSLOCO_SCOPE, TranslocoScope} from "@ngneat/transloco";
import {ListPromotionTypesComponent} from "./list-promotion-types/list-promotion-types.component";
import {
  ViewPromotionTypeComponent,
  ViewPromotionTypeTitleProvider,
} from "./view-promotion-type/view-promotion-type.component";
import {
  ManagePromotionTypeComponent,
  EditPromotionTypeTitleProvider,
} from "./manage-promotion-type/manage-promotion-type.component";
import {
  resolvePromotionTypesList,
  resolvePromotionTypeDetails,
  resolvePromotionTypeForUpdate,
} from "../../@core/promotion-types/promotion-types.resolvers";

const translocoLoader = createTranslocoLoader(
  // @ts-ignore
  () => import(/* webpackMode: "eager" */ './i18n-promotion-types/en.json'),
  lang => import(/* webpackChunkName: "promotion-types-i18n" */ `./i18n-promotion-types/${lang}.json`)
);

export const routes: Routes = [{
  path: '',
  providers: [
    {
      provide: TRANSLOCO_SCOPE,
      useValue: <TranslocoScope>{scope: 'promotionTypes', loader: translocoLoader},
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
      component: ManagePromotionTypeComponent,
      data: {
        [TITLE_PROVIDER_KEY]: translatedScopedTitle('manage.header.add'),
      },
      canDeactivate: [
        FormLossPreventionGuard,
      ],
    },
    {
      path: 'edit/:id',
      component: ManagePromotionTypeComponent,
      data: {
        [TITLE_PROVIDER_KEY]: EditPromotionTypeTitleProvider,
        [PRESENT_404_KEY]: <string[]> ['item'],
      },
      resolve: {
        item: resolvePromotionTypeForUpdate,
      },
      providers: [
        EditPromotionTypeTitleProvider,
      ],
      canDeactivate: [
        FormLossPreventionGuard,
      ],
    },
    {
      path: 'list',
      component: ListPromotionTypesComponent,
      resolve: {
        items: resolvePromotionTypesList,
      },
      data: {
        [TITLE_PROVIDER_KEY]: translatedScopedTitle('list.header'),
      },
    },
    {
      path: 'view/:id',
      component: ViewPromotionTypeComponent,
      data: {
        [TITLE_PROVIDER_KEY]: ViewPromotionTypeTitleProvider,
        [PRESENT_404_KEY]: <string[]> ['item'],
      },
      resolve: {
        item: resolvePromotionTypeDetails,
      },
      providers: [
        ViewPromotionTypeTitleProvider,
      ],
    },
  ],
}];
