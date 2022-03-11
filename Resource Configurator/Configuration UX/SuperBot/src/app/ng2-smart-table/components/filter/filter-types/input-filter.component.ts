import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import 'rxjs/add/operator/distinctUntilChanged';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/skip';

import { DefaultFilter } from './default-filter';

@Component({
  selector: 'input-filter',
  template: `
  <div style="position:relative;">
    <input style="padding-left:15%;" [(ngModel)]="query"
           [ngClass]="inputClass"
           [formControl]="inputControl"
           class="form-control"
           type="text"
           placeholder=" Search {{ column.title }}" />
    <span class="glyphicon glyphicon-search search-events"></span>
  </div>
  `,
})
export class InputFilterComponent extends DefaultFilter implements OnInit {

  inputControl = new FormControl();

  constructor() {
    super();
  }

  ngOnInit() {
    this.inputControl.valueChanges
      .skip(1)
      .distinctUntilChanged()
      .debounceTime(this.delay)
      .subscribe((value: string) => this.setFilter());
  }
}
