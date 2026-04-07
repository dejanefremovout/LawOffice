import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { LawyerService as GeneratedLawyerService } from '../api/office-management';
import type { LawyerModel, LawyerCreateModel } from '../api/office-management';
import { Lawyer } from '../models/lawyer.model';

@Injectable({
  providedIn: 'root'
})
export class LawyerService {
  private api = inject(GeneratedLawyerService);

  getLawyers(): Observable<Lawyer[]> {
    return this.api.getAllLawyers({ xOfficeId: '' }) as Observable<Lawyer[]>;
  }

  getLawyer(lawyerId: string): Observable<Lawyer> {
    return this.api.getLawyer({ xOfficeId: '', lawyerId }) as Observable<Lawyer>;
  }

  createLawyer(lawyer: Omit<Lawyer, 'id'>): Observable<Lawyer> {
    return this.api.createLawyer({ xOfficeId: '', requestBody: lawyer as LawyerCreateModel }) as Observable<Lawyer>;
  }

  updateLawyer(lawyer: Lawyer): Observable<Lawyer> {
    return this.api.updateLawyer({ xOfficeId: '', requestBody: lawyer as LawyerModel }) as Observable<Lawyer>;
  }
}
