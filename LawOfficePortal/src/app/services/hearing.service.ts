import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HearingService as GeneratedHearingService } from '../api/case-management';
import type { HearingModel, HearingCreateModel } from '../api/case-management';
import { Hearing } from '../models/hearing.model';

@Injectable({
  providedIn: 'root'
})
export class HearingService {
  private api = inject(GeneratedHearingService);

  getHearings(caseId: string): Observable<Hearing[]> {
    return this.api.getAllHearings({ xOfficeId: '', caseId }) as Observable<Hearing[]>;
  }

  getHearing(hearingId: string): Observable<Hearing> {
    return this.api.getHearing({ xOfficeId: '', hearingId }) as Observable<Hearing>;
  }

  createHearing(hearing: Omit<Hearing, 'id'>): Observable<Hearing> {
    return this.api.createHearing({ xOfficeId: '', requestBody: hearing as HearingCreateModel }) as Observable<Hearing>;
  }

  updateHearing(hearing: Hearing): Observable<Hearing> {
    return this.api.updateHearing({ xOfficeId: '', requestBody: hearing as HearingModel }) as Observable<Hearing>;
  }
}
