/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { DocumentSummaryModel } from '../models/DocumentSummaryModel';
import type { DocumentSummaryRequestModel } from '../models/DocumentSummaryRequestModel';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
  providedIn: 'root',
})
export class DocumentSummaryService {
  constructor(public readonly http: HttpClient) {}
  /**
   * Create an AI summary for a document
   * @returns DocumentSummaryModel Generated summary
   * @throws ApiError
   */
  public createDocumentSummary({
    xOfficeId,
    documentFileId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Document file identifier
     */
    documentFileId: string,
    requestBody?: DocumentSummaryRequestModel,
  }): Observable<DocumentSummaryModel> {
    return __request(OpenAPI, this.http, {
      method: 'POST',
      url: '/documentFile/{documentFileId}/summary',
      path: {
        'documentFileId': documentFileId,
      },
      headers: {
        'X-Office-Id': xOfficeId,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        400: `Invalid request`,
        413: `Input content too large`,
        429: `Daily AI quota exceeded`,
      },
    });
  }
}
