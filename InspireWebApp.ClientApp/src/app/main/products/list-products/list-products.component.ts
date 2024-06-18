import {Component, forwardRef, Injectable, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Observable} from "rxjs";
import {
  commonActionsColumnOptions,
  COMMON_ACTIONS_CONTEXT,
  CommonActionsContext,
} from "../../../@shared/data-table/columns/common-actions/common-actions.column";
import {ApiResult} from "../../../@shared/utils/api-result";
import {CommonModule} from "@angular/common";
import {TranslocoModule} from "@ngneat/transloco";
import {PanelModule} from "primeng/panel";
import {DataTableModule} from "../../../@shared/data-table/data-table.module";
import {ColumnSpec} from "../../../@shared/data-table/common";
import {Changeable} from "../../../@shared/utils/changeable";
import {MenuItem} from "primeng/api";
import {MenuTitleTranslation, MenuTranslatorService} from "../../../@shared/dynamic-menu/translated-menu";
import {DynamicMenuPipe} from "../../../@shared/dynamic-menu/dynamic-menu.pipe";
import {PanelHeaderActionsComponent} from "../../../@shared/panel-header-actions/panel-header-actions.component";
import {UntilDestroy, untilDestroyed} from "@ngneat/until-destroy";
import {
  ProductsClient,
  ProductsListModel,
  IProductCategoryReferenceModel,
  IPromotionTypeReferenceModel,
} from "../../../@core/app-api";
import {ProductRepresentingService} from "../../../@core/products/product-representing.utils";
import {prepareTranslatedValue} from "../../../@shared/dynamic-component/common/translated-value.component";
import {buildNumericColumnOptions} from "../../../@shared/data-table/columns/numeric.column";
import {
  buildSingleReferenceColumnOptions,
} from "../../../@shared/data-table/columns/single-reference/single-reference.column";
import {
  ProductCategoryRepresentingService,
} from "../../../@core/product-categories/product-category-representing.utils";
import {buildStringColumnOptions} from "../../../@shared/data-table/columns/string.column";
import {
  buildMultipleReferencesColumnOptions,
} from "../../../@shared/data-table/columns/multiple-references/multiple-references.column";
import {PromotionTypeRepresentingService} from "../../../@core/promotion-types/promotion-type-representing.utils";

@UntilDestroy()
@Component({
  templateUrl: './list-products.component.html',
  standalone: true,
  imports: [
    CommonModule,
    TranslocoModule,

    PanelModule,
    PanelHeaderActionsComponent,
    DynamicMenuPipe,
    DataTableModule,
  ],
  providers: [
    {
      provide: COMMON_ACTIONS_CONTEXT,
      useClass: forwardRef(() => ListProductsActionsContext),
    }
  ],
})
export class ListProductsComponent implements OnInit {
  constructor(
    private readonly activeRoute: ActivatedRoute,
    private readonly menuTranslator: MenuTranslatorService,
    private readonly productCategoryRepresentingService: ProductCategoryRepresentingService,
    private readonly promotionTypeRepresentingService: PromotionTypeRepresentingService,
  ) {
  }

  items: ProductsListModel[] = [];

  protected panelMenu: Readonly<MenuItem[]> = [
    {
      icon: 'pi pi-plus',
      [MenuTitleTranslation]: {key: 'buttons.create'},
      routerLink: '../add',
    },
  ];

  columns: ColumnSpec[] = [
    {
      header: prepareTranslatedValue('products.fields.id'),
      ...buildNumericColumnOptions('id'),
    },
    {
      header: prepareTranslatedValue('products.fields.category'),
      globalFilter: 'category.name',
      sortField: 'category.name',
      ...buildSingleReferenceColumnOptions<IProductCategoryReferenceModel>(
        'category',
        productCategory => productCategory.id,
        productCategory => this.productCategoryRepresentingService.getLabel(productCategory),
        productCategory => ['../../product-categories/view', productCategory.id],
      ),
    },
    {
      header: prepareTranslatedValue('products.fields.name'),
      ...buildStringColumnOptions('name'),
    },
    {
      header: prepareTranslatedValue('products.fields.price'),
      ...buildNumericColumnOptions('price'),
    },
    {
      header: prepareTranslatedValue('products.fields.promotionTypes'),
      ...buildMultipleReferencesColumnOptions<IPromotionTypeReferenceModel>(
        'promotionTypes',
        promotionType => promotionType.id,
        promotionType => this.promotionTypeRepresentingService.getLabel(promotionType),
        promotionType => ['../../promotion-types/view', promotionType.id],
      ),
    },
    commonActionsColumnOptions,
  ];

  ngOnInit() {
    this.items = (this.activeRoute.snapshot.data['items'] as ApiResult<ProductsListModel[]>).value!;

    this.menuTranslator.translateAllMenuItems(this.panelMenu, {
      pipe: untilDestroyed(this),
    });
  }
}

@Injectable()
class ListProductsActionsContext implements CommonActionsContext<ProductsListModel> {
  constructor(
    private readonly productsClient: ProductsClient,
    private readonly representingService: ProductRepresentingService,
  ) {
  }

  getViewLinkCommands(item: ProductsListModel): any[] {
    return ['../view/', item.id];
  }

  getEditLinkCommands(item: ProductsListModel): any[] {
    return ['../edit/', item.id];
  }

  getItemNameForDelete(item: ProductsListModel): Changeable<string> {
    return this.representingService.getLabel(item);
  }

  prepareDelete(item: ProductsListModel): Observable<unknown> {
    return this.productsClient.delete(item.id);
  }
}
