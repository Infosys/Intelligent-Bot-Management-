import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AutoConfigurationComponent } from './auto-configuration.component';

describe('MetaConfigurationComponent', () => {
  let component: AutoConfigurationComponent;
  let fixture: ComponentFixture<AutoConfigurationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AutoConfigurationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AutoConfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
 