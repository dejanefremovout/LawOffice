import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { ClientService } from '../../services/client.service';
import { Client } from '../../models/client.model';

@Component({
  selector: 'app-client-update-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './client-update.page.html',
  styleUrl: './client-update.page.scss'
})
export class ClientUpdatePageComponent implements OnInit {
  private errorBanner = signal<string | null>(null);
  private saving = signal<boolean>(false);
  private loading = signal<boolean>(true);

  readonly errorMessage = this.errorBanner.asReadonly();
  readonly isSaving = this.saving.asReadonly();
  readonly isLoading = this.loading.asReadonly();

  // Form fields
  clientId: string | null = null;
  firstName = '';
  lastName = '';
  address: string | null = null;
  description: string | null = null;
  phone: string | null = null;
  identificationNumber: string | null = null;

  constructor(
    private clientService: ClientService,
    private router: Router,
    private route: ActivatedRoute,
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    
    if (!id) {
      this.errorBanner.set('Client ID is missing.');
      this.loading.set(false);
      return;
    }

    this.clientId = id;
    this.loadClient(id);
  }

  private loadClient(id: string): void {
    this.loading.set(true);

    this.clientService.getClient(id).subscribe({
      next: (client) => {
        this.firstName = client.firstName;
        this.lastName = client.lastName;
        this.address = client.address;
        this.description = client.description;
        this.phone = client.phone;
        this.identificationNumber = client.identificationNumber;
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        this.errorBanner.set(`Failed to load client: ${err.message}`);
        console.error('Error loading client:', err);
      }
    });
  }

  save(): void {
    if (!this.clientId) {
      this.errorBanner.set('Client ID is missing.');
      return;
    }

    // Basic validation
    if (!this.firstName || !this.lastName) {
      this.errorBanner.set('First Name and Last Name are required fields.');
      return;
    }

    this.saving.set(true);
    this.errorBanner.set(null);

    const updatedClient: Client = {
      id: this.clientId,
      firstName: this.firstName,
      lastName: this.lastName,
      address: this.address || null,
      description: this.description || null,
      phone: this.phone || null,
      identificationNumber: this.identificationNumber || null
    };

    this.clientService.updateClient(updatedClient).subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['/clients']);
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        let errorMsg = 'Failed to update client. ';
        
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
        console.error('Error updating client:', err);
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
