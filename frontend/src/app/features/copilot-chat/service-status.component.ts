import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-service-status',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span *ngIf="source" class="status-badge" [ngClass]="getBadgeClass()">
      <span class="icon">{{ getIcon() }}</span>
      <span class="text">{{ source }}</span>
    </span>
  `,
  styles: [`
    .status-badge {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 4px 10px;
      border-radius: 12px;
      font-size: 0.75rem;
      font-weight: 600;
      margin-top: 8px;
    }
    
    .badge-rag { background: rgba(14, 165, 233, 0.15); color: #38bdf8; border: 1px solid rgba(14, 165, 233, 0.3); }
    .badge-mcp { background: rgba(16, 185, 129, 0.15); color: #10b981; border: 1px solid rgba(16, 185, 129, 0.3); }
    .badge-workflow { background: rgba(245, 158, 11, 0.15); color: #f59e0b; border: 1px solid rgba(245, 158, 11, 0.3); }
    .badge-default { background: rgba(255, 255, 255, 0.1); color: #94a3b8; }
  `]
})
export class ServiceStatusComponent {
  @Input() source?: string;

  getBadgeClass(): string {
    if (!this.source) return 'badge-default';
    const s = this.source.toLowerCase();
    if (s.includes('rag')) return 'badge-rag';
    if (s.includes('mcp')) return 'badge-mcp';
    if (s.includes('workflow')) return 'badge-workflow';
    return 'badge-default';
  }

  getIcon(): string {
    if (!this.source) return '🤖';
    const s = this.source.toLowerCase();
    if (s.includes('rag')) return '📚';
    if (s.includes('mcp')) return '🏥';
    if (s.includes('workflow')) return '⚙️';
    return '🤖';
  }
}
