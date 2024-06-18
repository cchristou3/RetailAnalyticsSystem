import {Component, inject, Injectable, OnInit} from '@angular/core';
import {ApiResult} from "../../../@shared/utils/api-result";
import {ActivatedRoute, Router, RouterModule} from "@angular/router";
import {UntilDestroy, untilDestroyed} from "@ngneat/until-destroy";
import {CrudHelpersService} from "../../../@shared/utils/crud-helpers.service";
import {SubscriptionsCounter} from "../../../@shared/utils/subscriptions-counter";
import {ResolvedApiItemTitleProvider} from "../../../@shared/page-title/common-title-providers";
import {CommonModule} from "@angular/common";
import {TranslocoModule} from "@ngneat/transloco";
import {PanelModule} from "primeng/panel";
import {DetailsListModule} from "../../../@shared/details-list/details-list.module";
import {ButtonModule} from "primeng/button";
import {DeletionConfirmDialogModule} from "../../../@shared/deletion-confirm-dialog/deletion-confirm-dialog.module";
import {MenuItem} from "primeng/api";
import {PanelHeaderActionsComponent} from "../../../@shared/panel-header-actions/panel-header-actions.component";
import {DynamicMenuPipe} from "../../../@shared/dynamic-menu/dynamic-menu.pipe";
import {MenuTitleTranslation, MenuTranslatorService} from "../../../@shared/dynamic-menu/translated-menu";
import {Changeable} from "../../../@shared/utils/changeable";
import {ProductsClient, ProductDetailsModel, IPromotionTypeReferenceModel} from "../../../@core/app-api";
import {ProductRepresentingService} from "../../../@core/products/product-representing.utils";
import {
  ProductCategoryRepresentingComponent,
} from "../../../@core/product-categories/product-category-representing.utils";
import {MultiLineTextComponent} from "../../../@shared/utils/multi-line-text.component";
import {ChipsListComponent} from "../../../@shared/chips-list/chips-list.component";
import {TemplateTypeWitnessModule} from "../../../@shared/utils/template-type-witness";
import {
  PromotionTypeRepresentingComponent,
} from "../../../@core/promotion-types/promotion-type-representing.utils";
import {RouterNavCommands} from "../../../@shared/utils/routing-helpers";
import {EmptyReplacerModule} from "../../../@shared/display-substitution/empty-replacer";
import {NullReplacerModule} from "../../../@shared/display-substitution/null-replacer";
import {ChipModule} from "primeng/chip";

@UntilDestroy()
@Component({
  templateUrl: './view-product.component.html',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TranslocoModule,

    PanelModule,
    PanelHeaderActionsComponent,
    DynamicMenuPipe,
    DetailsListModule,
    ButtonModule,
    DeletionConfirmDialogModule,
    ProductCategoryRepresentingComponent,
    MultiLineTextComponent,
    ChipsListComponent,
    TemplateTypeWitnessModule,
    PromotionTypeRepresentingComponent,
    EmptyReplacerModule,
    NullReplacerModule,
    ChipModule,
  ],
})
export class ViewProductComponent implements OnInit {
  item!: ProductDetailsModel;

  isDeleteDialogOpen = false;
  deleteMonitor = new SubscriptionsCounter();

  constructor(
    private readonly activatedRoute: ActivatedRoute,
    private readonly productsClient: ProductsClient,
    private readonly router: Router,
    private readonly crudHelpers: CrudHelpersService,
    private readonly menuTranslator: MenuTranslatorService,
    protected readonly representingService: ProductRepresentingService,
  ) {
  }

  protected panelMenu!: Readonly<MenuItem[]>;

  ngOnInit(): void {
    this.item = (this.activatedRoute.snapshot.data['item'] as ApiResult<ProductDetailsModel>).value!;

    this.panelMenu = [
      {
        icon: 'pi pi-pencil',
        [MenuTitleTranslation]: {key: 'buttons.edit'},
        routerLink: ['../../edit/', this.item.id],
      },
      {
        icon: 'pi pi-trash',
        [MenuTitleTranslation]: {key: 'buttons.delete'},
        command: () => this.isDeleteDialogOpen = true,
      },
      {separator: true},
      {
        icon: 'pi pi-plus',
        [MenuTitleTranslation]: {key: 'buttons.create'},
        routerLink: '../../add',
      },
      {
        icon: 'pi pi-list',
        [MenuTitleTranslation]: {key: 'buttons.listAll'},
        routerLink: '../../',
      },
    ];

    this.menuTranslator.translateAllMenuItems(this.panelMenu, {
      pipe: untilDestroyed(this),
    });
  }

  delete() {
    this.productsClient.delete(this.item.id)
      .pipe(
        this.deleteMonitor.monitor({decrementOnlyOnError: true}),
        this.crudHelpers.handleDelete(this.representingService.getLabel(this.item).value),
        untilDestroyed(this),
      )
      .subscribe(() => this.router.navigate(['../../list'], {relativeTo: this.activatedRoute}));
  }

  protected getPromotionTypeRouterCommands(promotionType: IPromotionTypeReferenceModel): RouterNavCommands {
    return ['../../../promotion-types/view', promotionType.id];
  }
}

@Injectable()
export class ViewProductTitleProvider extends ResolvedApiItemTitleProvider<ProductDetailsModel> {
  private readonly representingService = inject(ProductRepresentingService);

  getItemTitle(item: ProductDetailsModel): Changeable<string> {
    return this.representingService.getLabel(item);
  }

  protected override getTranslocoSuffix(): { key: string; scoped: boolean } | null {
    return {
      key: 'view.title_suffix',
      scoped: true,
    };
  }
}
