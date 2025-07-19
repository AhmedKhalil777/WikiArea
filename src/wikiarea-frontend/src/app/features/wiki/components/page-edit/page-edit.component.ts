import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { WikiService } from '../../../../core/services/wiki.service';
import { WikiPage } from '../../../../core/models/wiki.models';
import { MarkdownEditorComponent } from '../../../../shared/components/markdown-editor/markdown-editor.component';

@Component({
  selector: 'app-page-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MarkdownEditorComponent
  ],
  template: `
    <div class="page-edit-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Edit Wiki Page</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="pageForm" (ngSubmit)="onSubmit()" *ngIf="!loading">
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
              <button mat-raised-button color="primary" type="submit" [disabled]="pageForm.invalid || isSubmitting">
                {{ isSubmitting ? 'Saving...' : 'Save Changes' }}
              </button>
            </div>
          </form>
          
          <div class="loading" *ngIf="loading">
            Loading page...
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .page-edit-container {
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

    .loading {
      text-align: center;
      padding: 20px;
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
export class PageEditComponent implements OnInit {
  pageForm: FormGroup;
  isSubmitting = false;
  loading = true;
  pageId: string | null = null;
  page: WikiPage | null = null;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
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

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.pageId = params['id'];
      if (this.pageId) {
        this.loadPage(this.pageId);
      }
    });
  }

  loadPage(id: string) {
    this.wikiService.getWikiPageById(id).subscribe({
      next: (page: any) => {
        this.page = page;
        this.pageForm.patchValue({
          title: page.title,
          content: page.content,
          tags: page.tags ? page.tags.join(', ') : ''
        });
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Error loading page:', error);
        this.snackBar.open('Failed to load page.', 'Close', { duration: 3000 });
        this.router.navigate(['/wiki']);
      }
    });
  }

  onSubmit() {
    if (this.pageForm.valid && this.pageId) {
      this.isSubmitting = true;
      const formValue = this.pageForm.value;
      
      const updateRequest = {
        id: this.pageId,
        title: formValue.title,
        content: formValue.content,
        tags: formValue.tags ? formValue.tags.split(',').map((tag: string) => tag.trim()) : [],
        isPublic: this.page?.isPublic || true,
        allowedRoles: this.page?.allowedRoles || ['Reader', 'Writer', 'Reviewer', 'Administrator'],
        folderId: this.page?.folderId
      };

      this.wikiService.updateWikiPage(updateRequest).subscribe({
        next: (response: any) => {
          this.snackBar.open('Page updated successfully!', 'Close', { duration: 3000 });
          this.router.navigate(['/wiki/page', response.slug]);
        },
        error: (error: any) => {
          console.error('Error updating page:', error);
          this.snackBar.open('Failed to update page. Please try again.', 'Close', { duration: 3000 });
          this.isSubmitting = false;
        }
      });
    }
  }

  onCancel() {
    if (this.page) {
      this.router.navigate(['/wiki/page', this.page.slug]);
    } else {
      this.router.navigate(['/wiki']);
    }
  }
} 