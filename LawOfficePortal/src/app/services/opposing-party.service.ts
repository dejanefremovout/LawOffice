import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { OpposingPartyService as GeneratedOpposingPartyService } from '../api/party-management';
import type { PartyModel, PartyCreateModel } from '../api/party-management';
import { OpposingParty } from '../models/opposing-party.model';

@Injectable({
  providedIn: 'root'
})
export class OpposingPartyService {
  private api = inject(GeneratedOpposingPartyService);

  getOpposingParties(): Observable<OpposingParty[]> {
    return this.api.getAllOpposingParties({ xOfficeId: '' }) as Observable<OpposingParty[]>;
  }

  getOpposingParty(opposingPartyId: string): Observable<OpposingParty> {
    return this.api.getOpposingParty({ xOfficeId: '', opposingPartyId }) as Observable<OpposingParty>;
  }

  createOpposingParty(opposingParty: Omit<OpposingParty, 'id'>): Observable<OpposingParty> {
    return this.api.createOpposingParty({ xOfficeId: '', requestBody: opposingParty as PartyCreateModel }) as Observable<OpposingParty>;
  }

  updateOpposingParty(opposingParty: OpposingParty): Observable<OpposingParty> {
    return this.api.updateOpposingParty({ xOfficeId: '', requestBody: opposingParty as PartyModel }) as Observable<OpposingParty>;
  }

  deleteOpposingParty(opposingPartyId: string): Observable<void> {
    return this.api.deleteOpposingParty({ xOfficeId: '', opposingPartyId });
  }
}
