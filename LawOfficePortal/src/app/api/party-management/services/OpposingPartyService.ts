/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { PartyCreateModel } from '../models/PartyCreateModel';
import type { PartyModel } from '../models/PartyModel';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
  providedIn: 'root',
})
export class OpposingPartyService {
  constructor(public readonly http: HttpClient) {}
  /**
   * Get an opposing party by ID
   * @returns PartyModel The requested opposing party
   * @throws ApiError
   */
  public getOpposingParty({
    xOfficeId,
    opposingPartyId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Opposing party identifier
     */
    opposingPartyId: string,
  }): Observable<PartyModel> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/opposingParty/{opposingPartyId}',
      path: {
        'opposingPartyId': opposingPartyId,
      },
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
        404: `Opposing party not found`,
      },
    });
  }
  /**
   * Delete an opposing party
   * @returns void
   * @throws ApiError
   */
  public deleteOpposingParty({
    xOfficeId,
    opposingPartyId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Opposing party identifier
     */
    opposingPartyId: string,
  }): Observable<void> {
    return __request(OpenAPI, this.http, {
      method: 'DELETE',
      url: '/opposingParty/{opposingPartyId}',
      path: {
        'opposingPartyId': opposingPartyId,
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
   * Get all opposing parties
   * @returns PartyModel List of opposing parties
   * @throws ApiError
   */
  public getAllOpposingParties({
    xOfficeId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
  }): Observable<Array<PartyModel>> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/opposingParty',
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
      },
    });
  }
  /**
   * Create an opposing party
   * @returns PartyModel Created opposing party
   * @throws ApiError
   */
  public createOpposingParty({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: PartyCreateModel,
  }): Observable<PartyModel> {
    return __request(OpenAPI, this.http, {
      method: 'POST',
      url: '/opposingParty',
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
   * Update an opposing party
   * @returns PartyModel Updated opposing party
   * @throws ApiError
   */
  public updateOpposingParty({
    xOfficeId,
    requestBody,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    requestBody: PartyModel,
  }): Observable<PartyModel> {
    return __request(OpenAPI, this.http, {
      method: 'PUT',
      url: '/opposingParty',
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
