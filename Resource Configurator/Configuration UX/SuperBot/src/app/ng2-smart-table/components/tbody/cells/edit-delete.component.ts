import {Component, Input, Output, EventEmitter, OnChanges, ChangeDetectionStrategy } from '@angular/core';

import { Grid } from '../../../lib/grid';
import { Row } from '../../../lib/data-set/row';
import { DataSource } from '../../../lib/data-source/data-source';

@Component({
  selector: 'ng2-st-tbody-edit-delete',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
  <div class="centerActions">
    <button type="button"  *ngIf="isActionEdit" class="ng2-smart-action ng2-smart-action-edit-edit btn btn-sm btn-primary"
         (click)="onEdit($event)"><span class="glyphicon glyphicon-edit" style="color:white;"></span></button>
    <button type="button" *ngIf="isActionDelete" class="ng2-smart-action ng2-smart-action-delete-delete btn btn-sm btn-primary"
         (click)="onDelete($event)"><span class="glyphicon glyphicon-trash" style="color:white;"></span></button>
  </div>
  `,
})
export class TbodyEditDeleteComponent implements OnChanges {

  @Input() grid: Grid;
  @Input() row: Row;
  @Input() source: DataSource;
  @Input() deleteConfirm: EventEmitter<any>;
  @Input() editConfirm: EventEmitter<any>;

  @Output() edit = new EventEmitter<any>();
  @Output() delete = new EventEmitter<any>();
  @Output() editRowSelect = new EventEmitter<any>();

  isActionEdit: boolean;
  isActionDelete: boolean;
  editRowButtonContent: string;
  deleteRowButtonContent: string;

 
  onEdit(event: any) {
    event.preventDefault();
    event.stopPropagation();
    
    this.editRowSelect.emit(this.row);
    
    if (this.grid.getSetting('mode') === 'external') {
    //this.grid.edit(this.row);
    this.edit.emit({
    data: this.row.getData(),
    source: this.source,
    });
    } else {
    this.grid.edit(this.row);
    }
    } 

  onDelete(event: any) {
    event.preventDefault();
    event.stopPropagation();

    if (this.grid.getSetting('mode') === 'external') {
      this.delete.emit({
        data: this.row.getData(),
        source: this.source,
      });
    } else {
      this.grid.delete(this.row, this.deleteConfirm);
    }
  }

  ngOnChanges(){
    this.isActionEdit = this.grid.getSetting('actions.edit');
    this.isActionDelete = this.grid.getSetting('actions.delete');
    this.editRowButtonContent = this.grid.getSetting('edit.editButtonContent');
    this.deleteRowButtonContent = this.grid.getSetting('delete.deleteButtonContent');
  }
}
