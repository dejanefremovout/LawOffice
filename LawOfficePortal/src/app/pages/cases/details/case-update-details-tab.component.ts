import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CaseUpdateForm } from '../case-update-form.model';
import { Client } from '../../../models/client.model';
import { OpposingParty } from '../../../models/opposing-party.model';

@Component({
  selector: 'app-case-update-details-tab',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './case-update-details-tab.component.html',
  styleUrl: './case-update-details-tab.component.scss'
})
export class CaseUpdateDetailsTabComponent {
  @Input() form: CaseUpdateForm = {
    identificationNumber: '',
    description: null,
    year: null,
    city: null,
    competentCourt: null,
    judge: null,
    active: true,
    clientIds: [],
    opposingPartyIds: []
  };
  @Input() clients: Client[] = [];
  @Input() opposingParties: OpposingParty[] = [];
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
