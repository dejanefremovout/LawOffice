import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { HearingService } from '../../../services/hearing.service';

@Component({
  selector: 'app-case-update-hearing-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './case-update-hearing-create.component.html',
  styleUrls: ['./case-update-hearing-create.component.scss']
})
export class CaseUpdateHearingCreateComponent {
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

  constructor(private hearingService: HearingService) {}

  save(): void {
    if (!this.caseId) {
      this.errorBanner.set('Case ID is missing.');
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

    const newHearing = {
      caseId: this.caseId,
      courtroom: this.courtroom,
      description: this.description,
      date: hearingDate,
      held: false
    };

    this.hearingService.createHearing(newHearing).subscribe({
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
        console.error('Error creating hearing:', err);
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
