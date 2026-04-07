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
export class ClientService {
  constructor(public readonly http: HttpClient) {}
  /**
   * Get a client by ID
   * @returns PartyModel The requested client
   * @throws ApiError
   */
  public getClient({
    xOfficeId,
    clientId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
    /**
     * Client identifier
     */
    clientId: string,
  }): Observable<PartyModel> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/client/{clientId}',
      path: {
        'clientId': clientId,
      },
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
        404: `Client not found`,
      },
    });
  }
  /**
   * Get all clients
   * @returns PartyModel List of clients
   * @throws ApiError
   */
  public getAllClients({
    xOfficeId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
  }): Observable<Array<PartyModel>> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/client',
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
      },
    });
  }
  /**
   * Create a client
   * @returns PartyModel Created client
   * @throws ApiError
   */
  public createClient({
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
      url: '/client',
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
   * Update a client
   * @returns PartyModel Updated client
   * @throws ApiError
   */
  public updateClient({
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
      url: '/client',
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
