import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';

export interface ChatMessage {
  text: string;
  sender: 'user' | 'ai';
  sourceService?: string; // e.g., 'RAG', 'MCP', 'Workflow'
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection: signalR.HubConnection;
  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  public messages$ = this.messagesSubject.asObservable();

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/orchestrator/chatHub') // Going through API Gateway
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveMessage', (message: string, sourceService: string) => {
      this.addMessage({ text: message, sender: 'ai', sourceService });
    });
  }

  public async startConnection(): Promise<void> {
    try {
      await this.hubConnection.start();
      console.log('SignalR connected to Orchestrator API');
    } catch (err) {
      console.error('Error connecting to SignalR', err);
      setTimeout(() => this.startConnection(), 5000);
    }
  }

  public async sendMessage(userId: string, role: string, message: string): Promise<void> {
    this.addMessage({ text: message, sender: 'user' });
    try {
      await this.hubConnection.invoke('SendMessage', userId, role, message);
    } catch (err) {
      console.error('Error sending message', err);
    }
  }

  private addMessage(msg: ChatMessage) {
    const current = this.messagesSubject.getValue();
    this.messagesSubject.next([...current, msg]);
  }
}
