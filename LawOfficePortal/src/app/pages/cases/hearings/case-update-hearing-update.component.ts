import { Component, Input, Output, EventEmitter, signal, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { HearingService } from '../../../services/hearing.service';

@Component({
  selector: 'app-case-update-hearing-update',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './case-update-hearing-update.component.html',
  styleUrls: ['./case-update-hearing-update.component.scss']
})
export class CaseUpdateHearingUpdateComponent {
  @Input() hearingId!: string;
  @Input() caseId!: string;
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  private errorBanner = signal<string | null>(null);
  private saving = signal<boolean>(false);

  readonly errorMessage = this.errorBanner.asReadonly();
  readonly isSaving = this.saving.asReadonly();

  // Form fields
  courtroom: string | null = null;
  description: string | null = null;
  date: string = new Date().toISOString().slice(0, 16); // For datetime-local input
  held: boolean = false;

  constructor(private hearingService: HearingService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    if (!this.hearingId || !this.caseId) {
      this.errorBanner.set('Hearing ID or Case ID is missing.');
      return;
    }

    this.loadHearing();
  }

  private loadHearing(): void {
    this.saving.set(true);
    this.hearingService.getHearing(this.hearingId).subscribe({
      next: (hearing) => {
        this.courtroom = hearing.courtroom;
        this.description = hearing.description;
        this.date = hearing.date.slice(0, 16); // Assuming hearing.date is in ISO format
        this.held = hearing.held;
        this.saving.set(false);
        this.cdr.detectChanges();
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        this.errorBanner.set(`Failed to load hearing: ${err.message}`);
        console.error('Error loading hearing:', err);
        this.cdr.detectChanges();
      }
    });
  }

  save(): void {
    if (!this.hearingId || !this.caseId) {
      this.errorBanner.set('Hearing ID or Case ID is missing.');
      return;
    }

    if (!this.date) {
      this.errorBanner.set('Hearing date/time is required.');
      return;
    }

    this.saving.set(true);
    this.errorBanner.set(null);

    // Convert the datetime-local input to ISO string format
    const hearingDate = new Date(this.date).toISOString();

    const updatedHearing = {
      id: this.hearingId,
      caseId: this.caseId,
      courtroom: this.courtroom,
      description: this.description,
      date: hearingDate,
      held: this.held
    };

    this.hearingService.updateHearing(updatedHearing).subscribe({
      next: () => {
        this.saving.set(false);
        this.saved.emit();
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        let errorMsg = 'Failed to save hearing. ';

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
        console.error('Error updating hearing:', err);
      }
    });
  }

  cancel(): void {
    this.cancelled.emit();
  }

  closeError(): void {
    this.errorBanner.set(null);
  }
}
