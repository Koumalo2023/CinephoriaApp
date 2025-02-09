import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@app/core/guards/auth.guard';


const routes: Routes = [
  
   
   {
    // Route pour l'administration'
    path: 'dashboard',
    loadComponent: () =>
      import('@app/pages/adminModule/admin-dashboard/admin-dashboard.component').then(
        (m) => m.AdminDashboardComponent
      ),
  },
  {
    // Route pour la gestion des cinemas
    path: 'manage-cinema',
    loadComponent: () =>
      import('@app/pages/adminModule/manage-cinema/manage-cinema.component').then(
        (m) => m.ManageCinemaComponent
      ),
  },
  {
    // Route pour la gestion des films et les séances et validation des avis
    path: 'manage-movie',
    loadComponent: () =>
      import('@app/pages/adminModule/manage-movie/manage-movie.component').then(
        (m) => m.ManageMovieComponent
      ),
  },
  {
    // Route pour la gestion des utilisateurs et leurs réservations
    path: 'manage-reservation',
    loadComponent: () =>
      import('@app/pages/adminModule/manage-reservation/manage-reservation.component').then(
        (m) => m.ManageReservationComponent
      ),
  },
  {
    // Route pour la gestion des employés
    path: 'manage-employee',
    loadComponent: () =>
      import('@app/pages/adminModule/manage-employee/manage-employee.component').then(
        (m) => m.ManageEmployeeComponent
      ),
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}