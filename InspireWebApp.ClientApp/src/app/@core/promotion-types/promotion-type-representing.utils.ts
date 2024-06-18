import {ChangeDetectionStrategy, Component, inject, Injectable, Input} from '@angular/core';
import {Changeable, changeableFromConstValue, ChangeableValuePipe} from "../../@shared/utils/changeable";
import {Observable} from "rxjs";
import {RepresentingOption, getRepresentingOptions$} from "../../@shared/utils/representing-helpers";
import {IPromotionTypeReferenceModel} from "../app-api";

export type PromotionTypeIdentifyingProps = Pick<IPromotionTypeReferenceModel, 'id'>;
export type PromotionTypeRepresentingProps = Pick<IPromotionTypeReferenceModel, 'name'>;

export type PromotionTypeReferencingProps = PromotionTypeIdentifyingProps & PromotionTypeRepresentingProps;

export type PromotionTypeOption<TEntry = never> = RepresentingOption<PromotionTypeIdentifyingProps, TEntry>;

@Injectable({
  providedIn: 'root'
})
export class PromotionTypeRepresentingService {
  public getLabel(promotionType: PromotionTypeRepresentingProps): Changeable<string> {
    return changeableFromConstValue(promotionType.name);
  }

  public getOptions<TEntry extends PromotionTypeReferencingProps>(
    promotionTypes: TEntry[],
  ): Observable<PromotionTypeOption<TEntry>[]> {
    return getRepresentingOptions$(
      promotionTypes,
      promotionType => this.getLabel(promotionType),
      promotionType => ({id: promotionType.id}),
    );
  }
}

@Component({
  selector: 'app-promotion-type-representing',
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
export class PromotionTypeRepresentingComponent {
  protected readonly representingService = inject(PromotionTypeRepresentingService);

  @Input()
  set promotionType(promotionType: PromotionTypeRepresentingProps | null | undefined) {
    this.labelChangeable = promotionType
      ? this.representingService.getLabel(promotionType)
      : undefined;
  }

  protected labelChangeable?: Changeable<string>;
}
