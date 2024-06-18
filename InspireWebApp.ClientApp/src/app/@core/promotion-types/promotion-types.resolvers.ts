import {wrapResolverApiResult} from "../../@shared/utils/api-result";
import {inject} from "@angular/core";
import {PromotionTypesClient} from "../app-api";

export const resolvePromotionTypesList = wrapResolverApiResult(() => inject(PromotionTypesClient).list());

export const resolvePromotionTypesDropdown = wrapResolverApiResult(() => inject(PromotionTypesClient).listForReference());

export const resolvePromotionTypeDetails = wrapResolverApiResult(
  route => inject(PromotionTypesClient)
    .get(+route.paramMap.get('id')!)
);

export const resolvePromotionTypeForUpdate = wrapResolverApiResult(
  route => inject(PromotionTypesClient)
    .getForUpdate(+route.paramMap.get('id')!)
);
