import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RemediationPlanComponent } from './remediation-plan.component';

describe('RemediationPlanComponent', () => {
  let component: RemediationPlanComponent;
  let fixture: ComponentFixture<RemediationPlanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RemediationPlanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RemediationPlanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
