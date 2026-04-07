/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { HearingCreateModel } from '../models/HearingCreateModel';
import type { HearingModel } from '../models/HearingModel';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
  providedIn: 'root',
})
export class HearingService {
  constructor(public readonly http: HttpClient) {}
  /**
   * Get a hearing by ID
   * @returns HearingModel The requested hearing
   * @throws ApiError
   */
  public getHearing({
    xOfficeId,
    hearingId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Hearing identifier
     */
    hearingId: string,
  }): Observable<HearingModel> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/hearing/{hearingId}',
      path: {
        'hearingId': hearingId,
      },
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
        404: `Hearing not found`,
      },
    });
  }
  /**
   * Delete a hearing
   * @returns void
   * @throws ApiError
   */
  public deleteHearing({
    xOfficeId,
    hearingId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Hearing identifier
     */
    hearingId: string,
  }): Observable<void> {
    return __request(OpenAPI, this.http, {
      method: 'DELETE',
      url: '/hearing/{hearingId}',
      path: {
        'hearingId': hearingId,
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
   * Get all hearings for a case
   * @returns HearingModel List of hearings
   * @throws ApiError
   */
  public getAllHearings({
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
  }): Observable<Array<HearingModel>> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/hearing/case/{caseId}',
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
   * Create a hearing
   * @returns HearingModel Created hearing
   * @throws ApiError
   */
  public createHearing({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: HearingCreateModel,
  }): Observable<HearingModel> {
    return __request(OpenAPI, this.http, {
      method: 'POST',
      url: '/hearing',
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
   * Update a hearing
   * @returns HearingModel Updated hearing
   * @throws ApiError
   */
  public updateHearing({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: HearingModel,
  }): Observable<HearingModel> {
    return __request(OpenAPI, this.http, {
      method: 'PUT',
      url: '/hearing',
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
