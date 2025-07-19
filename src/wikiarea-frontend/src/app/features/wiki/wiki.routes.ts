import { Routes } from '@angular/router';

export const wikiRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./components/page-list/page-list.component').then(c => c.PageListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./components/page-create/page-create.component').then(c => c.PageCreateComponent)
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./components/page-edit/page-edit.component').then(c => c.PageEditComponent)
  },
  {
    path: 'page/:slug',
    loadComponent: () => import('./components/page-view/page-view.component').then(c => c.PageViewComponent)
  }
]; 