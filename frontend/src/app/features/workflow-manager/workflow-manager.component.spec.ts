import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkflowManagerComponent } from './workflow-manager.component';

describe('WorkflowManagerComponent', () => {
  let component: WorkflowManagerComponent;
  let fixture: ComponentFixture<WorkflowManagerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkflowManagerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WorkflowManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
