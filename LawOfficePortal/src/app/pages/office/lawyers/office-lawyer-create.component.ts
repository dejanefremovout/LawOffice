import { Component, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { LawyerService } from '../../../services/lawyer.service';

@Component({
  selector: 'app-lawyer-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './office-lawyer-create.component.html',
  styleUrls: ['./office-lawyer-create.component.scss']
})
export class LawyerCreateComponent {
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

  constructor(private lawyerService: LawyerService) {}

  save(): void {
    if (!this.firstName || !this.lastName || !this.email ) {
      this.errorBanner.set('All fields are required.');
      return;
    }

    this.saving.set(true);
    this.errorBanner.set(null);

    const newLawyer = {
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email
    };

    this.lawyerService.createLawyer(newLawyer).subscribe({
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
        console.error('Error creating lawyer:', err);
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
