import { Component, OnInit, inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatDividerModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  public authService = inject(AuthService); // Make public for template access
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  isLoading = false;
  error: string | null = null;

  ngOnInit() {
    // Check for authorization code in URL (callback from ADFS)
    const code = this.route.snapshot.queryParams['code'];
    if (code) {
      this.handleCallback(code);
      return;
    }

    // Check if user is already authenticated
    if (this.authService.isAuthenticated) {
      this.redirectAfterLogin();
      return;
    }
  }

  // Navigation methods for local authentication
  navigateToSignin() {
    this.router.navigate(['/auth/signin']);
  }

  navigateToSignup() {
    this.router.navigate(['/auth/signup']);
  }

  // Logout
  logout() {
    this.authService.logout();
    this.error = null;
  }

  // ADFS authentication
  loginWithADFS() {
    this.isLoading = true;
    this.error = null;
    
    try {
      this.authService.login();
    } catch (error) {
      this.error = 'Failed to initiate ADFS login. Please try again.';
      this.isLoading = false;
    }
  }

  private handleCallback(code: string) {
    this.isLoading = true;
    
    this.authService.handleCallback(code).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.redirectAfterLogin();
      },
      error: (error) => {
        this.isLoading = false;
        this.error = 'Authentication failed. Please try again.';
        console.error('Auth callback error:', error);
      }
    });
  }

  private redirectAfterLogin() {
    const returnUrl = localStorage.getItem('returnUrl') || '/';
    localStorage.removeItem('returnUrl');
    this.router.navigate([returnUrl]);
  }
} 