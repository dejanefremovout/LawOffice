import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OfficeUpdateForm } from '../office-update-form.model';
import { Client } from '../../../models/client.model';
import { OpposingParty } from '../../../models/opposing-party.model';

@Component({
  selector: 'app-office-details-tab',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './office-details-tab.component.html',
  styleUrl: './office-details-tab.component.scss'
})
export class OfficeDetailsTabComponent {
  @Input() form: OfficeUpdateForm = {
    name: '',
    address: null
  };
  @Input() isSaving = false;

  @Output() save = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  onSave(): void {
    this.save.emit();
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
