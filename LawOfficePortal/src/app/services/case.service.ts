import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL, API_ENDPOINTS } from '../constants/api.constants';
import { Case } from '../models/case.model';

@Injectable({
  providedIn: 'root'
})
export class CaseService {
  private http = inject(HttpClient);

  /**
   * Get all cases for a specific office
   */
  getCases(): Observable<Case[]> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}${API_ENDPOINTS.GET_CASES}`;
    return this.http.get<Case[]>(url);
  }

  /**
   * Get a specific case by ID
   */
  getCase(caseId: string): Observable<Case> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}${API_ENDPOINTS.GET_CASE(caseId)}`;
    return this.http.get<Case>(url);
  }

  /**
   * Create a new case
   */
  createCase(caseData: Omit<Case, 'id'>): Observable<Case> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}${API_ENDPOINTS.CREATE_CASE}`;
    return this.http.post<Case>(url, caseData);
  }

  /**
   * Update an existing case
   */
  updateCase(caseData: Case): Observable<Case> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}${API_ENDPOINTS.UPDATE_CASE}`;
    return this.http.put<Case>(url, caseData);
  }
}
