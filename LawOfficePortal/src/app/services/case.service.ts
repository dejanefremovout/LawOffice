import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CaseService as GeneratedCaseService } from '../api/case-management';
import type { CaseModel, CaseCountModel, CaseCreateModel, CaseHearingModel } from '../api/case-management';
import { Case } from '../models/case.model';
import { CaseCount } from '../models/case-count.model';
import { CaseHearing } from '../models/case-hearing.model';

@Injectable({
  providedIn: 'root'
})
export class CaseService {
  private api = inject(GeneratedCaseService);

  getCases(): Observable<Case[]> {
    return this.api.getAllCases({ xOfficeId: '' }) as Observable<Case[]>;
  }

  getCase(caseId: string): Observable<Case> {
    return this.api.getCase({ xOfficeId: '', caseId }) as Observable<Case>;
  }

  createCase(caseData: Omit<Case, 'id'>): Observable<Case> {
    return this.api.createCase({ xOfficeId: '', requestBody: caseData as unknown as CaseCreateModel }) as Observable<Case>;
  }

  updateCase(caseData: Case): Observable<Case> {
    return this.api.updateCase({ xOfficeId: '', requestBody: caseData as CaseModel }) as Observable<Case>;
  }

  getCount(): Observable<CaseCount> {
    return this.api.getCaseCount({ xOfficeId: '' }) as Observable<CaseCount>;
  }

  getLastCases(count: number): Observable<Case[]> {
    return this.api.getLastCases({ xOfficeId: '', count }) as Observable<Case[]>;
  }

  getUpcomingHearings(count: number): Observable<CaseHearing[]> {
    return this.api.getCasesWithHearings({ xOfficeId: '', count }) as Observable<CaseHearing[]>;
  }
}
