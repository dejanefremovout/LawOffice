import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL, API_ENDPOINTS } from '../constants/api.constants';
import { Case } from '../models/case.model';
import { CaseCount } from '../models/case-count.model';
import { CaseHearing } from '../models/case-hearing.model';

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

  /**
   * Get the count of cases
   */
  getCount(): Observable<CaseCount> {
      const url = `${API_BASE_URL.CASE_MANAGEMENT}${API_ENDPOINTS.COUNT_CASES}`;
      return this.http.get<CaseCount>(url);
    }

  /**
   * Get last cases for a specific office
   */
  getLastCases(count: number): Observable<Case[]> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}${API_ENDPOINTS.LAST_CASES(count)}`;
    return this.http.get<Case[]>(url);
  }

  /**
   * Get cases with upcoming hearings
   */
  getUpcomingHearings(count: number): Observable<CaseHearing[]> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}${API_ENDPOINTS.UPCOMING_HEARINGS(count)}`;
    return this.http.get<CaseHearing[]>(url);
  }
}
