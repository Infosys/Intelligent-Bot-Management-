import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivateResourceComponent } from './activate-resource.component';

describe('ActivateResourceComponent', () => {
  let component: ActivateResourceComponent;
  let fixture: ComponentFixture<ActivateResourceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActivateResourceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActivateResourceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
