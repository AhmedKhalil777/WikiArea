import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated) {
    return true;
  }

  // Store the attempted URL for redirecting after login
  localStorage.setItem('returnUrl', state.url);
  
  // Redirect to login page
  router.navigate(['/auth/login']);
  return false;
};

export const roleGuard = (allowedRoles: string[]): CanActivateFn => {
  return (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isAuthenticated) {
      localStorage.setItem('returnUrl', state.url);
      router.navigate(['/auth/login']);
      return false;
    }

    const currentUser = authService.currentUser;
    if (currentUser && allowedRoles.includes(currentUser.role)) {
      return true;
    }

    // Redirect to unauthorized page or home
    router.navigate(['/']);
    return false;
  };
};

export const permissionGuard = (requiredPermissions: string[]): CanActivateFn => {
  return (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isAuthenticated) {
      localStorage.setItem('returnUrl', state.url);
      router.navigate(['/auth/login']);
      return false;
    }

    const hasPermission = requiredPermissions.some(permission => 
      authService.hasPermission(permission)
    );

    if (hasPermission) {
      return true;
    }

    // Redirect to unauthorized page or home
    router.navigate(['/']);
    return false;
  };
}; 