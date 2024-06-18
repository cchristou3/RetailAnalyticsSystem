import {ChangeDetectionStrategy, Component, inject, Injectable, Input} from '@angular/core';
import {Changeable, changeableFromConstValue, ChangeableValuePipe} from "../../@shared/utils/changeable";
import {Observable} from "rxjs";
import {RepresentingOption, getRepresentingOptions$} from "../../@shared/utils/representing-helpers";
import {IProductCategoryReferenceModel} from "../app-api";

export type ProductCategoryIdentifyingProps = Pick<IProductCategoryReferenceModel, 'id'>;
export type ProductCategoryRepresentingProps = Pick<IProductCategoryReferenceModel, 'name'>;

export type ProductCategoryReferencingProps = ProductCategoryIdentifyingProps & ProductCategoryRepresentingProps;

export type ProductCategoryOption<TEntry = never> = RepresentingOption<ProductCategoryIdentifyingProps, TEntry>;

@Injectable({
  providedIn: 'root'
})
export class ProductCategoryRepresentingService {
  public getLabel(productCategory: ProductCategoryRepresentingProps): Changeable<string> {
    return changeableFromConstValue(productCategory.name);
  }

  public getOptions<TEntry extends ProductCategoryReferencingProps>(
    productCategories: TEntry[],
  ): Observable<ProductCategoryOption<TEntry>[]> {
    return getRepresentingOptions$(
      productCategories,
      productCategory => this.getLabel(productCategory),
      productCategory => ({id: productCategory.id}),
    );
  }
}

@Component({
  selector: 'app-product-category-representing',
  standalone: true,
  imports: [ChangeableValuePipe],
  template: `
    {{ labelChangeable | changeableValue }}
  `,
  styles: [
    ':host {display: contents}',
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductCategoryRepresentingComponent {
  protected readonly representingService = inject(ProductCategoryRepresentingService);

  @Input()
  set productCategory(productCategory: ProductCategoryRepresentingProps | null | undefined) {
    this.labelChangeable = productCategory
      ? this.representingService.getLabel(productCategory)
      : undefined;
  }

  protected labelChangeable?: Changeable<string>;
}
