import { Injectable, inject, signal } from '@angular/core';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus } from '@azure/msal-browser';
import { filter } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthStateService {
  private readonly authService = inject(MsalService);
  private readonly msalBroadcastService = inject(MsalBroadcastService);
  private readonly loggedIn = signal(false);
  readonly isLoggedIn = this.loggedIn.asReadonly();
  private isMonitoringStarted = false;

  constructor() {
  }

  refreshLoginState(): void {
    try {
      this.loggedIn.set(this.authService.instance.getAllAccounts().length > 0);
    } catch {
      this.loggedIn.set(false);
    }
  }

  startMonitoring(): void {
    if (this.isMonitoringStarted) {
      return;
    }

    this.isMonitoringStarted = true;

    this.msalBroadcastService.inProgress$
      .pipe(filter((status) => status === InteractionStatus.None))
      .subscribe(() => {
        this.refreshLoginState();
      });
  }
}