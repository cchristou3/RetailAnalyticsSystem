import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {PanelModule} from "primeng/panel";
import {InputTextModule} from "primeng/inputtext";
import {InputTextareaModule} from "primeng/inputtextarea";
import {CalendarModule} from "primeng/calendar";
import {FormControl, FormGroup, ReactiveFormsModule} from "@angular/forms";
import {CommonValidators} from "../../../@shared/utils/common-validators";
import {FormControlErrorsModule} from "../../../@shared/form-control-errors/form-control-errors.module";
import {SupplierCreateModel, SuppliersClient} from "../../../@core/app-api";
import {emptyStringToNull} from "../../../@shared/utils/string.helpers";
import {nativeToLocalDate} from "../../../@shared/date-time/joda.helpers";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  standalone: true,
  imports: [
    PanelModule,
    InputTextModule,
    InputTextareaModule,
    CalendarModule,
    ReactiveFormsModule,
    FormControlErrorsModule,
  ],
  templateUrl: './manage-supplier.component.html',
  styleUrl: './manage-supplier.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ManageSupplierComponent {
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly suppliersClient = inject(SuppliersClient);

  form = new FormGroup({
    name: new FormControl<string>('', {
      validators: [
        CommonValidators.required(),
        CommonValidators.maxLength({value: 100}),
      ],
    }),
    info: new FormControl<string>('', {
      validators: [
        CommonValidators.minLength({value: 5}),
      ],
    }),
    contractStartDate: new FormControl<Date | null>(null, {
      validators: [
        CommonValidators.required(),
      ],
    }),
    contractEndDate: new FormControl<Date | null>(null),
  });

  submitForm() {
    const values = this.form.getRawValue();

    this.suppliersClient
      .create(new SupplierCreateModel({
        name: values.name!,
        info: emptyStringToNull(values.info!),
        contractStartDate: nativeToLocalDate(values.contractStartDate!),
        contractEndDate: nativeToLocalDate(values.contractEndDate!),
      }))
      .subscribe(id => this.router.navigate(['view', id], { relativeTo: this.activatedRoute.parent }));
  }
}
