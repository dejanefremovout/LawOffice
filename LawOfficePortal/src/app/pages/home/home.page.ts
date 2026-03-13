import { Component, OnInit, inject, effect, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { AuthStateService } from '../../services/auth-state.service';
import { UserService } from '../../services/user.service';
import { PartyService } from '../../services/party.service';
import { CaseService } from '../../services/case.service';
import { Case } from '../../models/case.model';
import { CaseHearing } from '../../models/case-hearing.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home-page',
  imports: [RouterLink, DatePipe],
  templateUrl: './home.page.html',
  styleUrl: './home.page.scss'
})
export class HomePageComponent implements OnInit {
  private readonly authStateService = inject(AuthStateService);
  private readonly userService = inject(UserService);
  private readonly partyService = inject(PartyService);
  private readonly caseService = inject(CaseService);
  protected readonly isLoggedIn = this.authStateService.isLoggedIn;
  protected readonly name = signal('');
  protected readonly clientsCount = signal(0);
  protected readonly opposingPartiesCount = signal(0);
  protected readonly totalCases = signal(0);
  protected readonly activeCases = signal(0);
  protected readonly lastCases = signal<Case[]>([]);
  protected readonly casesWithUpcomingHearings = signal<CaseHearing[]>([]);

  constructor() {
    effect(() => {
      if (this.isLoggedIn()) {
        this.loadCounts();
      }
    });
  }

  async ngOnInit(): Promise<void> {
    const name = await this.userService.getName();
    this.name.set(name ?? '');
  }

  private loadCounts(): void {
    this.partyService.getCount().subscribe({
      next: (data) => {
        this.clientsCount.set(data.clientsCount);
        this.opposingPartiesCount.set(data.opposingPartiesCount);
      },
      error: (error) => {
        console.error('Failed to load party counts', error);
      }
    });

    this.caseService.getCount().subscribe({
      next: (data) => {
        this.totalCases.set(data.totalCases);
        this.activeCases.set(data.activeCases);
      },
      error: (error) => {
        console.error('Failed to load case counts', error);
      }
    });

    this.caseService.getLastCases(3).subscribe({
      next: (data) => {
        this.lastCases.set(data);
      },
      error: (error) => {
        console.error('Failed to load last cases', error);
      }
    });

    this.caseService.getUpcomingHearings(3).subscribe({
      next: (data) => {
        this.casesWithUpcomingHearings.set(data);
      },
      error: (error) => {
        console.error('Failed to load upcoming hearings', error);
      }
    });
  }
}
