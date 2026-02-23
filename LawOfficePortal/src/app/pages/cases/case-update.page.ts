import { Component, signal, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { OfficeService } from '../../services/office.service';
import { CaseService } from '../../services/case.service';
import { ClientService } from '../../services/client.service';
import { OpposingPartyService } from '../../services/opposing-party.service';
import { Case } from '../../models/case.model';
import { Client } from '../../models/client.model';
import { OpposingParty } from '../../models/opposing-party.model';
import { CaseUpdateForm } from './case-update-form.model';
import { CaseUpdateDetailsTabComponent } from './details/case-update-details-tab.component';
import { CaseUpdateHearingsTabComponent } from './hearings/case-update-hearings-tab.component';
import { CaseUpdateDocumentsTabComponent } from './documents/case-update-documents-tab.component';

@Component({
  selector: 'app-case-update-page',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    CaseUpdateDetailsTabComponent,
    CaseUpdateHearingsTabComponent,
    CaseUpdateDocumentsTabComponent
  ],
  templateUrl: './case-update.page.html',
  styleUrl: './case-update.page.scss'
})
export class CaseUpdatePageComponent implements OnInit {
  private errorBanner = signal<string | null>(null);
  private saving = signal<boolean>(false);
  private loading = signal<boolean>(true);
  private clients = signal<Client[]>([]);
  private opposingParties = signal<OpposingParty[]>([]);

  readonly errorMessage = this.errorBanner.asReadonly();
  readonly isSaving = this.saving.asReadonly();
  readonly isLoading = this.loading.asReadonly();
  readonly clientsList = this.clients.asReadonly();
  readonly opposingPartiesList = this.opposingParties.asReadonly();

  // Form fields
  caseId: string | null = null;
  form: CaseUpdateForm = {
    identificationNumber: '',
    description: null,
    year: null,
    city: null,
    competentCourt: null,
    judge: null,
    active: true,
    clientIds: [],
    opposingPartyIds: []
  };

  constructor(
    private caseService: CaseService,
    private clientService: ClientService,
    private opposingPartyService: OpposingPartyService,
    private router: Router,
    private route: ActivatedRoute,
    private officeService: OfficeService
  ) {
    effect(() => {
      const officeId = this.officeService.officeId();
      if (officeId) {
        this.clientService.getClients(officeId).subscribe({
          next: (clientsList) => this.clients.set(clientsList),
          error: (err) => console.error('Error loading clients:', err)
        });
        this.opposingPartyService.getOpposingParties(officeId).subscribe({
          next: (opposingPartiesList) => this.opposingParties.set(opposingPartiesList),
          error: (err) => console.error('Error loading opposing parties:', err)
        });
      }
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    
    if (!id) {
      this.errorBanner.set('Case ID is missing.');
      this.loading.set(false);
      return;
    }

    this.caseId = id;
    this.loadCase(id);
  }

  private loadCase(id: string): void {
    const officeId = this.officeService.officeId();
    
    if (!officeId) {
      this.errorBanner.set('Office ID is not set. Please select an office first.');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);

    this.caseService.getCase(officeId, id).subscribe({
      next: (caseData) => {
        this.form = {
          identificationNumber: caseData.identificationNumber,
          description: caseData.description,
          year: caseData.year,
          city: caseData.city,
          competentCourt: caseData.competentCourt,
          judge: caseData.judge,
          active: caseData.active,
          clientIds: [...caseData.clientIds],
          opposingPartyIds: [...caseData.opposingPartyIds]
        };
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        this.errorBanner.set(`Failed to load case: ${err.message}`);
        console.error('Error loading case:', err);
      }
    });
  }

  save(): void {
    const officeId = this.officeService.officeId();
    
    if (!officeId) {
      this.errorBanner.set('Office ID is not set. Please select an office first.');
      return;
    }

    if (!this.caseId) {
      this.errorBanner.set('Case ID is missing.');
      return;
    }

    // Basic validation
    if (!this.form.identificationNumber || !this.form.clientIds.length) {
      this.errorBanner.set('Identification Number and Clients are required fields.');
      return;
    }

    this.saving.set(true);
    this.errorBanner.set(null);

    const updatedCase: Case = {
      id: this.caseId,
      officeId,
      clientIds: this.form.clientIds,
      opposingPartyIds: this.form.opposingPartyIds,
      identificationNumber: this.form.identificationNumber,
      description: this.form.description || null,
      active: this.form.active,
      competentCourt: this.form.competentCourt || null,
      city: this.form.city || null,
      year: this.form.year || null,
      judge: this.form.judge || null
    };

    this.caseService.updateCase(updatedCase).subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['/cases']);
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        let errorMsg = 'Failed to update case. ';

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
        console.error('Error updating case:', err);
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
