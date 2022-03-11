import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ObservableConfigComponent } from './observable-config.component';

describe('ObservableConfigComponent', () => {
  let component: ObservableConfigComponent;
  let fixture: ComponentFixture<ObservableConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ObservableConfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ObservableConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
