import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Get the auth token
  const token = authService.token;

  // Clone the request and add the authorization header if token exists
  let authReq = req;
  if (token) {
    authReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
  }

  // Handle the request and catch authentication errors
  return next(authReq).pipe(
    catchError(error => {
      // If we get a 401 Unauthorized response, logout the user
      if (error.status === 401) {
        authService.logout();
        router.navigate(['/auth/login']);
      }
      
      return throwError(() => error);
    })
  );
}; 