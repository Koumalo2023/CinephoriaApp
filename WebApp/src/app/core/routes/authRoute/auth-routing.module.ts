import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

/**
 * Configuration des routes pour le module d'authentification.
 * Ces routes sont accessibles via le chemin `/auth`.
 */
const routes: Routes = [
  {
    path: '',
    children: [
      // Route pour la page de connexion
      {
        path: 'login',
        loadComponent: () =>
          import('@app/pages/authModule/login/login.component').then(
            (m) => m.LoginComponent
          ),
      },

      // Route pour la page d'inscription
      {
        path: 'register',
        loadComponent: () =>
          import('@app/pages/authModule/register/register.component').then(
            (m) => m.RegisterComponent
          ),
      },

      // Route pour la page de réinitialisation du mot de passe
      {
        path: 'reset-password',
        loadComponent: () =>
          import('@app/pages/authModule/resset-password/resset-password.component').then(
            (m) => m.ResetPasswordComponent
          ),
      },

      // Route pour la page de demande de réinitialisation du mot de passe
      {
        path: 'forgot-password',
        loadComponent: () =>
          import('@app/pages/authModule/forgot-password/forgot-password.component').then(
            (m) => m.ForgotPasswordComponent
          ),
      },
      // Route pour la page de confirmation du mot de passe
      {
        path: 'confirm-email',
        loadComponent: () =>
          import('@app/pages/authModule/confirm-email/confirm-email.component').then(
            (m) => m.ConfirmEmailComponent
          ),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}