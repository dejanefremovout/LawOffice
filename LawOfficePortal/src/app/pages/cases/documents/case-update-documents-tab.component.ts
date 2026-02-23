import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DocumentService } from '../../../services/document.service';
import { OfficeService } from '../../../services/office.service';
import { ActivatedRoute } from '@angular/router';
import { Document } from '../../../models/document.model';

@Component({
  selector: 'app-case-update-documents-tab',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './case-update-documents-tab.component.html',
  styleUrl: './case-update-documents-tab.component.scss'
})
export class CaseUpdateDocumentsTabComponent implements OnInit {
  private documents = signal<Document[]>([]);
  private loading = signal<boolean>(false);
  private error = signal<string | null>(null);
  public caseId!: string;
  public officeId!: string;
  public selectedDocumentId!: string;

  readonly documentsList = this.documents.asReadonly();
  readonly isLoading = this.loading.asReadonly();
  readonly errorMessage = this.error.asReadonly();
  readonly hasDocuments = () => this.documentsList().length > 0;

  constructor(
    private documentService: DocumentService,
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
    this.loadDocuments(officeId, caseId);
  }

  private loadDocuments(officeId: string, caseId: string): void {
    this.loading.set(true);
    this.error.set(null);
    this.documentService.getDocuments(officeId, caseId).subscribe({
      next: (data) => {
        this.documents.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load documents: ' + (err?.message || 'Unknown error'));
        this.loading.set(false);
      }
    });
  }

  downloadDocument(document: Document): void {
    if(!document) {
      return;
    }

    this.documentService.getDocument(document.officeId, document.id).subscribe({
        next: (document) => {
          this.documentService.downloadFileFromBlob(document.uri!, document.name);
          
          // this.documentService.downloadFileFromBlob(document.uri!, document.name).subscribe({
          //   next: () => {
          //     this.loadDocuments(this.officeId, this.caseId);
          //     this.loading.set(false);
          //   },
          //   error: (err) => {
          //     this.loading.set(false);
          //     let errorMsg = 'Failed to upload document.';

          //     if (err.error?.message) {
          //       errorMsg += err.error.message;
          //     } else if (err.error?.title) {
          //       errorMsg += err.error.title;
          //     } else if (err.message) {
          //       errorMsg += err.message;
          //     } else {
          //       errorMsg += 'Please try again.';
          //     }

          //     this.error.set(errorMsg);
          //     console.error('Error uploading document:', err);
          //   }
          // });
        },
        error: (err) => {
          this.loading.set(false);
          let errorMsg = 'Failed to save document. ';

          if (err.error?.message) {
            errorMsg += err.error.message;
          } else if (err.error?.title) {
            errorMsg += err.error.title;
          } else if (err.message) {
            errorMsg += err.message;
          } else {
            errorMsg += 'Please try again.';
          }

          this.error.set(errorMsg);
          console.error('Error creating document:', err);
        }
      });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) {
      return;
    }
    const file = input.files[0];

    if (file) {
      const newDocument = {
        caseId: this.caseId,
        officeId: this.officeId,
        name: file.name,
      };

      this.documentService.createDocument(newDocument).subscribe({
        next: (document) => {
          this.documentService.uploadFileToBlob(document.uri!, file, file.type).subscribe({
            next: () => {
              this.loadDocuments(this.officeId, this.caseId);
              this.loading.set(false);
            },
            error: (err) => {
              this.loading.set(false);
              let errorMsg = 'Failed to upload document.';

              if (err.error?.message) {
                errorMsg += err.error.message;
              } else if (err.error?.title) {
                errorMsg += err.error.title;
              } else if (err.message) {
                errorMsg += err.message;
              } else {
                errorMsg += 'Please try again.';
              }

              this.error.set(errorMsg);
              console.error('Error uploading document:', err);
            }
          });
        },
        error: (err) => {
          this.loading.set(false);
          let errorMsg = 'Failed to save document. ';

          if (err.error?.message) {
            errorMsg += err.error.message;
          } else if (err.error?.title) {
            errorMsg += err.error.title;
          } else if (err.message) {
            errorMsg += err.message;
          } else {
            errorMsg += 'Please try again.';
          }

          this.error.set(errorMsg);
          console.error('Error creating document:', err);
        }
      });
    }
  }
}

