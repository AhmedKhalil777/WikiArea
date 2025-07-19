import { Routes } from '@angular/router';

export const authRoutes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./components/login/login.component').then(c => c.LoginComponent)
  },
  {
    path: 'signin',
    loadComponent: () => import('./components/signin/signin.component').then(c => c.SigninComponent)
  },
  {
    path: 'signup',
    loadComponent: () => import('./components/signup/signup.component').then(c => c.SignupComponent)
  },
  {
    path: 'callback',
    loadComponent: () => import('./components/login/login.component').then(c => c.LoginComponent)
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  }
]; 