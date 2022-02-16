import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActionConfigurationComponent } from './action-configuration.component';

describe('ActionConfigurationComponent', () => {
  let component: ActionConfigurationComponent;
  let fixture: ComponentFixture<ActionConfigurationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActionConfigurationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActionConfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
