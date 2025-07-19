import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { WikiService } from '../../../../core/services/wiki.service';
import { MarkdownEditorComponent } from '../../../../shared/components/markdown-editor/markdown-editor.component';

@Component({
  selector: 'app-page-create',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatSnackBarModule,
    MarkdownEditorComponent
  ],
  template: `
    <div class="page-create-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Create New Wiki Page</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="pageForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Title</mat-label>
              <input matInput formControlName="title" placeholder="Enter page title">
              <mat-error *ngIf="pageForm.get('title')?.hasError('required')">
                Title is required
              </mat-error>
            </mat-form-field>

            <div class="markdown-editor-container">
              <label class="editor-label">Content *</label>
              <app-markdown-editor 
                formControlName="content"
                placeholder="Enter your wiki page content in Markdown format">
              </app-markdown-editor>
              <mat-error *ngIf="pageForm.get('content')?.hasError('required')" class="editor-error">
                Content is required
              </mat-error>
            </div>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Tags</mat-label>
              <input matInput formControlName="tags" placeholder="Enter tags (comma separated)">
            </mat-form-field>

            <div class="form-actions">
              <button mat-raised-button type="button" (click)="onCancel()">Cancel</button>
              <button mat-raised-button color="primary" type="submit" [disabled]="pageForm.invalid || isLoading">
                {{ isLoading ? 'Creating...' : 'Create Page' }}
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .page-create-container {
      max-width: 800px;
      margin: 20px auto;
      padding: 20px;
    }

    .full-width {
      width: 100%;
      margin-bottom: 15px;
    }

    .form-actions {
      display: flex;
      gap: 10px;
      justify-content: flex-end;
      margin-top: 20px;
    }

    .markdown-editor-container {
      margin-bottom: 15px;
    }

    .editor-label {
      display: block;
      font-size: 14px;
      font-weight: 500;
      margin-bottom: 8px;
      color: rgba(0, 0, 0, 0.87);
    }

    .editor-error {
      font-size: 12px;
      color: #f44336;
      margin-top: 4px;
    }
  `]
})
export class PageCreateComponent {
  pageForm: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private wikiService: WikiService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.pageForm = this.fb.group({
      title: ['', [Validators.required]],
      content: ['', [Validators.required]],
      tags: ['']
    });
  }

  onSubmit() {
    if (this.pageForm.valid) {
      this.isLoading = true;
      const formValue = this.pageForm.value;
      
      const pageData = {
        title: formValue.title,
        content: formValue.content,
        tags: formValue.tags ? formValue.tags.split(',').map((tag: string) => tag.trim()) : [],
        contentType: 'Markdown'
      };

      const createRequest = {
        title: pageData.title,
        content: pageData.content,
        contentType: pageData.contentType,
        tags: pageData.tags,
        isPublic: true,
        allowedRoles: ['Reader', 'Writer', 'Reviewer', 'Administrator']
      };

      this.wikiService.createWikiPage(createRequest).subscribe({
        next: (response: any) => {
          this.snackBar.open('Page created successfully!', 'Close', { duration: 3000 });
          this.router.navigate(['/wiki/page', response.slug]);
        },
        error: (error: any) => {
          console.error('Error creating page:', error);
          this.snackBar.open('Failed to create page. Please try again.', 'Close', { duration: 3000 });
          this.isLoading = false;
        }
      });
    }
  }

  onCancel() {
    this.router.navigate(['/wiki']);
  }
} 