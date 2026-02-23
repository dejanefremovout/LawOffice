import { Component, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { signal, computed } from '@angular/core';
import { OfficeService } from '../../services/office.service';
import { CaseService } from '../../services/case.service';
import { Case } from '../../models/case.model';

@Component({
  selector: 'app-cases-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cases.page.html',
  styleUrl: './cases.page.scss'
})
export class CasesPageComponent implements OnInit {
  private cases = signal<Case[]>([]);
  private loading = signal<boolean>(false);
  private error = signal<string | null>(null);

  readonly casesList = this.cases.asReadonly();
  readonly isLoading = this.loading.asReadonly();
  readonly errorMessage = this.error.asReadonly();
  readonly hasCases = computed(() => this.casesList().length > 0);

  constructor(
    private officeService: OfficeService,
    private caseService: CaseService,
    private router: Router
  ) {
    // Auto-fetch cases when officeId changes
    effect(() => {
      const officeId = this.officeService.officeId();
      if (officeId) {
        this.loadCases(officeId);
      }
    });
  }

  ngOnInit(): void {
    const officeId = this.officeService.officeId();
    if (officeId) {
      this.loadCases(officeId);
    }
  }

  private loadCases(officeId: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.caseService.getCases(officeId).subscribe({
      next: (data) => {
        this.cases.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(`Failed to load cases: ${err.message}`);
        this.loading.set(false);
        console.error('Error loading cases:', err);
      }
    });
  }

  createNewCase(): void {
    this.router.navigate(['/cases/create']);
  }

  editCase(caseId: string): void {
    this.router.navigate(['/cases', caseId]);
  }

  getStatusLabel(active: boolean): string {
    return active ? 'Active' : 'Inactive';
  }
}
