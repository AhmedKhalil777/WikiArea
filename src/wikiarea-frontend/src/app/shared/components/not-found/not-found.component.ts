import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <div class="not-found-container">
      <mat-card class="not-found-card">
        <mat-card-content>
          <div class="not-found-content">
            <mat-icon class="not-found-icon">error_outline</mat-icon>
            <h1>404 - Page Not Found</h1>
            <p>The page you are looking for doesn't exist or has been moved.</p>
            <div class="actions">
              <button mat-raised-button color="primary" routerLink="/">
                <mat-icon>home</mat-icon>
                Go Home
              </button>
              <button mat-button routerLink="/search">
                <mat-icon>search</mat-icon>
                Search
              </button>
            </div>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .not-found-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: calc(100vh - 64px);
      padding: 24px;
    }
    
    .not-found-card {
      max-width: 500px;
      width: 100%;
    }
    
    .not-found-content {
      text-align: center;
      padding: 24px;
      
      .not-found-icon {
        font-size: 64px;
        width: 64px;
        height: 64px;
        color: #ff9800;
        margin-bottom: 16px;
      }
      
      h1 {
        margin: 0 0 16px;
        color: #333;
        font-size: 28px;
      }
      
      p {
        margin: 0 0 24px;
        color: #666;
        font-size: 16px;
        line-height: 1.5;
      }
      
      .actions {
        display: flex;
        gap: 12px;
        justify-content: center;
        flex-wrap: wrap;
        
        button {
          display: flex;
          align-items: center;
          gap: 8px;
        }
      }
    }
  `]
})
export class NotFoundComponent {} 