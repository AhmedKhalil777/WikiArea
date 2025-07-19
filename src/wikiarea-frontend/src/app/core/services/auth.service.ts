import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { User } from '../models/wiki.models';

export interface AuthResponse {
  token: string;
  user: User;
  expiresAt: Date;
}

export interface SignupRequest {
  username: string;
  email: string;
  password: string;
  displayName: string;
  department: string;
}

export interface SigninRequest {
  usernameOrEmail: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = `${environment.apiUrl}/api`;

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private tokenSubject = new BehaviorSubject<string | null>(null);
  public token$ = this.tokenSubject.asObservable();

  constructor() {
    this.loadStoredAuth();
  }

  private loadStoredAuth(): void {
    const token = localStorage.getItem('auth_token');
    const userJson = localStorage.getItem('current_user');
    
    if (token && userJson) {
      try {
        const user = JSON.parse(userJson);
        this.tokenSubject.next(token);
        this.currentUserSubject.next(user);
      } catch (error) {
        this.clearStoredAuth();
      }
    }
  }

  private storeAuth(token: string, user: User): void {
    localStorage.setItem('auth_token', token);
    localStorage.setItem('current_user', JSON.stringify(user));
  }

  private clearStoredAuth(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('current_user');
  }

  login(): void {
    // Redirect to ADFS login
    window.location.href = `${environment.adfsUrl}/oauth2/authorize?response_type=code&client_id=${environment.clientId}&redirect_uri=${environment.redirectUri}&scope=openid%20profile%20email`;
  }

  handleCallback(code: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/callback`, { code })
      .pipe(
        tap(response => {
          this.tokenSubject.next(response.token);
          this.currentUserSubject.next(response.user);
          this.storeAuth(response.token, response.user);
        })
      );
  }

  logout(): void {
    this.tokenSubject.next(null);
    this.currentUserSubject.next(null);
    this.clearStoredAuth();
    this.router.navigate(['/auth/login']);
  }

  get isAuthenticated(): boolean {
    return !!this.tokenSubject.value;
  }

  get currentUser(): User | null {
    return this.currentUserSubject.value;
  }

  get token(): string | null {
    return this.tokenSubject.value;
  }

  hasRole(role: string): boolean {
    const user = this.currentUser;
    return user ? user.role === role : false;
  }

  hasPermission(permission: string): boolean {
    const user = this.currentUser;
    return user ? user.permissions.includes(permission) || user.permissions.includes('*') : false;
  }

  // Local authentication methods
  signup(request: SignupRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/signup`, request)
      .pipe(
        tap(response => {
          this.tokenSubject.next(response.token);
          this.currentUserSubject.next(response.user);
          this.storeAuth(response.token, response.user);
        })
      );
  }

  signin(request: SigninRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/signin`, request)
      .pipe(
        tap(response => {
          this.tokenSubject.next(response.token);
          this.currentUserSubject.next(response.user);
          this.storeAuth(response.token, response.user);
        })
      );
  }
}
