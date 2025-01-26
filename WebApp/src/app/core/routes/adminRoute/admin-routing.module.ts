import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@app/core/guards/auth.guard';


const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard], 
    data: { expectedRole: 'Admin, Employee' }, 
    children: [
      // Route pour la gestion des films
      {
        path: 'dashboard',
        loadComponent: () =>
          import('@app/pages/adminModule/admin-dashboard/admin-dashboard.component').then(
            (m) => m.AdminDashboardComponent
          ),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}