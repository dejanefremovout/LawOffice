import { Component, signal, inject, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { OfficeService } from './services/office.service';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus } from '@azure/msal-browser';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  private readonly officeService = inject(OfficeService);
  private readonly authService = inject(MsalService);
  private readonly msalBroadcastService = inject(MsalBroadcastService);
  protected readonly isMenuCollapsed = signal(false);
  protected readonly isLoggedIn = signal(false);

  constructor() {
    // Initialize office ID on app start - TODO: Move to login flow
    if (!this.officeService.hasOfficeId()) {
      this.officeService.setOfficeId('59769a1c-3e26-4523-a6b5-6040e5b49edb');
    }
  }

  toggleMenu(): void {
    this.isMenuCollapsed.update((value) => !value);
  }

  async ngOnInit(): Promise<void> {
    await this.authService.instance.initialize();

    try {
      await this.authService.instance.handleRedirectPromise();
    } catch (error) {
      console.error('Redirect error:', error);
    }

    this.updateLoginState();

    this.msalBroadcastService.inProgress$
      .pipe(filter((status) => status === InteractionStatus.None))
      .subscribe(() => this.updateLoginState());
  }

  private updateLoginState(): void {
    this.isLoggedIn.set(this.authService.instance.getAllAccounts().length > 0);
  }

  login() {
    this.authService.loginRedirect();
  }

  logout() {
    this.authService.logoutRedirect();
  }
}
