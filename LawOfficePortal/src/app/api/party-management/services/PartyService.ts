/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { PartyCountModel } from '../models/PartyCountModel';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
  providedIn: 'root',
})
export class PartyService {
  constructor(public readonly http: HttpClient) {}
  /**
   * Get party count aggregates
   * @returns PartyCountModel Party count aggregates
   * @throws ApiError
   */
  public getPartyCount({
    xOfficeId,
  }: {
    /**
     * Tenant office identifier
     */
    xOfficeId: string,
  }): Observable<PartyCountModel> {
    return __request(OpenAPI, this.http, {
      method: 'GET',
      url: '/party/count',
      headers: {
        'X-Office-Id': xOfficeId,
      },
      errors: {
        400: `Invalid request`,
      },
    });
  }
}
