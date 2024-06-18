import {
  IPromotionTypeReferenceModel,
  IPromotionTypesListModel,
  IPromotionTypeDetailsModel,
  PromotionTypeReferenceModel,
  PromotionTypeIdentifier,
} from "../app-api";

export function asPromotionTypeIdentifier(model: IPromotionTypesListModel): PromotionTypeIdentifier;
export function asPromotionTypeIdentifier(model: IPromotionTypeDetailsModel): PromotionTypeIdentifier;
export function asPromotionTypeIdentifier(model: IPromotionTypeReferenceModel): PromotionTypeIdentifier;
export function asPromotionTypeIdentifier(model: { id: number }): PromotionTypeIdentifier {
  return new PromotionTypeIdentifier({
    id: model.id,
  });
}

export function asPromotionTypeReference(model: IPromotionTypesListModel): PromotionTypeReferenceModel;
export function asPromotionTypeReference(model: IPromotionTypeDetailsModel): PromotionTypeReferenceModel;
export function asPromotionTypeReference(model: IPromotionTypeReferenceModel): PromotionTypeReferenceModel {
  return new PromotionTypeReferenceModel({
    id: model.id,
    name: model.name,
  });
}
