import { Component, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { OfficeService } from '../../services/office.service';
import { CaseService } from '../../services/case.service';
import { ClientService } from '../../services/client.service';
import { Case } from '../../models/case.model';
import { Client } from '../../models/client.model';
import { OpposingParty } from '../../models/opposing-party.model';
import { OpposingPartyService } from '../../services/opposing-party.service';

@Component({
  selector: 'app-case-create-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './case-create.page.html',
  styleUrl: './case-create.page.scss'
})
export class CaseCreatePageComponent {
  private errorBanner = signal<string | null>(null);
  private saving = signal<boolean>(false);
  private clients = signal<Client[]>([]);
  private opposingParties = signal<OpposingParty[]>([]);

  readonly errorMessage = this.errorBanner.asReadonly();
  readonly isSaving = this.saving.asReadonly();
  readonly clientsList = this.clients.asReadonly();
  readonly opposingPartiesList = this.opposingParties.asReadonly();

  // Form fields
  identificationNumber = '';
  description = '';
  year = new Date().getFullYear();
  city: string | null = null;
  competentCourt: string | null = null;
  judge: string | null = null;
  active = true;
  clientIds: string[] = [];
  opposingPartyIds: string[] = [];
  
  constructor(
    private caseService: CaseService,
    private router: Router,
    private officeService: OfficeService,
    private clientService: ClientService,
    private opposingPartyService: OpposingPartyService
  ) {
    effect(() => {
      const officeId = this.officeService.officeId();
      if (officeId) {
        this.clientService.getClients(officeId).subscribe({
          next: (clients) => this.clients.set(clients),
          error: (err) => console.error('Error loading clients:', err)
        });
        this.opposingPartyService.getOpposingParties(officeId).subscribe({
          next: (opposingParties) => this.opposingParties.set(opposingParties),
          error: (err) => console.error('Error loading opposing parties:', err)
        });
      }
    });
  }

  save(): void {
    const officeId = this.officeService.officeId();

    if (!officeId) {
      this.errorBanner.set('Office ID is not set. Please select an office first.');
      return;
    }

    if (!this.identificationNumber || !this.clientIds.length) {
      this.errorBanner.set('Identification Number and Clients are required fields.');
      return;
    }

    this.saving.set(true);
    this.errorBanner.set(null);

    const newCase: Omit<Case, 'id'> = {
      officeId,
      clientIds: this.clientIds,
      opposingPartyIds: this.opposingPartyIds,
      identificationNumber: this.identificationNumber,
      description: this.description || null,
      active: true,
      competentCourt: this.competentCourt || null,
      city: this.city || null,
      year: this.year || null,
      judge: this.judge || null
    };

    this.caseService.createCase(newCase).subscribe({
      next: (savedCase) => {
        this.saving.set(false);
        this.router.navigate(['/cases/', savedCase.id]);
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        let errorMsg = 'Failed to save case. ';

        if (err.error?.message) {
          errorMsg += err.error.message;
        } else if (err.error?.title) {
          errorMsg += err.error.title;
        } else if (err.message) {
          errorMsg += err.message;
        } else {
          errorMsg += 'Please try again.';
        }

        this.errorBanner.set(errorMsg);
        console.error('Error creating case:', err);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/cases']);
  }

  closeError(): void {
    this.errorBanner.set(null);
  }
}
