import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL, API_ENDPOINTS } from '../constants/api.constants';
import { Client } from '../models/client.model';

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private http = inject(HttpClient);

  /**
   * Get all clients for a specific office
   */
  getClients(officeId: string): Observable<Client[]> {
    const url = `${API_BASE_URL.PARTY_MANAGEMENT}${API_ENDPOINTS.GET_CLIENTS(officeId)}`;
    return this.http.get<Client[]>(url);
  }

  /**
   * Get a specific client by ID
   */
  getClient(officeId: string, clientId: string): Observable<Client> {
    const url = `${API_BASE_URL.PARTY_MANAGEMENT}${API_ENDPOINTS.GET_CLIENT(officeId, clientId)}`;
    return this.http.get<Client>(url);
  }

  /**
   * Create a new client
   */
  createClient(client: Omit<Client, 'id'>): Observable<Client> {
    const url = `${API_BASE_URL.PARTY_MANAGEMENT}${API_ENDPOINTS.CREATE_CLIENT}`;
    return this.http.post<Client>(url, client);
  }

  /**
   * Update an existing client
   */
  updateClient(client: Client): Observable<Client> {
    const url = `${API_BASE_URL.PARTY_MANAGEMENT}${API_ENDPOINTS.UPDATE_CLIENT}`;
    return this.http.put<Client>(url, client);
  }
}
