import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'login',
        loadComponent: () => import('@app/pages/authModule/login/login.component').then((m) => m.LoginComponent),
      },
      {
        path: 'register',
        loadComponent: () => import('@app/pages/authModule/register/register.component').then((m) => m.RegisterComponent),
      },
      {
        path: 'reset-password',
        loadComponent: () => import('@app/pages/authModule/resset-password/resset-password.component').then((m) => m.RessetPasswordComponent),
      },
      {
        path: 'forgot-password',
        loadComponent: () => import('@app/pages/authModule/forgot-password/forgot-password.component').then((m) => m.ForgotPasswordComponent),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}