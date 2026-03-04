import { Component, OnInit, signal } from '@angular/core';
import { LawyerCreateComponent } from './office-lawyer-create.component';
import { LawyerUpdateComponent } from './office-lawyer-update.component';
import { CommonModule } from '@angular/common';
import { LawyerService } from '../../../services/lawyer.service';
import { Lawyer } from '../../../models/lawyer.model';

@Component({
  selector: 'app-office-lawyers-tab',
  standalone: true,
  imports: [CommonModule, LawyerCreateComponent, LawyerUpdateComponent],
  templateUrl: './office-lawyers-tab.component.html',
  styleUrl: './office-lawyers-tab.component.scss'
})
export class OfficeLawyersTabComponent implements OnInit {
  private lawyers = signal<Lawyer[]>([]);
  private loading = signal<boolean>(false);
  private error = signal<string | null>(null);
  public selectedLawyerId!: string;

  readonly lawyersList = this.lawyers.asReadonly();
  readonly isLoading = this.loading.asReadonly();
  readonly errorMessage = this.error.asReadonly();
  readonly hasLawyers = () => this.lawyersList().length > 0;
  showCreateLawyer = signal<boolean>(false);
  showUpdateLawyer = signal<boolean>(false);

  constructor(
    private lawyerService: LawyerService,
  ) {}

  ngOnInit(): void {
    this.loadLawyers();
  }

  private loadLawyers(): void {
    this.loading.set(true);
    this.error.set(null);
    this.lawyerService.getLawyers().subscribe({
      next: (data) => {
        this.lawyers.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load lawyers: ' + (err?.message || 'Unknown error'));
        this.loading.set(false);
      }
    });
  }

  createNewLawyer(): void {
    this.showCreateLawyer.set(true);
  }

  updateExistingLawyer(lawyer: Lawyer): void {
    this.selectedLawyerId = lawyer.id;
    this.showUpdateLawyer.set(true);
  }

  onLawyerSaved(): void {
    this.showCreateLawyer.set(false);
    this.showUpdateLawyer.set(false);
    this.loadLawyers();
  }

  onLawyerCancelled(): void {
    this.showCreateLawyer.set(false);
    this.showUpdateLawyer.set(false);
  }

  getStatusLabel(held: boolean): string {
    return held ? 'Held' : 'Not held';
  }
}
