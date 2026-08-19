import { Component, OnInit, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService, ChatMessage } from '../../core/services/chat.service';
import { ServiceStatusComponent } from './service-status.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-copilot-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, ServiceStatusComponent],
  templateUrl: './copilot-chat.component.html',
  styleUrl: './copilot-chat.component.css'
})
export class CopilotChatComponent implements OnInit, AfterViewChecked {
  @ViewChild('scrollContainer') private scrollContainer!: ElementRef;
  
  messages$: Observable<ChatMessage[]>;
  newMessage: string = '';

  constructor(private chatService: ChatService) {
    this.messages$ = this.chatService.messages$;
  }

  ngOnInit(): void {
    this.chatService.startConnection();
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  private scrollToBottom(): void {
    try {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    } catch(err) { }
  }

  sendMessage(): void {
    if (!this.newMessage.trim()) return;
    
    // Hardcoded for mockup purposes, normally comes from Auth
    const userId = "JohnDoe123";
    const role = "doctor";
    
    this.chatService.sendMessage(userId, role, this.newMessage);
    this.newMessage = '';
  }
}
