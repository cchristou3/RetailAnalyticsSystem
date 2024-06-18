import {wrapResolverApiResult} from "../../@shared/utils/api-result";
import {inject} from "@angular/core";
import {ProductCategoriesClient} from "../app-api";

export const resolveProductCategoriesList = wrapResolverApiResult(() => inject(ProductCategoriesClient).list());

export const resolveProductCategoriesDropdown = wrapResolverApiResult(() => inject(ProductCategoriesClient).listForReference());

export const resolveProductCategoryDetails = wrapResolverApiResult(
  route => inject(ProductCategoriesClient)
    .get(+route.paramMap.get('id')!)
);

export const resolveProductCategoryForUpdate = wrapResolverApiResult(
  route => inject(ProductCategoriesClient)
    .getForUpdate(+route.paramMap.get('id')!)
);
