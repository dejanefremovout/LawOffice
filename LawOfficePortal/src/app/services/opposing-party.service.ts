import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL, API_ENDPOINTS } from '../constants/api.constants';
import { OpposingParty } from '../models/opposing-party.model';

@Injectable({
  providedIn: 'root'
})
export class OpposingPartyService {
  private http = inject(HttpClient);

  /**
   * Get all opposing parties for a specific office
   */
  getOpposingParties(): Observable<OpposingParty[]> {
    const url = `${API_BASE_URL.PARTY_MANAGEMENT}${API_ENDPOINTS.GET_OPPOSING_PARTIES}`;
    return this.http.get<OpposingParty[]>(url);
  }

  /**
   * Get a specific opposing party by ID
   */
  getOpposingParty(opposingPartyId: string): Observable<OpposingParty> {
    const url = `${API_BASE_URL.PARTY_MANAGEMENT}${API_ENDPOINTS.GET_OPPOSING_PARTY(opposingPartyId)}`;
    return this.http.get<OpposingParty>(url);
  }

  /**
   * Create a new opposing party
   */
  createOpposingParty(opposingParty: Omit<OpposingParty, 'id'>): Observable<OpposingParty> {
    const url = `${API_BASE_URL.PARTY_MANAGEMENT}${API_ENDPOINTS.CREATE_OPPOSING_PARTY}`;
    return this.http.post<OpposingParty>(url, opposingParty);
  }

  /**
   * Update an existing opposing party
   */
  updateOpposingParty(opposingParty: OpposingParty): Observable<OpposingParty> {
    const url = `${API_BASE_URL.PARTY_MANAGEMENT}${API_ENDPOINTS.UPDATE_OPPOSING_PARTY}`;
    return this.http.put<OpposingParty>(url, opposingParty);
  }
}
