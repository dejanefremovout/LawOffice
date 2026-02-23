import { Component, OnInit, signal } from '@angular/core';
import { LawyerCreateComponent } from './office-lawyer-create.component';
import { LawyerUpdateComponent } from './office-lawyer-update.component';
import { CommonModule } from '@angular/common';
import { LawyerService } from '../../../services/lawyer.service';
import { OfficeService } from '../../../services/office.service';
import { ActivatedRoute } from '@angular/router';
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
  public officeId!: string;
  public selectedLawyerId!: string;

  readonly lawyersList = this.lawyers.asReadonly();
  readonly isLoading = this.loading.asReadonly();
  readonly errorMessage = this.error.asReadonly();
  readonly hasLawyers = () => this.lawyersList().length > 0;
  showCreateLawyer = signal<boolean>(false);
  showUpdateLawyer = signal<boolean>(false);

  constructor(
    private lawyerService: LawyerService,
    private officeService: OfficeService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const officeId = this.officeService.officeId();
    if (!officeId) {
      this.error.set('Office ID is missing.');
      return;
    }
    this.officeId = officeId;
    this.loadLawyers(officeId);
  }

  private loadLawyers(officeId: string): void {
    this.loading.set(true);
    this.error.set(null);
    this.lawyerService.getLawyers(officeId).subscribe({
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
    this.loadLawyers(this.officeId);
  }

  onLawyerCancelled(): void {
    this.showCreateLawyer.set(false);
    this.showUpdateLawyer.set(false);
  }

  getStatusLabel(held: boolean): string {
    return held ? 'Held' : 'Not held';
  }
}
