import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PartyService as GeneratedPartyService } from '../api/party-management';
import type { PartyCountModel } from '../api/party-management';
import { PartyCount } from '../models/party-count.model';

@Injectable({
  providedIn: 'root'
})
export class PartyService {
  private api = inject(GeneratedPartyService);

  getCount(): Observable<PartyCount> {
    return this.api.getPartyCount({ xOfficeId: '' }) as Observable<PartyCount>;
  }
}