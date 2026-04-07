/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { DocumentFileCreateModel } from '../models/DocumentFileCreateModel';
import type { DocumentFileModel } from '../models/DocumentFileModel';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
  providedIn: 'root',
})
export class DocumentFileService {
  constructor(public readonly http: HttpClient) {}
  /**
   * Get a document file by ID
   * @returns DocumentFileModel The requested document file
   * @throws ApiError
   */
  public getDocumentFile({
    xOfficeId,
    documentFileId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Document file identifier
     */
    documentFileId: string,
  }): Observable<DocumentFileModel> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/documentFile/{documentFileId}',
      path: {
        'documentFileId': documentFileId,
      },
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
        404: `Document file not found`,
      },
    });
  }
  /**
   * Delete a document file
   * @returns void
   * @throws ApiError
   */
  public deleteDocumentFile({
    xOfficeId,
    documentFileId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Document file identifier
     */
    documentFileId: string,
  }): Observable<void> {
    return __request(OpenAPI, this.http, {
      method: 'DELETE',
      url: '/documentFile/{documentFileId}',
      path: {
        'documentFileId': documentFileId,
      },
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
      },
    });
  }
  /**
   * Get all document files for a case
   * @returns DocumentFileModel List of document files
   * @throws ApiError
   */
  public getAllDocumentFiles({
    xOfficeId,
    caseId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Case identifier
     */
    caseId: string,
  }): Observable<Array<DocumentFileModel>> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/documentFile/case/{caseId}',
      path: {
        'caseId': caseId,
      },
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
      },
    });
  }
  /**
   * Create a document file
   * @returns DocumentFileModel Created document file
   * @throws ApiError
   */
  public createDocumentFile({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: DocumentFileCreateModel,
  }): Observable<DocumentFileModel> {
    return __request(OpenAPI, this.http, {
      method: 'POST',
      url: '/documentFile',
      headers: {
        'X-Office-Id': xOfficeId,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        400: `Invalid request`,
      },
    });
  }
  /**
   * Update a document file
   * @returns DocumentFileModel Updated document file
   * @throws ApiError
   */
  public updateDocumentFile({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: DocumentFileModel,
  }): Observable<DocumentFileModel> {
    return __request(OpenAPI, this.http, {
      method: 'PUT',
      url: '/documentFile',
      headers: {
        'X-Office-Id': xOfficeId,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        400: `Invalid request`,
      },
    });
  }
}
