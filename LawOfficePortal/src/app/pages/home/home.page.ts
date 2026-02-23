import { Component, inject } from '@angular/core';
import { OfficeService } from '../../services/office.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home.page.html',
  styleUrl: './home.page.scss'
})
export class HomePageComponent {
  // Inject the office service
  protected readonly officeService = inject(OfficeService);

  // Access the office ID signal directly in the component
  protected readonly officeId = this.officeService.officeId;
  protected readonly hasOfficeId = this.officeService.hasOfficeId;
}
