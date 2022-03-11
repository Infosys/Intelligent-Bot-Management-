import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AnomalyConfigComponent } from './anomaly-config.component';

describe('AnomalyConfigComponent', () => {
  let component: AnomalyConfigComponent;
  let fixture: ComponentFixture<AnomalyConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AnomalyConfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AnomalyConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
