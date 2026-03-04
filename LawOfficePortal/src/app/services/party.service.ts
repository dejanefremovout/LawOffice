import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../constants/api.constants';
import { PartyCount } from '../models/party-count.model';

@Injectable({
  providedIn: 'root'
})
export class PartyService {
  private http = inject(HttpClient);

  getCount(): Observable<PartyCount> {
    const url = `${API_BASE_URL.PARTY_MANAGEMENT}/party/count`;
    return this.http.get<PartyCount>(url);
  }
}