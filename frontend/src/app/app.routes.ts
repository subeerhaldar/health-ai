import { Routes } from '@angular/router';
import { Patient360Component } from './features/patient360/patient360.component';
import { WorkflowManagerComponent } from './features/workflow-manager/workflow-manager.component';
import { CopilotChatComponent } from './features/copilot-chat/copilot-chat.component';

export const routes: Routes = [
  { path: '', redirectTo: '/patient-360', pathMatch: 'full' },
  { path: 'patient-360', component: Patient360Component },
  { path: 'workflows', component: WorkflowManagerComponent },
  { path: 'copilot', component: CopilotChatComponent }
];
