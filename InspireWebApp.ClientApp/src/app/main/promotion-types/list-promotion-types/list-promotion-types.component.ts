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
import {PromotionTypesClient, PromotionTypesListModel} from "../../../@core/app-api";
import {PromotionTypeRepresentingService} from "../../../@core/promotion-types/promotion-type-representing.utils";
import {prepareTranslatedValue} from "../../../@shared/dynamic-component/common/translated-value.component";
import {buildNumericColumnOptions} from "../../../@shared/data-table/columns/numeric.column";
import {buildStringColumnOptions} from "../../../@shared/data-table/columns/string.column";

@UntilDestroy()
@Component({
  templateUrl: './list-promotion-types.component.html',
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
      useClass: forwardRef(() => ListPromotionTypesActionsContext),
    }
  ],
})
export class ListPromotionTypesComponent implements OnInit {
  constructor(
    private readonly activeRoute: ActivatedRoute,
    private readonly menuTranslator: MenuTranslatorService,
  ) {
  }

  items: PromotionTypesListModel[] = [];

  protected panelMenu: Readonly<MenuItem[]> = [
    {
      icon: 'pi pi-plus',
      [MenuTitleTranslation]: {key: 'buttons.create'},
      routerLink: '../add',
    },
  ];

  columns: ColumnSpec[] = [
    {
      header: prepareTranslatedValue('promotionTypes.fields.id'),
      ...buildNumericColumnOptions('id'),
    },
    {
      header: prepareTranslatedValue('promotionTypes.fields.name'),
      ...buildStringColumnOptions('name'),
    },
    commonActionsColumnOptions,
  ];

  ngOnInit() {
    this.items = (this.activeRoute.snapshot.data['items'] as ApiResult<PromotionTypesListModel[]>).value!;

    this.menuTranslator.translateAllMenuItems(this.panelMenu, {
      pipe: untilDestroyed(this),
    });
  }
}

@Injectable()
class ListPromotionTypesActionsContext implements CommonActionsContext<PromotionTypesListModel> {
  constructor(
    private readonly promotionTypesClient: PromotionTypesClient,
    private readonly representingService: PromotionTypeRepresentingService,
  ) {
  }

  getViewLinkCommands(item: PromotionTypesListModel): any[] {
    return ['../view/', item.id];
  }

  getEditLinkCommands(item: PromotionTypesListModel): any[] {
    return ['../edit/', item.id];
  }

  getItemNameForDelete(item: PromotionTypesListModel): Changeable<string> {
    return this.representingService.getLabel(item);
  }

  prepareDelete(item: PromotionTypesListModel): Observable<unknown> {
    return this.promotionTypesClient.delete(item.id);
  }
}
