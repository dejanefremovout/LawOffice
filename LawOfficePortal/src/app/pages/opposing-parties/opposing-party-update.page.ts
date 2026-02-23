import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { OfficeService } from '../../services/office.service';
import { OpposingParty } from '../../models/opposing-party.model';
import { OpposingPartyService } from '../../services/opposing-party.service';

@Component({
  selector: 'app-opposing-party-update-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './opposing-party-update.page.html',
  styleUrl: './opposing-party-update.page.scss'
})
export class OpposingPartyUpdatePageComponent implements OnInit {
  private errorBanner = signal<string | null>(null);
  private saving = signal<boolean>(false);
  private loading = signal<boolean>(true);

  readonly errorMessage = this.errorBanner.asReadonly();
  readonly isSaving = this.saving.asReadonly();
  readonly isLoading = this.loading.asReadonly();

  // Form fields
  opposingPartyId: string | null = null;
  firstName = '';
  lastName = '';
  address: string | null = null;
  description: string | null = null;
  phone: string | null = null;
  identificationNumber: string | null = null;

  constructor(
    private opposingPartyService: OpposingPartyService,
    private router: Router,
    private route: ActivatedRoute,
    private officeService: OfficeService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    
    if (!id) {
      this.errorBanner.set('Opposing Party ID is missing.');
      this.loading.set(false);
      return;
    }

    this.opposingPartyId = id;
    this.loadOpposingParty(id);
  }

  private loadOpposingParty(id: string): void {
    const officeId = this.officeService.officeId();
    
    if (!officeId) {
      this.errorBanner.set('Office ID is not set. Please select an office first.');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);

    this.opposingPartyService.getOpposingParty(officeId, id).subscribe({
      next: (opposingParty) => {
        this.firstName = opposingParty.firstName;
        this.lastName = opposingParty.lastName;
        this.address = opposingParty.address;
        this.description = opposingParty.description;
        this.phone = opposingParty.phone;
        this.identificationNumber = opposingParty.identificationNumber;
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        this.errorBanner.set(`Failed to load opposing party: ${err.message}`);
        console.error('Error loading opposing party:', err);
      }
    });
  }

  save(): void {
    const officeId = this.officeService.officeId();
    
    if (!officeId) {
      this.errorBanner.set('Office ID is not set. Please select an office first.');
      return;
    }

    if (!this.opposingPartyId) {
      this.errorBanner.set('Opposing Party ID is missing.');
      return;
    }

    // Basic validation
    if (!this.firstName || !this.lastName) {
      this.errorBanner.set('First Name and Last Name are required fields.');
      return;
    }

    this.saving.set(true);
    this.errorBanner.set(null);

    const updatedOpposingParty: OpposingParty = {
      id: this.opposingPartyId,
      officeId,
      firstName: this.firstName,
      lastName: this.lastName,
      address: this.address || null,
      description: this.description || null,
      phone: this.phone || null,
      identificationNumber: this.identificationNumber || null
    };

    this.opposingPartyService.updateOpposingParty(updatedOpposingParty).subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['/opposing-parties']);
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        let errorMsg = 'Failed to update opposing party. ';
        
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
        console.error('Error updating opposing party:', err);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/opposing-parties']);
  }

  closeError(): void {
    this.errorBanner.set(null);
  }
}
