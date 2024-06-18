import {Routes} from "@angular/router";
import {ViewSupplierComponent} from "./view-supplier/view-supplier.component";
import {resolveSupplierDetails} from "../../@core/suppliers/suppliers.resolvers";
import {ManageSupplierComponent} from "./manage-supplier/manage-supplier.component";

export default [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'add',
  },
  {
    path: 'add',
    component: ManageSupplierComponent,
  },
  {
    path: 'view/:id',
    component: ViewSupplierComponent,
    resolve: {
      item: resolveSupplierDetails,
    },
  },
] as Routes;
