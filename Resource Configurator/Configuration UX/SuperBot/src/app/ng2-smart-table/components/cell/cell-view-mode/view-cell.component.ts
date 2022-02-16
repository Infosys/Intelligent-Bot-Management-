import {Component, Input, ChangeDetectionStrategy,EventEmitter,Output } from '@angular/core';

import { Cell } from '../../../lib/data-set/cell';

@Component({
  selector: 'table-cell-view-mode',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div [ngSwitch]="cell.getColumn().type">
        <custom-view-component *ngSwitchCase="'custom'" [cell]="cell"></custom-view-component>
        <div *ngSwitchCase="'html'">
        <span class="view-cell-eventId" data-toggle="tooltip" data-placement="top" title="{{cell.getValue()}}" (click)="onSelectionChanged()">{{cell.getValue()}}</span>
        </div>
        <div *ngSwitchDefault>{{ cell.getValue() }}</div>
    </div>
    `,
})
export class ViewCellComponent {

  @Input() cell: Cell;
  @Output() selectionChanged = new EventEmitter<string>();
  
  onSelectionChanged(event){
  // event.preventDefault();
  // event.stopPropagation();
  this.selectionChanged.emit(this.cell.getValue());
  } 
}
