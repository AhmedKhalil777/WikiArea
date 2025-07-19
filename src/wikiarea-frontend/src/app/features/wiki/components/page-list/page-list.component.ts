import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { WikiService } from '../../../../core/services/wiki.service';
import { WikiPageSummary } from '../../../../core/models/wiki.models';

@Component({
  selector: 'app-page-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, RouterModule],
  template: `
    <div class="page-list-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>
            <mat-icon>book</mat-icon>
            Wiki Pages
          </mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <p>Welcome to WikiArea! Your knowledge management system.</p>
          <div class="actions">
            <button mat-raised-button color="primary" routerLink="/search">
              <mat-icon>search</mat-icon>
              Search Pages
            </button>
            <button mat-raised-button color="accent" routerLink="/wiki/create">
              <mat-icon>add</mat-icon>
              Create Page
            </button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .page-list-container {
      max-width: 1000px;
      margin: 0 auto;
      padding: 32px 24px;
    }
    
    mat-card {
      border-radius: 16px !important;
      box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08) !important;
      background: linear-gradient(145deg, #ffffff 0%, #fafafa 100%);
      border: 1px solid rgba(0, 0, 0, 0.04);
    }
    
    mat-card-header mat-card-title {
      display: flex;
      align-items: center;
      gap: 12px;
      font-size: 28px;
      font-weight: 500;
      color: #1a1a1a;
    }
    
    mat-card-content p {
      font-size: 16px;
      color: #666;
      line-height: 1.6;
      margin-bottom: 24px;
    }
    
    .actions {
      display: flex;
      gap: 16px;
      margin-top: 24px;
      flex-wrap: wrap;
    }
    
    .actions button {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 12px 24px;
      border-radius: 12px !important;
      font-weight: 500;
      text-transform: none;
      transition: all 0.2s ease;
    }
    
    .actions button:hover {
      transform: translateY(-1px);
      box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15) !important;
    }
    
    @media (max-width: 600px) {
      .page-list-container {
        padding: 16px;
      }
      
      .actions {
        flex-direction: column;
      }
      
      .actions button {
        justify-content: center;
      }
    }
  `]
})
export class PageListComponent implements OnInit {
  private wikiService = inject(WikiService);
  
  pages: WikiPageSummary[] = [];

  ngOnInit() {
    // Load recent pages
    this.wikiService.getRecentWikiPages(10).subscribe({
      next: (pages) => {
        this.pages = pages;
      },
      error: (error) => {
        console.error('Error loading pages:', error);
      }
    });
  }
} 