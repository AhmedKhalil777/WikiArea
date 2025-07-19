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
import { AuthService, SignupRequest } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-signup',
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
    <div class="signup-container">
      <mat-card class="signup-card">
        <mat-card-header>
          <mat-card-title>
            <mat-icon>person_add</mat-icon>
            Create Account
          </mat-card-title>
          <mat-card-subtitle>Join WikiArea today</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="signupForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline">
              <mat-label>Username</mat-label>
              <input matInput formControlName="username" placeholder="Enter username">
              <mat-error *ngIf="signupForm.get('username')?.hasError('required')">
                Username is required
              </mat-error>
              <mat-error *ngIf="signupForm.get('username')?.hasError('minlength')">
                Username must be at least 3 characters
              </mat-error>
              <mat-error *ngIf="signupForm.get('username')?.hasError('pattern')">
                Username can only contain letters, numbers, and underscores
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" placeholder="Enter email">
              <mat-error *ngIf="signupForm.get('email')?.hasError('required')">
                Email is required
              </mat-error>
              <mat-error *ngIf="signupForm.get('email')?.hasError('email')">
                Please enter a valid email
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Display Name</mat-label>
              <input matInput formControlName="displayName" placeholder="Enter full name">
              <mat-error *ngIf="signupForm.get('displayName')?.hasError('required')">
                Display name is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Department</mat-label>
              <input matInput formControlName="department" placeholder="Enter department">
              <mat-error *ngIf="signupForm.get('department')?.hasError('required')">
                Department is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Password</mat-label>
              <input matInput [type]="hidePassword ? 'password' : 'text'" formControlName="password" placeholder="Enter password">
              <button mat-icon-button matSuffix (click)="hidePassword = !hidePassword" type="button">
                <mat-icon>{{hidePassword ? 'visibility_off' : 'visibility'}}</mat-icon>
              </button>
              <mat-error *ngIf="signupForm.get('password')?.hasError('required')">
                Password is required
              </mat-error>
              <mat-error *ngIf="signupForm.get('password')?.hasError('minlength')">
                Password must be at least 6 characters
              </mat-error>
              <mat-error *ngIf="signupForm.get('password')?.hasError('pattern')">
                Password must contain uppercase, lowercase, and number
              </mat-error>
            </mat-form-field>

            <div class="error-message" *ngIf="errorMessage">
              <mat-icon>error</mat-icon>
              <span>{{ errorMessage }}</span>
            </div>

            <button mat-raised-button color="primary" type="submit" 
                    [disabled]="signupForm.invalid || isLoading" class="signup-button">
              <mat-spinner diameter="20" *ngIf="isLoading"></mat-spinner>
              <span *ngIf="!isLoading">Create Account</span>
              <span *ngIf="isLoading">Creating Account...</span>
            </button>
          </form>
        </mat-card-content>
        
        <mat-card-actions>
          <p>Already have an account? 
            <a routerLink="/auth/signin" class="auth-link">Sign in here</a>
          </p>
          <p>
            <a routerLink="/auth/login" class="auth-link">← Back to all sign-in options</a>
          </p>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .signup-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      padding: 20px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }
    
    .signup-card {
      max-width: 500px;
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
    
    .signup-button {
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
export class SignupComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  signupForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  hidePassword = true;

  constructor() {
    this.signupForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3), Validators.pattern(/^[a-zA-Z0-9_]+$/)]],
      email: ['', [Validators.required, Validators.email]],
      displayName: ['', [Validators.required]],
      department: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$/)]]
    });
  }

  onSubmit() {
    if (this.signupForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      const request: SignupRequest = this.signupForm.value;

      this.authService.signup(request).subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/']);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'An error occurred during signup';
        }
      });
    }
  }
} 