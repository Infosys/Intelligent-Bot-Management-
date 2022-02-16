import {Component, Input, Output, EventEmitter, OnChanges } from '@angular/core';

import { Grid } from '../../../lib/grid';

@Component({
  selector: 'ng2-st-actions',
  template: `
  <div class="centerActions">
    <button type="button" class="ng2-smart-action ng2-smart-action-add-create btn btn-sm btn-primary"
        (click)="$event.preventDefault();create.emit($event)"><span class="glyphicon glyphicon-ok-circle" style="color:white;"></span></button>
    <button type="button" class="ng2-smart-action ng2-smart-action-add-cancel btn btn-sm btn-danger"
        (click)="$event.preventDefault();grid.createFormShown = false;"><span class="glyphicon glyphicon-remove-circle" style="color:white;"></span></button>
  </div>
  `,
    
})
export class ActionsComponent implements OnChanges {

  @Input() grid: Grid;
  @Output() create = new EventEmitter<any>();

  createButtonContent: string;
  cancelButtonContent: string;

  ngOnChanges() {
    this.createButtonContent = this.grid.getSetting('add.createButtonContent');
    this.cancelButtonContent = this.grid.getSetting('add.cancelButtonContent');
  }
}
