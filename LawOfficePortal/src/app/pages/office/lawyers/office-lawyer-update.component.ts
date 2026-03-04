import { Component, Input, Output, EventEmitter, signal, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { LawyerService } from '../../../services/lawyer.service';

@Component({
  selector: 'app-lawyer-update',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './office-lawyer-update.component.html',
  styleUrls: ['./office-lawyer-update.component.scss']
})
export class LawyerUpdateComponent {
  @Input() lawyerId!: string;
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  private errorBanner = signal<string | null>(null);
  private saving = signal<boolean>(false);

  readonly errorMessage = this.errorBanner.asReadonly();
  readonly isSaving = this.saving.asReadonly();

  // Form fields
  firstName: string = '';
  lastName: string = '';
  email: string = '';

  constructor(private lawyerService: LawyerService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadLawyer();
  }

  private loadLawyer(): void {
    this.saving.set(true);
    this.lawyerService.getLawyer(this.lawyerId).subscribe({
      next: (lawyer) => {
        this.firstName = lawyer.firstName;
        this.lastName = lawyer.lastName;
        this.email = lawyer.email;
        this.saving.set(false);
        this.cdr.detectChanges();
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        this.errorBanner.set(`Failed to load lawyer: ${err.message}`);
        console.error('Error loading lawyer:', err);
        this.cdr.detectChanges();
      }
    });
  }

  save(): void {
    if (!this.lawyerId) {
      this.errorBanner.set('Lawyer ID is missing.');
      return;
    }

    if (!this.firstName || !this.lastName || !this.email ) {
      this.errorBanner.set('All fields are required.');
      return;
    }

    this.saving.set(true);
    this.errorBanner.set(null);

    const updatedLawyer = {
      id: this.lawyerId,
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email
    };

    this.lawyerService.updateLawyer(updatedLawyer).subscribe({
      next: () => {
        this.saving.set(false);
        this.saved.emit();
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        let errorMsg = 'Failed to save lawyer. ';

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
        console.error('Error updating lawyer:', err);
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
