import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Patient360Component } from './patient360.component';

describe('Patient360Component', () => {
  let component: Patient360Component;
  let fixture: ComponentFixture<Patient360Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Patient360Component]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(Patient360Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
