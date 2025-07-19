import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService, SigninRequest } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="signin-container">
      <mat-card class="signin-card">
        <mat-card-header>
          <mat-card-title>
            <mat-icon>login</mat-icon>
            Sign In
          </mat-card-title>
          <mat-card-subtitle>Welcome back to WikiArea</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="signinForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline">
              <mat-label>Username or Email</mat-label>
              <input matInput formControlName="usernameOrEmail" placeholder="Enter username or email">
              <mat-error *ngIf="signinForm.get('usernameOrEmail')?.hasError('required')">
                Username or email is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Password</mat-label>
              <input matInput [type]="hidePassword ? 'password' : 'text'" formControlName="password" placeholder="Enter password">
              <button mat-icon-button matSuffix (click)="hidePassword = !hidePassword" type="button">
                <mat-icon>{{hidePassword ? 'visibility_off' : 'visibility'}}</mat-icon>
              </button>
              <mat-error *ngIf="signinForm.get('password')?.hasError('required')">
                Password is required
              </mat-error>
            </mat-form-field>

            <div class="error-message" *ngIf="errorMessage">
              <mat-icon>error</mat-icon>
              <span>{{ errorMessage }}</span>
            </div>

            <button mat-raised-button color="primary" type="submit" 
                    [disabled]="signinForm.invalid || isLoading" class="signin-button">
              <mat-spinner diameter="20" *ngIf="isLoading"></mat-spinner>
              <span *ngIf="!isLoading">Sign In</span>
              <span *ngIf="isLoading">Signing In...</span>
            </button>
          </form>
        </mat-card-content>
        
        <mat-card-actions>
          <p>Don't have an account? 
            <a routerLink="/auth/signup" class="auth-link">Sign up here</a>
          </p>
          <p>
            <a routerLink="/auth/login" class="auth-link">← Back to all sign-in options</a>
          </p>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .signin-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      padding: 20px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }
    
    .signin-card {
      max-width: 400px;
      width: 100%;
      background: rgba(255, 255, 255, 0.95);
      border-radius: 20px;
      box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
    }
    
    mat-card-header {
      text-align: center;
      padding: 24px 24px 0;
    }
    
    mat-card-title {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      font-size: 24px;
      margin: 0;
    }
    
    mat-form-field {
      width: 100%;
      margin-bottom: 16px;
    }
    
    .signin-button {
      width: 100%;
      height: 48px;
      margin-top: 16px;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
    }
    
    .error-message {
      display: flex;
      align-items: center;
      gap: 8px;
      color: #d32f2f;
      margin: 16px 0;
      font-size: 14px;
    }
    
    mat-card-actions {
      text-align: center;
      padding: 16px 24px 24px;
    }
    
    .auth-link {
      color: #1976d2;
      text-decoration: none;
      font-weight: 500;
    }
    
    .auth-link:hover {
      text-decoration: underline;
    }
  `]
})
export class SigninComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  signinForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  hidePassword = true;

  constructor() {
    this.signinForm = this.fb.group({
      usernameOrEmail: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit() {
    if (this.signinForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      const request: SigninRequest = this.signinForm.value;

      this.authService.signin(request).subscribe({
        next: () => {
          this.isLoading = false;
          const returnUrl = localStorage.getItem('returnUrl') || '/';
          localStorage.removeItem('returnUrl');
          this.router.navigate([returnUrl]);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'Invalid credentials';
        }
      });
    }
  }
} 