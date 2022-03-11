import { Component, Input, OnChanges } from '@angular/core';

import { Grid } from '../../../lib/grid';
import { DataSource } from '../../../lib/data-source/data-source';

@Component({
  selector: '[ng2-st-checkbox-select-all]',
  template: `
  
    <input type="checkbox" [ngModel]="isAllSelected" *ngIf="isCheckboxVisible">
  
  `,
})
export class CheckboxSelectAllComponent implements OnChanges  {

  @Input() grid: Grid;
  @Input() source: DataSource;
  @Input() isAllSelected: boolean;
  @Input() hideProperty: any;

  isCheckboxVisible : boolean = true;

  ngOnChanges() {
      // if(this.hideProperty && this.hideProperty.isHideCheckbox){
    if((this.hideProperty && this.hideProperty.isHideCheckbox) || this.grid.isRadioSelectVisible()){
        this.isCheckboxVisible = false;
    } 
  }
}
