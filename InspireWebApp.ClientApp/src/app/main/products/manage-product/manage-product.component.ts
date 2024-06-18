import {Component, Injectable, OnInit} from '@angular/core';
import {ManageComponentMode} from "../../../@shared/utils/manage-component-mode.enum";
import {ActivatedRoute, Router} from "@angular/router";
import {ApiResult} from "../../../@shared/utils/api-result";
import {FormControl, FormGroup, ReactiveFormsModule} from "@angular/forms";
import {UntilDestroy, untilDestroyed} from "@ngneat/until-destroy";
import {SubmittableFormDisabler} from "../../../@shared/submittable-form/submittable-form-disabler";
import {CrudHelpersService} from "../../../@shared/utils/crud-helpers.service";
import {BehaviorSubject, Observable} from "rxjs";
import {EditItemTitleProvider} from "../../../@shared/page-title/common-title-providers";
import {CommonModule} from "@angular/common";
import {TranslocoModule} from "@ngneat/transloco";
import {FormControlErrorsModule} from "../../../@shared/form-control-errors/form-control-errors.module";
import {IdNamespaceModule} from "../../../@shared/id-namespace/id-namespace.module";
import {PanelModule} from "primeng/panel";
import {ButtonModule} from "primeng/button";
import {BlockUIModule} from "primeng/blockui";
import {ProgressSpinnerModule} from "primeng/progressspinner";
import {FormLossPreventionModule} from "../../../@shared/form-loss-prevention/form-loss-prevention.module";
import {MenuItem} from "primeng/api";
import {MenuTitleTranslation, MenuTranslatorService} from "../../../@shared/dynamic-menu/translated-menu";
import {DynamicMenuPipe} from "../../../@shared/dynamic-menu/dynamic-menu.pipe";
import {PanelHeaderActionsComponent} from "../../../@shared/panel-header-actions/panel-header-actions.component";
import {RequiredFieldIndicatorModule} from "../../../@shared/required-field-indicator/required-field-indicator.module";
import {CommonValidators} from "../../../@shared/utils/common-validators";

import {
  ProductsClient,
  ProductCreateModel,
  ProductUpdateModel,
  ProductCategoryReferenceModel,
  PromotionTypeReferenceModel,
} from "../../../@core/app-api";
import {ProductRepresentingService} from "../../../@core/products/product-representing.utils";
import {
  ProductCategoryOption,
  ProductCategoryRepresentingService,
} from "../../../@core/product-categories/product-category-representing.utils";
import {
  PromotionTypeOption,
  PromotionTypeRepresentingService,
} from "../../../@core/promotion-types/promotion-type-representing.utils";
import {asPromotionTypeIdentifier} from "../../../@core/promotion-types/promotion-types-model.helpers";
import {DropdownModule} from "primeng/dropdown";
import {DropdownDefaultsModule} from "../../../@shared/defaults/dropdown-defaults.module";
import {InputTextModule} from "primeng/inputtext";
import {InputNumberModule} from "primeng/inputnumber";
import {InputTextareaModule} from "primeng/inputtextarea";
import {MultiSelectModule} from "primeng/multiselect";
import {MultiSelectDefaultsModule} from "../../../@shared/defaults/multi-select-defaults";
import {emptyStringToNull} from "../../../@shared/utils/string.helpers";

interface ProductCreateForm {
  category: FormControl<ProductCategoryReferenceModel | null>;
  name: FormControl<string>;
  price: FormControl<number | null>;
  description: FormControl<string>;
  promotionTypes: FormControl<PromotionTypeReferenceModel[]>;
}

interface ProductUpdateForm {
  category: FormControl<ProductCategoryReferenceModel | null>;
  name: FormControl<string>;
  price: FormControl<number | null>;
  description: FormControl<string>;
  promotionTypes: FormControl<PromotionTypeReferenceModel[]>;
}

@UntilDestroy()
@Component({
  templateUrl: './manage-product.component.html',
  standalone: true,
  imports: [
    CommonModule,
    TranslocoModule,

    ReactiveFormsModule,
    FormControlErrorsModule,
    IdNamespaceModule,
    PanelModule,
    PanelHeaderActionsComponent,
    DynamicMenuPipe,
    ButtonModule,
    BlockUIModule,
    ProgressSpinnerModule,
    FormLossPreventionModule,
    RequiredFieldIndicatorModule,
    DropdownModule,
    DropdownDefaultsModule,
    InputTextModule,
    InputNumberModule,
    InputTextareaModule,
    MultiSelectModule,
    MultiSelectDefaultsModule,
  ],
})
export class ManageProductComponent implements OnInit {
  // Info from route
  manageMode!: ManageComponentMode;
  itemId?: number;

  // FK dropdowns data
  productCategories!: ProductCategoryReferenceModel[];
  promotionTypes!: PromotionTypeReferenceModel[];

  // FK options
  productCategoryOptions$!: Observable<ProductCategoryOption<ProductCategoryReferenceModel>[]>;
  promotionTypeOptions$!: Observable<PromotionTypeOption<PromotionTypeReferenceModel>[]>;

  // Internal component state
  form!: FormGroup<ProductCreateForm> | FormGroup<ProductUpdateForm>;
  formDisabler!: SubmittableFormDisabler;

  // Page title
  private editItemTitleSubject = new BehaviorSubject<string | undefined>(undefined);
  public editItemTitle$ = this.editItemTitleSubject.asObservable();

  // The top right menu
  protected panelMenu!: Readonly<MenuItem[]>;

  constructor(
    public readonly activeRoute: ActivatedRoute,
    private readonly productsClient: ProductsClient,
    private readonly crudHelpers: CrudHelpersService,
    private readonly router: Router,
    private readonly menuTranslator: MenuTranslatorService,
    private readonly representingService: ProductRepresentingService,
    private readonly productCategoryRepresentingService: ProductCategoryRepresentingService,
    private readonly promotionTypeRepresentingService: PromotionTypeRepresentingService,
  ) {
  }

  ngOnInit(): void {
    const routeData = this.activeRoute.snapshot.data;

    const productCategoriesResult: ApiResult<ProductCategoryReferenceModel[]> = routeData['productCategories'];
    this.productCategories = productCategoriesResult.value!;
    this.productCategoryOptions$ = this.productCategoryRepresentingService.getOptions(this.productCategories);

    const promotionTypesResult: ApiResult<PromotionTypeReferenceModel[]> = routeData['promotionTypes'];
    this.promotionTypes = promotionTypesResult.value!;
    this.promotionTypeOptions$ = this.promotionTypeRepresentingService.getOptions(this.promotionTypes);

    if (routeData.hasOwnProperty('item')) {
      this.itemId = +this.activeRoute.snapshot.paramMap.get('id')!;
      this.manageMode = ManageComponentMode.Edit;

      const itemResult: ApiResult<ProductUpdateModel> = routeData['item'];
      const item = itemResult.value!;

      const form = this.form = this.buildUpdateForm();
      form.setValue({
        category: this.productCategories.find(p => p.id === item.categoryId) ?? null,
        name: item.name,
        price: item.price,
        description: item.description ?? '',
        promotionTypes: item.promotionTypes.map(id => this.promotionTypes.find(ref => ref.id == id.id)!),
      });

      this.updateEditItemTitle();

      this.panelMenu = [
        {
          icon: 'pi pi-search',
          [MenuTitleTranslation]: {key: 'buttons.viewDetails'},
          routerLink: ['../../view/', this.itemId],
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
    } else {
      this.manageMode = ManageComponentMode.Add;
      this.form = this.buildCreateForm();

      this.panelMenu = [
        {
          icon: 'pi pi-list',
          [MenuTitleTranslation]: {key: 'buttons.listAll'},
          routerLink: '../',
        },
      ];
    }

    this.formDisabler = new SubmittableFormDisabler(this.form);

    this.menuTranslator.translateAllMenuItems(this.panelMenu, {
      pipe: untilDestroyed(this),
    });
  }

  submitForm() {
    if (this.manageMode == ManageComponentMode.Add) this.addItem();
    else this.updateItem();
  }

  private addItem(): void {
    const form = <FormGroup<ProductCreateForm>>this.form;

    const category = form.controls.category.value;

    const model = new ProductCreateModel({
      categoryId: category!.id,
      name: form.controls.name.value,
      price: form.controls.price.value!,
      description: emptyStringToNull(form.controls.description.value),
      promotionTypes: form.controls.promotionTypes.value.map(asPromotionTypeIdentifier),
    });

    this.productsClient.create(model)
      .pipe(
        this.formDisabler.monitor.monitor(),
        this.crudHelpers.handleManageToasts(
          this.representingService
            .getLabel({
              name: form.controls.name.value,
            })
            .value,
          this.manageMode,
        ),
        untilDestroyed(this),
      )
      .subscribe(id => {
        this.form.markAsPristine();
        this.router.navigate(['../view/', id], {relativeTo: this.activeRoute});
      });
  }

  private updateItem(): void {
    const form = <FormGroup<ProductUpdateForm>>this.form;

    const category = form.controls.category.value;

    const model = new ProductUpdateModel({
      categoryId: category!.id,
      name: form.controls.name.value,
      price: form.controls.price.value!,
      description: emptyStringToNull(form.controls.description.value),
      promotionTypes: form.controls.promotionTypes.value.map(asPromotionTypeIdentifier),
    });

    this.productsClient.update(this.itemId!, model)
      .pipe(
        this.formDisabler.monitor.monitor(),
        this.crudHelpers.handleManageToasts(
          this.representingService
            .getLabel({
              name: form.controls.name.value,
            })
            .value,
          this.manageMode,
        ),
        untilDestroyed(this),
      )
      .subscribe(() => {
        this.form.markAsPristine();
        this.updateEditItemTitle();
      });
  }

  private updateEditItemTitle() {
    const form = <FormGroup<ProductUpdateForm>>this.form;

    this.editItemTitleSubject.next(
      this.representingService
        .getLabel({
          name: form.controls.name.value,
        })
        .value,
    );
  }

  private buildCreateForm(): FormGroup<ProductCreateForm> {
    return new FormGroup({
      category: new FormControl<ProductCategoryReferenceModel | null>(null, {
        validators: [
          CommonValidators.required(),
        ],
      }),
      name: new FormControl('', {
        nonNullable: true,
        validators: [
          CommonValidators.required(),
          CommonValidators.maxLength({value: 70}),
          CommonValidators.minLength({value: 3}),
        ],
        asyncValidators: [
          CommonValidators.commonUniqueValidator(abstractControl => {
            const control = abstractControl as FormControl<string>;
            const value = control.value;

            return this.productsClient.checkUniqueName(this.itemId, value);
          }),
        ],
      }),
      price: new FormControl<number | null>(null, {
        validators: [
          CommonValidators.required(),
        ],
      }),
      description: new FormControl('', {nonNullable: true}),
      promotionTypes: new FormControl<PromotionTypeReferenceModel[]>([], {nonNullable: true}),
    });
  }

  private buildUpdateForm(): FormGroup<ProductUpdateForm> {
    return new FormGroup({
      category: new FormControl<ProductCategoryReferenceModel | null>(null, {
        validators: [
          CommonValidators.required(),
        ],
      }),
      name: new FormControl('', {
        nonNullable: true,
        validators: [
          CommonValidators.required(),
          CommonValidators.maxLength({value: 70}),
          CommonValidators.minLength({value: 3}),
        ],
        asyncValidators: [
          CommonValidators.commonUniqueValidator(abstractControl => {
            const control = abstractControl as FormControl<string>;
            const value = control.value;

            return this.productsClient.checkUniqueName(this.itemId, value);
          }),
        ],
      }),
      price: new FormControl<number | null>(null, {
        validators: [
          CommonValidators.required(),
        ],
      }),
      description: new FormControl('', {nonNullable: true}),
      promotionTypes: new FormControl<PromotionTypeReferenceModel[]>([], {nonNullable: true}),
    });
  }
}

@Injectable()
export class EditProductTitleProvider extends EditItemTitleProvider<ManageProductComponent> {
  getItemName$(component: ManageProductComponent): Observable<string | undefined> {
    return component.editItemTitle$;
  }
}
