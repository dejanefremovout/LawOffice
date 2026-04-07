import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ClientService as GeneratedClientService } from '../api/party-management';
import type { PartyModel, PartyCreateModel } from '../api/party-management';
import { Client } from '../models/client.model';

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private api = inject(GeneratedClientService);

  getClients(): Observable<Client[]> {
    return this.api.getAllClients({ xOfficeId: '' }) as Observable<Client[]>;
  }

  getClient(clientId: string): Observable<Client> {
    return this.api.getClient({ xOfficeId: '', clientId }) as Observable<Client>;
  }

  createClient(client: Omit<Client, 'id'>): Observable<Client> {
    return this.api.createClient({ xOfficeId: '', requestBody: client as PartyCreateModel }) as Observable<Client>;
  }

  updateClient(client: Client): Observable<Client> {
    return this.api.updateClient({ xOfficeId: '', requestBody: client as PartyModel }) as Observable<Client>;
  }
}
