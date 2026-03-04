import { Component, effect, signal, inject, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { AuthStateService } from './services/auth-state.service';
import { UserService } from './services/user.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  private readonly authService = inject(MsalService);
  private readonly authStateService = inject(AuthStateService);
  private readonly userService = inject(UserService);
  protected readonly isMenuCollapsed = signal(false);
  protected readonly isLoggedIn = this.authStateService.isLoggedIn;
  protected readonly preferredUsername = signal('');

  constructor() {
    effect(() => {
      if (!this.isLoggedIn()) {
        this.preferredUsername.set('');
        return;
      }

      void this.updateUserClaims();
    });
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

    this.authStateService.startMonitoring();
    this.authStateService.refreshLoginState();
  }

  private async updateUserClaims(): Promise<void> {
    if (!this.isLoggedIn()) {
      this.preferredUsername.set('');
      return;
    }

    const preferredUsername = await this.userService.getPreferredUsername();
    this.preferredUsername.set(preferredUsername ?? '');
  }

  login() {
    this.authService.loginRedirect({
      scopes: ['openid', 'profile', 'api://a9a5990c-f11e-49df-a582-a2c1416456cf/access_as_user']
    });
  }

  logout() {
    this.authService.logoutRedirect();
  }
}
