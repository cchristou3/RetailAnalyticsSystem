import {wrapResolverApiResult} from "../../@shared/utils/api-result";
import {inject} from "@angular/core";
import {ProductsClient} from "../app-api";

export const resolveProductsList = wrapResolverApiResult(() => inject(ProductsClient).list());

export const resolveProductsDropdown = wrapResolverApiResult(() => inject(ProductsClient).listForReference());

export const resolveProductDetails = wrapResolverApiResult(
  route => inject(ProductsClient)
    .get(+route.paramMap.get('id')!)
);

export const resolveProductForUpdate = wrapResolverApiResult(
  route => inject(ProductsClient)
    .getForUpdate(+route.paramMap.get('id')!)
);
