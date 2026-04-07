/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { CaseCountModel } from '../models/CaseCountModel';
import type { CaseCreateModel } from '../models/CaseCreateModel';
import type { CaseHearingModel } from '../models/CaseHearingModel';
import type { CaseModel } from '../models/CaseModel';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
  providedIn: 'root',
})
export class CaseService {
  constructor(public readonly http: HttpClient) {}
  /**
   * Get a case by ID
   * @returns CaseModel The requested case
   * @throws ApiError
   */
  public getCase({
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
  }): Observable<CaseModel> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/case/{caseId}',
      path: {
        'caseId': caseId,
      },
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
        404: `Case not found`,
      },
    });
  }
  /**
   * Delete a case
   * @returns void
   * @throws ApiError
   */
  public deleteCase({
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
  }): Observable<void> {
    return __request(OpenAPI, this.http, {
      method: 'DELETE',
      url: '/case/{caseId}',
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
   * Get all cases
   * @returns CaseModel List of cases
   * @throws ApiError
   */
  public getAllCases({
    xOfficeId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
  }): Observable<Array<CaseModel>> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/case',
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
      },
    });
  }
  /**
   * Create a case
   * @returns CaseModel Created case
   * @throws ApiError
   */
  public createCase({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: CaseCreateModel,
  }): Observable<CaseModel> {
    return __request(OpenAPI, this.http, {
      method: 'POST',
      url: '/case',
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
   * Update a case
   * @returns CaseModel Updated case
   * @throws ApiError
   */
  public updateCase({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: CaseModel,
  }): Observable<CaseModel> {
    return __request(OpenAPI, this.http, {
      method: 'PUT',
      url: '/case',
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
   * Get case count aggregates
   * @returns CaseCountModel Case count aggregates
   * @throws ApiError
   */
  public getCaseCount({
    xOfficeId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
  }): Observable<CaseCountModel> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/cases/count',
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
      },
    });
  }
  /**
   * Get last N cases
   * @returns CaseModel Last N cases
   * @throws ApiError
   */
  public getLastCases({
    xOfficeId,
    count,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Number of cases to return
     */
    count: number,
  }): Observable<Array<CaseModel>> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/cases/last/{count}',
      path: {
        'count': count,
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
   * Get cases with upcoming hearings
   * @returns CaseHearingModel Cases with upcoming hearings
   * @throws ApiError
   */
  public getCasesWithHearings({
    xOfficeId,
    count,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Number of hearings to return
     */
    count: number,
  }): Observable<Array<CaseHearingModel>> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/cases/hearings/{count}',
      path: {
        'count': count,
      },
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
      },
    });
  }
}
