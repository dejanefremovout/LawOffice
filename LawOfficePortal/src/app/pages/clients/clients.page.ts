import { Component, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { signal, computed } from '@angular/core';
import { OfficeService } from '../../services/office.service';
import { ClientService } from '../../services/client.service';
import { Client } from '../../models/client.model';

@Component({
  selector: 'app-clients-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './clients.page.html',
  styleUrl: './clients.page.scss'
})
export class ClientsPageComponent implements OnInit {
  private clients = signal<Client[]>([]);
  private loading = signal<boolean>(false);
  private error = signal<string | null>(null);

  readonly clientsList = this.clients.asReadonly();
  readonly isLoading = this.loading.asReadonly();
  readonly errorMessage = this.error.asReadonly();
  readonly hasClients = computed(() => this.clientsList().length > 0);

  constructor(
    private officeService: OfficeService,
    private clientService: ClientService,
    private router: Router
  ) {
    // Auto-fetch clients when officeId changes
    effect(() => {
      const officeId = this.officeService.officeId();
      if (officeId) {
        this.loadClients(officeId);
      }
    });
  }

  ngOnInit(): void {
  }

  private loadClients(officeId: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.clientService.getClients(officeId).subscribe({
      next: (data) => {
        this.clients.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(`Failed to load clients: ${err.message}`);
        this.loading.set(false);
        console.error('Error loading clients:', err);
      }
    });
  }

  createNewClient(): void {
    this.router.navigate(['/clients/create']);
  }

  editClient(clientId: string): void {
    this.router.navigate(['/clients', clientId]);
  }
}
