import {ChangeDetectionStrategy, Component, inject, Injectable, Input} from '@angular/core';
import {Changeable, changeableFromConstValue, ChangeableValuePipe} from "../../@shared/utils/changeable";
import {Observable} from "rxjs";
import {RepresentingOption, getRepresentingOptions$} from "../../@shared/utils/representing-helpers";
import {IProductReferenceModel} from "../app-api";

export type ProductIdentifyingProps = Pick<IProductReferenceModel, 'id'>;
export type ProductRepresentingProps = Pick<IProductReferenceModel, 'name'>;

export type ProductReferencingProps = ProductIdentifyingProps & ProductRepresentingProps;

export type ProductOption<TEntry = never> = RepresentingOption<ProductIdentifyingProps, TEntry>;

@Injectable({
  providedIn: 'root'
})
export class ProductRepresentingService {
  public getLabel(product: ProductRepresentingProps): Changeable<string> {
    return changeableFromConstValue(product.name);
  }

  public getOptions<TEntry extends ProductReferencingProps>(
    products: TEntry[],
  ): Observable<ProductOption<TEntry>[]> {
    return getRepresentingOptions$(
      products,
      product => this.getLabel(product),
      product => ({id: product.id}),
    );
  }
}

@Component({
  selector: 'app-product-representing',
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
export class ProductRepresentingComponent {
  protected readonly representingService = inject(ProductRepresentingService);

  @Input()
  set product(product: ProductRepresentingProps | null | undefined) {
    this.labelChangeable = product
      ? this.representingService.getLabel(product)
      : undefined;
  }

  protected labelChangeable?: Changeable<string>;
}
