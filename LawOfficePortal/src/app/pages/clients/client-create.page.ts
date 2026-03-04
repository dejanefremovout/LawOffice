import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { ClientService } from '../../services/client.service';
import { Client } from '../../models/client.model';

@Component({
  selector: 'app-client-create-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './client-create.page.html',
  styleUrl: './client-create.page.scss'
})
export class ClientCreatePageComponent {
  private errorBanner = signal<string | null>(null);
  private saving = signal<boolean>(false);

  readonly errorMessage = this.errorBanner.asReadonly();
  readonly isSaving = this.saving.asReadonly();

  // Form fields
  firstName = '';
  lastName = '';
  address: string | null = null;
  description: string | null = null;
  phone: string | null = null;
  identificationNumber: string | null = null;

  constructor(
    private clientService: ClientService,
    private router: Router,
  ) {}

  save(): void {
    // Basic validation
    if (!this.firstName || !this.lastName) {
      this.errorBanner.set('First Name and Last Name are required fields.');
      return;
    }

    this.saving.set(true);
    this.errorBanner.set(null);

    const newClient: Omit<Client, 'id'> = {
      firstName: this.firstName,
      lastName: this.lastName,
      address: this.address || null,
      description: this.description || null,
      phone: this.phone || null,
      identificationNumber: this.identificationNumber || null
    };

    this.clientService.createClient(newClient).subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['/clients']);
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        let errorMsg = 'Failed to save client. ';
        
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
        console.error('Error creating client:', err);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/clients']);
  }

  closeError(): void {
    this.errorBanner.set(null);
  }
}
