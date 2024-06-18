import {ChangeDetectionStrategy, Component, inject, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {ApiResult} from "../../../@shared/utils/api-result";
import {DetailsModel} from "../../../@core/app-api";
import {NullReplacerModule} from "../../../@shared/display-substitution/null-replacer";
import {PanelModule} from "primeng/panel";
import {DetailsListModule} from "../../../@shared/details-list/details-list.module";

@Component({
  standalone: true,
  imports: [NullReplacerModule, PanelModule, DetailsListModule],
  templateUrl: './view-supplier.component.html',
  styleUrl: './view-supplier.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ViewSupplierComponent implements OnInit {
  private readonly activatedRoute = inject(ActivatedRoute);

  protected item!: DetailsModel;

  ngOnInit() {
    const supplierResult = this.activatedRoute.snapshot.data['item'] as ApiResult<DetailsModel>;
    this.item = supplierResult.value!;
  }
}
