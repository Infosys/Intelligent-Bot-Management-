import { Component, Input, EventEmitter, OnChanges } from '@angular/core';

import { Grid } from '../../../lib/grid';
import { Row } from '../../../lib/data-set/row';

@Component({
  selector: 'ng2-st-tbody-create-cancel',
  template: `
  <div class="centerActions">
    <button type="button" class="ng2-smart-action ng2-smart-action-edit-save btn btn-sm btn-primary"
        (click)="onSave($event)"><span class="glyphicon glyphicon-ok-circle" style="color:white;"></span></button>
    <button type="button" class="ng2-smart-action ng2-smart-action-edit-cancel btn btn-sm btn-danger"
        (click)="onCancelEdit($event)"><span class="glyphicon glyphicon-remove-circle" style="color:white;"></span></button>
  </div>
  `,
})
export class TbodyCreateCancelComponent implements OnChanges {

  @Input() grid: Grid;
  @Input() row: Row;
  @Input() editConfirm: EventEmitter<any>;

  cancelButtonContent: string;
  saveButtonContent: string;

  onSave(event: any) {
    event.preventDefault();
    event.stopPropagation();

    this.grid.save(this.row, this.editConfirm);
  }

  onCancelEdit(event: any) {
    event.preventDefault();
    event.stopPropagation();

    this.row.isInEditing = false;
  }

  ngOnChanges() {
    this.saveButtonContent = this.grid.getSetting('edit.saveButtonContent');
    this.cancelButtonContent = this.grid.getSetting('edit.cancelButtonContent')
  }
}
