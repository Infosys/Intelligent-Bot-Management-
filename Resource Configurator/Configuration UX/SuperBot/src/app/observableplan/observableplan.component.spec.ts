import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ObservableplanComponent } from './observableplan.component';

describe('ObservableplanComponent', () => {
  let component: ObservableplanComponent;
  let fixture: ComponentFixture<ObservableplanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ObservableplanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ObservableplanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
