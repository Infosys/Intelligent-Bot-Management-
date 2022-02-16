import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MetaConfigurationComponent } from './meta-configuration.component';

describe('MetaConfigurationComponent', () => {
  let component: MetaConfigurationComponent;
  let fixture: ComponentFixture<MetaConfigurationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MetaConfigurationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MetaConfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
