/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { OfficeModel } from '../models/OfficeModel';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
  providedIn: 'root',
})
export class OfficeService {
  constructor(public readonly http: HttpClient) {}
  /**
   * Get office details
   * @returns OfficeModel The office
   * @throws ApiError
   */
  public getOffice({
    xOfficeId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
  }): Observable<OfficeModel> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/office',
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
        404: `Office not found`,
      },
    });
  }
  /**
   * Update office details
   * @returns OfficeModel Updated office
   * @throws ApiError
   */
  public updateOffice({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: OfficeModel,
  }): Observable<OfficeModel> {
    return __request(OpenAPI, this.http, {
      method: 'PUT',
      url: '/office',
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
