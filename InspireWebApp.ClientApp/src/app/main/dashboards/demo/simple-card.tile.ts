import {Component} from '@angular/core';
import {CardModule} from "primeng/card";

let instanceCounter = 0;

@Component({
  standalone: true,
  imports: [CardModule],
  template: `
    <p-card>
      Item: {{ instanceId }}
    </p-card>
  `,
  styles: [
    `
      :host {
        display: contents;
      }

      p-card {
        display: block;
        height: 100%;

        ::ng-deep .p-card {
          height: 100%;
        }
      }
    `,
  ]
})
export class SimpleCardComponent {
  readonly instanceId = instanceCounter++;
}
