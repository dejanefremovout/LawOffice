import { Component, signal, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { OfficeService } from './services/office.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  private readonly officeService = inject(OfficeService);
  protected readonly isMenuCollapsed = signal(false);

  constructor() {
    // Initialize office ID on app start - TODO: Move to login flow
    if (!this.officeService.hasOfficeId()) {
      this.officeService.setOfficeId('59769a1c-3e26-4523-a6b5-6040e5b49edb');
    }
  }

  toggleMenu(): void {
    this.isMenuCollapsed.update((value) => !value);
  }
}
