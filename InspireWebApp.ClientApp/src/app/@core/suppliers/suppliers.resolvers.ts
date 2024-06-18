import {inject} from "@angular/core";
import {SuppliersClient} from "../app-api";
import {wrapResolverApiResult} from "../../@shared/utils/api-result";

export const resolveSupplierDetails = wrapResolverApiResult(
  (route) => inject(SuppliersClient)
    .get(+route.paramMap.get('id')!)
);
