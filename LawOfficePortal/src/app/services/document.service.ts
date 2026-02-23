import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../constants/api.constants';
import { Document } from '../models/document.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private http = inject(HttpClient);

  /**
   * Get all documents for a specific case
   */
  getDocuments(officeId: string, caseId: string): Observable<Document[]> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}/api/documentFile/${officeId}/case/${caseId}`;
    return this.http.get<Document[]>(url);
  }

  /**
   * Get a specific document by ID
   */
  getDocument(officeId: string, documentId: string): Observable<Document> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}/api/documentFile/${officeId}/${documentId}`;
    return this.http.get<Document>(url);
  }

  /**
   * Create a new document
   */
  createDocument(document: Omit<Document, 'id' | 'uri'>): Observable<Document> {
    const url = `${API_BASE_URL.CASE_MANAGEMENT}/api/documentFile`;
    return this.http.post<Document>(url, document);
  }

  /**
   * Upload a file to Azure Blob Storage using a SAS URI
   */
  uploadFileToBlob(sasUri: string, file: Blob | File, contentType?: string): Observable<any> {
    const headers: Record<string, string> = { 'x-ms-blob-type': 'BlockBlob' };
    if (contentType) {
      headers['Content-Type'] = contentType;
    }
    return this.http.put(sasUri, file, {
      headers: headers,
      reportProgress: true,
      observe: 'events' as const
    });
  }

  /**
   * Download a file from Azure Blob Storage using a SAS URI in the background
   */
  downloadFileFromBlob(sasUri: string, fileName: string): void {
    this.http.get(sasUri, { responseType: 'blob' }).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('File download failed', err);
      }
    });
  }
}
