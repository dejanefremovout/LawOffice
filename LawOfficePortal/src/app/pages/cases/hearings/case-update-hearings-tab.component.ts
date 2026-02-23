import { Component, OnInit, signal } from '@angular/core';
import { CaseUpdateHearingCreateComponent } from './case-update-hearing-create.component';
import { CaseUpdateHearingUpdateComponent } from './case-update-hearing-update.component';
import { CommonModule } from '@angular/common';
import { HearingService } from '../../../services/hearing.service';
import { OfficeService } from '../../../services/office.service';
import { ActivatedRoute } from '@angular/router';
import { Hearing } from '../../../models/hearing.model';

@Component({
  selector: 'app-case-update-hearings-tab',
  standalone: true,
  imports: [CommonModule, CaseUpdateHearingCreateComponent, CaseUpdateHearingUpdateComponent],
  templateUrl: './case-update-hearings-tab.component.html',
  styleUrl: './case-update-hearings-tab.component.scss'
})
export class CaseUpdateHearingsTabComponent implements OnInit {
  private hearings = signal<Hearing[]>([]);
  private loading = signal<boolean>(false);
  private error = signal<string | null>(null);
  public caseId!: string;
  public officeId!: string;
  public selectedHearingId!: string;

  readonly hearingsList = this.hearings.asReadonly();
  readonly isLoading = this.loading.asReadonly();
  readonly errorMessage = this.error.asReadonly();
  readonly hasHearings = () => this.hearingsList().length > 0;
  showCreateHearing = signal<boolean>(false);
  showUpdateHearing = signal<boolean>(false);

  constructor(
    private hearingService: HearingService,
    private officeService: OfficeService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const officeId = this.officeService.officeId();
    const caseId = this.route.snapshot.paramMap.get('id');
    if (!officeId || !caseId) {
      this.error.set('Office ID or Case ID is missing.');
      return;
    }
    this.caseId = caseId;
    this.officeId = officeId;
    this.loadHearings(officeId, caseId);
  }

  private loadHearings(officeId: string, caseId: string): void {
    this.loading.set(true);
    this.error.set(null);
    this.hearingService.getHearings(officeId, caseId).subscribe({
      next: (data) => {
        this.hearings.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load hearings: ' + (err?.message || 'Unknown error'));
        this.loading.set(false);
      }
    });
  }

  createNewHearing(): void {
    this.showCreateHearing.set(true);
  }

  updateExistingHearing(hearing: Hearing): void {
    this.selectedHearingId = hearing.id;
    this.showUpdateHearing.set(true);
  }

  onHearingSaved(): void {
    this.showCreateHearing.set(false);
    this.showUpdateHearing.set(false);
    this.loadHearings(this.officeId, this.caseId);
  }

  onHearingCancelled(): void {
    this.showCreateHearing.set(false);
    this.showUpdateHearing.set(false);
  }

  getStatusLabel(held: boolean): string {
    return held ? 'Held' : 'Not held';
  }
}
