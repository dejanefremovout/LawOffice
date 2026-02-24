import { Injectable, inject, signal, computed, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL, API_ENDPOINTS } from '../constants/api.constants';
import { Office } from '../models/office.model';
import { Lawyer } from '../models/lawyer.model';

/**
 * Service for managing the global office ID.
 * The office ID is persisted in a cookie and accessible from all pages.
 */
@Injectable({
  providedIn: 'root'
})
export class OfficeService {
  private readonly COOKIE_NAME = 'officeId';
  private readonly COOKIE_EXPIRY_DAYS = 365;
  private http = inject(HttpClient);

  // Signal to store the office ID
  private officeIdSignal = signal<string | null>(this.getOfficeIdFromCookie());

  // Public readonly signal for accessing the office ID
  readonly officeId = this.officeIdSignal.asReadonly();

  // Computed signal to check if office ID is set
  readonly hasOfficeId = computed(() => this.officeId() !== null);

  constructor() {
    // Effect to automatically save office ID to cookie when it changes
    effect(() => {
      const id = this.officeId();
      if (id !== null) {
        this.saveOfficeIdToCookie(id);
      } else {
        this.deleteOfficeIdCookie();
      }
    });
  }

  /**
   * Sets the office ID. This will update the signal and save to cookie.
   */
  setOfficeId(id: string): void {
    this.officeIdSignal.set(id);
  }

  /**
   * Clears the office ID. This will update the signal and remove the cookie.
   */
  clearOfficeId(): void {
    this.officeIdSignal.set(null);
  }

  /**
   * Retrieves the office ID from the cookie.
   */
  private getOfficeIdFromCookie(): string | null {
    if (typeof document === 'undefined') {
      return null; // SSR compatibility
    }

    const name = this.COOKIE_NAME + '=';
    const decodedCookie = decodeURIComponent(document.cookie);
    const cookieArray = decodedCookie.split(';');

    for (let i = 0; i < cookieArray.length; i++) {
      let cookie = cookieArray[i];
      while (cookie.charAt(0) === ' ') {
        cookie = cookie.substring(1);
      }
      if (cookie.indexOf(name) === 0) {
        return cookie.substring(name.length, cookie.length);
      }
    }
    return null;
  }

  /**
   * Saves the office ID to a cookie with expiration.
   */
  private saveOfficeIdToCookie(id: string): void {
    if (typeof document === 'undefined') {
      return; // SSR compatibility
    }

    const date = new Date();
    date.setTime(date.getTime() + (this.COOKIE_EXPIRY_DAYS * 24 * 60 * 60 * 1000));
    const expires = 'expires=' + date.toUTCString();
    document.cookie = `${this.COOKIE_NAME}=${encodeURIComponent(id)};${expires};path=/;SameSite=Strict`;
  }

  /**
   * Deletes the office ID cookie.
   */
  private deleteOfficeIdCookie(): void {
    if (typeof document === 'undefined') {
      return; // SSR compatibility
    }

    document.cookie = `${this.COOKIE_NAME}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;SameSite=Strict`;
  }

  /**
   * Get a specific office by ID
   */
  getOffice(officeId: string): Observable<Office> {
    const url = `${API_BASE_URL.OFFICE_MANAGEMENT}${API_ENDPOINTS.GET_OFFICE(officeId)}`;
    return this.http.get<Office>(url);
  }
  
  /**
   * Create a new office
   */
  createOffice(office: Omit<Office, 'id'>): Observable<Office> {
    const url = `${API_BASE_URL.OFFICE_MANAGEMENT}${API_ENDPOINTS.CREATE_OFFICE}`;
    return this.http.post<Office>(url, office);
  }

    /**
   * Update an existing office
   */
  updateOffice(office: Office): Observable<Office> {
    const url = `${API_BASE_URL.OFFICE_MANAGEMENT}${API_ENDPOINTS.UPDATE_OFFICE}`;
    return this.http.put<Office>(url, office);
  }
}
