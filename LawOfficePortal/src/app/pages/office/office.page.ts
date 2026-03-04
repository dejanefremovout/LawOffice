import { Component, signal, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { OfficeService } from '../../services/office.service';
import { Office } from '../../models/office.model';
import { OfficeUpdateForm } from './office-update-form.model';
import { OfficeDetailsTabComponent } from './details/office-details-tab.component';
import { OfficeLawyersTabComponent } from './lawyers/office-lawyers-tab.component';

@Component({
  selector: 'app-office-page',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    OfficeDetailsTabComponent,
    OfficeLawyersTabComponent
  ],
  templateUrl: './office.page.html',
  styleUrl: './office.page.scss'
})
export class OfficePageComponent implements OnInit {
  private errorBanner = signal<string | null>(null);
  private saving = signal<boolean>(false);
  private loading = signal<boolean>(true);

  readonly errorMessage = this.errorBanner.asReadonly();
  readonly isSaving = this.saving.asReadonly();
  readonly isLoading = this.loading.asReadonly();

  // Form fields
  form: OfficeUpdateForm = {
    name: '',
    address: null,
  };

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private officeService: OfficeService
  ) {
    effect(() => {
    });
  }

  ngOnInit(): void {
    this.loadOffice();
  }

  private loadOffice(): void {
    this.loading.set(true);

    this.officeService.getOffice().subscribe({
      next: (officeData) => {
        this.form = {
          name: officeData.name,
          address: officeData.address || null
        };
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        this.errorBanner.set(`Failed to load office: ${err.message}`);
        console.error('Error loading office:', err);
      }
    });
  }

  save(): void {
    // Basic validation
    if (!this.form.name) {
      this.errorBanner.set('Name is required field.');
      return;
    }

    this.saving.set(true);
    this.errorBanner.set(null);

    const updatedOffice: Office = {
      name: this.form.name,
      address: this.form.address || null
    };

    this.officeService.updateOffice(updatedOffice).subscribe({
      next: () => {
        this.saving.set(false);
        this.loadOffice();
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        let errorMsg = 'Failed to update office. ';

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
        console.error('Error updating office:', err);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/']);
  }

  closeError(): void {
    this.errorBanner.set(null);
  }
}
