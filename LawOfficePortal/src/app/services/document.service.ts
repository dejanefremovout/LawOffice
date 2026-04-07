import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DocumentFileService as GeneratedDocumentFileService } from '../api/case-management';
import type { DocumentFileModel, DocumentFileCreateModel } from '../api/case-management';
import { Document } from '../models/document.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private http = inject(HttpClient);
  private api = inject(GeneratedDocumentFileService);

  getDocuments(caseId: string): Observable<Document[]> {
    return this.api.getAllDocumentFiles({ xOfficeId: '', caseId }) as Observable<Document[]>;
  }

  getDocument(documentId: string): Observable<Document> {
    return this.api.getDocumentFile({ xOfficeId: '', documentFileId: documentId }) as Observable<Document>;
  }

  createDocument(document: Omit<Document, 'id' | 'uri'>): Observable<Document> {
    return this.api.createDocumentFile({ xOfficeId: '', requestBody: document as DocumentFileCreateModel }) as Observable<Document>;
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
