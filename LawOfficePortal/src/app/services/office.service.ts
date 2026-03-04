import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL, API_ENDPOINTS } from '../constants/api.constants';
import { Office } from '../models/office.model';

@Injectable({
  providedIn: 'root'
})
export class OfficeService {
  private http = inject(HttpClient);

  /**
   * Get office details
   */
  getOffice(): Observable<Office> {
    const url = `${API_BASE_URL.OFFICE_MANAGEMENT}${API_ENDPOINTS.GET_OFFICE}`;
    return this.http.get<Office>(url);
  }
  
  /**
   * Update an existing office
   */
  updateOffice(office: Office): Observable<Office> {
    const url = `${API_BASE_URL.OFFICE_MANAGEMENT}${API_ENDPOINTS.UPDATE_OFFICE}`;
    return this.http.put<Office>(url, office);
  }
}
