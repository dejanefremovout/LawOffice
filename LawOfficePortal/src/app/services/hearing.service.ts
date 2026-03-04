import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../constants/api.constants';
import { Hearing } from '../models/hearing.model';

@Injectable({
  providedIn: 'root'
})
export class HearingService {
  private http = inject(HttpClient);

  /**
   * Get all hearings for a specific case
   */
  getHearings(caseId: string): Observable<Hearing[]> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}/hearing/case/${caseId}`;
    return this.http.get<Hearing[]>(url);
  }

  /**
   * Get a specific hearing by ID
   */
  getHearing(hearingId: string): Observable<Hearing> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}/hearing/${hearingId}`;
    return this.http.get<Hearing>(url);
  }

  /**
   * Create a new hearing
   */
  createHearing(hearing: Omit<Hearing, 'id'>): Observable<Hearing> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}/hearing`;
    return this.http.post<Hearing>(url, hearing);
  }

    /**
   * Update an existing hearing
   */
  updateHearing(hearing: Hearing): Observable<Hearing> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}/hearing`;
    return this.http.put<Hearing>(url, hearing);
  }
}
