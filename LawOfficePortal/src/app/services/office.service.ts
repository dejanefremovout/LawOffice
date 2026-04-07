import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { OfficeService as GeneratedOfficeService } from '../api/office-management';
import type { OfficeModel } from '../api/office-management';
import { Office } from '../models/office.model';

@Injectable({
  providedIn: 'root'
})
export class OfficeService {
  private api = inject(GeneratedOfficeService);

  getOffice(): Observable<Office> {
    return this.api.getOffice({ xOfficeId: '' }) as Observable<Office>;
  }

  updateOffice(office: Office): Observable<Office> {
    return this.api.updateOffice({ xOfficeId: '', requestBody: office as OfficeModel }) as Observable<Office>;
  }
}
