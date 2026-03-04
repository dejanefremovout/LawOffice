import { Component, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { signal, computed } from '@angular/core';
import { OpposingPartyService } from '../../services/opposing-party.service';
import { OpposingParty } from '../../models/opposing-party.model';

@Component({
  selector: 'app-opposing-parties-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './opposing-parties.page.html',
  styleUrl: './opposing-parties.page.scss'
})
export class OpposingPartiesPageComponent implements OnInit {
  private opposingParties = signal<OpposingParty[]>([]);
  private loading = signal<boolean>(false);
  private error = signal<string | null>(null);

  readonly opposingPartiesList = this.opposingParties.asReadonly();
  readonly isLoading = this.loading.asReadonly();
  readonly errorMessage = this.error.asReadonly();
  readonly hasOpposingParties = computed(() => this.opposingPartiesList().length > 0);

  constructor(
    private opposingPartyService: OpposingPartyService,
    private router: Router
  ) {
    // Auto-fetch clients when officeId changes
    effect(() => {
      this.loadOpposingParties();
    });
  }

  ngOnInit(): void {
  }

  private loadOpposingParties(): void {
    this.loading.set(true);
    this.error.set(null);

    this.opposingPartyService.getOpposingParties().subscribe({
      next: (data) => {
        this.opposingParties.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(`Failed to load opposing parties: ${err.message}`);
        this.loading.set(false);
        console.error('Error loading opposing parties:', err);
      }
    });
  }

  createNewOpposingParty(): void {
    this.router.navigate(['/opposing-parties/create']);
  }

  editOpposingParty(opposingPartyId: string): void {
    this.router.navigate(['/opposing-parties', opposingPartyId]);
  }
}
