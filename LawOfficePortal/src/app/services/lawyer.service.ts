import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../constants/api.constants';
import { Lawyer } from '../models/lawyer.model';

@Injectable({
  providedIn: 'root'
})
export class LawyerService {
  private http = inject(HttpClient);

  /**
   * Get all lawyers for a specific case
   */
  getLawyers(officeId: string): Observable<Lawyer[]> {
    const url = `${API_BASE_URL.OFFICE_MANAGEMENT}/lawyer/${officeId}`;
    return this.http.get<Lawyer[]>(url);
  }

  /**
   * Get a specific lawyer by ID
   */
  getLawyer(officeId: string, lawyerId: string): Observable<Lawyer> {
    const url = `${API_BASE_URL.OFFICE_MANAGEMENT}/lawyer/${officeId}/${lawyerId}`;
    return this.http.get<Lawyer>(url);
  }

  /**
   * Create a new lawyer
   */
  createLawyer(lawyer: Omit<Lawyer, 'id'>): Observable<Lawyer> {
    const url = `${API_BASE_URL.OFFICE_MANAGEMENT}/lawyer`;
    return this.http.post<Lawyer>(url, lawyer);
  }

    /**
   * Update an existing lawyer
   */
  updateLawyer(lawyer: Lawyer): Observable<Lawyer> {
    const url = `${API_BASE_URL.OFFICE_MANAGEMENT}/lawyer`;
    return this.http.put<Lawyer>(url, lawyer);
  }
}
