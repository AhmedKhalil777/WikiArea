import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule],
  template: `
    <div class="admin-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>
            <mat-icon>admin_panel_settings</mat-icon>
            Administration
          </mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <p>Welcome to the administration panel. Here you can manage users, content, and system settings.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .admin-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 24px;
    }
    
    mat-card-header mat-card-title {
      display: flex;
      align-items: center;
      gap: 8px;
    }
  `]
})
export class AdminComponent {} 