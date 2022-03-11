import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RemediationConfigComponent } from './remediation-config.component';

describe('RemediationConfigComponent', () => {
  let component: RemediationConfigComponent;
  let fixture: ComponentFixture<RemediationConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RemediationConfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RemediationConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
