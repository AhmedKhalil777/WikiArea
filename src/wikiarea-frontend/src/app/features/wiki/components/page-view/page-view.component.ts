import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { WikiService } from '../../../../core/services/wiki.service';
import { WikiPage } from '../../../../core/models/wiki.models';

@Component({
  selector: 'app-page-view',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule
  ],
  template: `
    <div class="page-view-container" *ngIf="page">
      <mat-card>
        <mat-card-header>
          <mat-card-title>{{ page.title }}</mat-card-title>
          <mat-card-subtitle>
                         Created by {{ page.author?.displayName || page.createdBy }} on {{ page.createdAt | date }}
             <span *ngIf="page.updatedAt !== page.createdAt">
               • Updated on {{ page.updatedAt | date }}
             </span>
          </mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <div class="page-actions">
            <button mat-raised-button color="primary" (click)="editPage()">
              <mat-icon>edit</mat-icon>
              Edit
            </button>
            <button mat-raised-button (click)="goBack()">
              <mat-icon>arrow_back</mat-icon>
              Back to List
            </button>
          </div>
          
          <div class="page-content" [innerHTML]="page.content"></div>
          
          <div class="page-tags" *ngIf="page.tags && page.tags.length > 0">
            <mat-chip-set>
              <mat-chip *ngFor="let tag of page.tags">{{ tag }}</mat-chip>
            </mat-chip-set>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
    
    <div class="loading" *ngIf="!page && !error">
      Loading page...
    </div>
    
    <div class="error" *ngIf="error">
      <mat-card>
        <mat-card-content>
          <p>{{ error }}</p>
          <button mat-raised-button (click)="goBack()">Go Back</button>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .page-view-container {
      max-width: 800px;
      margin: 20px auto;
      padding: 20px;
    }

    .page-actions {
      display: flex;
      gap: 10px;
      margin-bottom: 20px;
    }

    .page-content {
      margin: 20px 0;
      line-height: 1.6;
    }

    .page-tags {
      margin-top: 20px;
    }

    .loading, .error {
      text-align: center;
      margin: 50px auto;
      max-width: 400px;
    }
  `]
})
export class PageViewComponent implements OnInit {
  page: WikiPage | null = null;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private wikiService: WikiService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const slug = params['slug'];
      if (slug) {
        this.loadPage(slug);
      }
    });
  }

  loadPage(slug: string) {
    this.wikiService.getWikiPageBySlug(slug).subscribe({
      next: (page: any) => {
        this.page = page;
      },
      error: (error: any) => {
        console.error('Error loading page:', error);
        this.error = 'Page not found or failed to load.';
      }
    });
  }

  editPage() {
    if (this.page) {
      this.router.navigate(['/wiki/edit', this.page.id]);
    }
  }

  goBack() {
    this.router.navigate(['/wiki']);
  }
} 