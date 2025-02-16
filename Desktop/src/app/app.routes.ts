import { Routes } from '@angular/router';
import { AuthComponent } from './layourt/auth/auth.component';
import { DashboardComponent } from './layourt/dashboard/dashboard.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    // Redirection par défaut vers la page d'accueil
  {
    path: '',
    redirectTo: '/admin/dashboard',
    pathMatch: 'full',
  },
  {
    path: 'login',
    loadComponent: () =>
      import('@app/pages/authModule/login/login.component').then(
        (m) => m.LoginComponent
      ),
  },

  // Routes pour le module "Admin"
  {
    path: 'admin',
    component: DashboardComponent,
    children: [
      {
        path: '',
        loadChildren: () =>
          import('@app/core/routes/adminRoute/admin-routing.module').then(
            (m) => m.AdminRoutingModule
          ),
      },
    ],
  },

  // Gestion des routes non trouvées (redirection vers la page d'accueil)
  { path: '**', redirectTo: '/admin/dashboard' },
];
