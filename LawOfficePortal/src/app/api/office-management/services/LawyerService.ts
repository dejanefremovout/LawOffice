/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { LawyerCreateModel } from '../models/LawyerCreateModel';
import type { LawyerModel } from '../models/LawyerModel';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
  providedIn: 'root',
})
export class LawyerService {
  constructor(public readonly http: HttpClient) {}
  /**
   * Get a lawyer by ID
   * @returns LawyerModel The requested lawyer
   * @throws ApiError
   */
  public getLawyer({
    xOfficeId,
    lawyerId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Lawyer identifier
     */
    lawyerId: string,
  }): Observable<LawyerModel> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/lawyer/{lawyerId}',
      path: {
        'lawyerId': lawyerId,
      },
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
        404: `Lawyer not found`,
      },
    });
  }
  /**
   * Get all lawyers
   * @returns LawyerModel List of lawyers
   * @throws ApiError
   */
  public getAllLawyers({
    xOfficeId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
  }): Observable<Array<LawyerModel>> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/lawyer',
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
      },
    });
  }
  /**
   * Create a lawyer
   * @returns LawyerModel Created lawyer
   * @throws ApiError
   */
  public createLawyer({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: LawyerCreateModel,
  }): Observable<LawyerModel> {
    return __request(OpenAPI, this.http, {
      method: 'POST',
      url: '/lawyer',
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
   * Update a lawyer
   * @returns LawyerModel Updated lawyer
   * @throws ApiError
   */
  public updateLawyer({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: LawyerModel,
  }): Observable<LawyerModel> {
    return __request(OpenAPI, this.http, {
      method: 'PUT',
      url: '/lawyer',
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
