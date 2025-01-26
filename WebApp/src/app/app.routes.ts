import { Routes } from '@angular/router';
import { HomeComponent } from './layourt/home/home.component';
import { AuthComponent } from './layourt/auth/auth.component';
import { DashboardComponent } from './layourt/dashboard/dashboard.component';


export const routes: Routes = [
  // Redirection par défaut vers la page d'accueil
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },

  // Routes pour le module "Home"
  {
    path: 'home',
    component: HomeComponent, 
    children: [
      {
        path: '',
        loadChildren: () =>
          import('@app/core/routes/homeRoute/home.module').then(
            (m) => m.HomeModule
          ), // Charge le module Home de manière asynchrone
      },
    ],
  },

  // Routes pour le module "Auth" (Authentification)
  {
    path: 'auth',
    component: AuthComponent, 
    children: [
      {
        path: '',
        loadChildren: () =>
          import('@app/core/routes/authRoute/auth-routing.module').then(
            (m) => m.AuthRoutingModule
          ), // Charge le module Auth de manière asynchrone
      },
    ],
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
          ), // Charge le module Admin de manière asynchrone
      },
    ],
  },

  // Gestion des routes non trouvées (redirection vers la page d'accueil)
  { path: '**', redirectTo: '/home' },
];