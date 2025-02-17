import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@app/core/guards/auth.guard';


const routes: Routes = [


  {
    // Route pour la gestion des cinemas
    path: 'incident-list',
    loadComponent: () =>
      import('@app/pages/adminModule/manage-incident/incident-list.component').then(
        (m) => m.IncidentListComponent
      ),
  },
  {
    // Route pour la gestion des cinemas
    path: 'incident-detail/:id',
    loadComponent: () =>
      import('@app/pages/adminModule/incident-detail/incident-detail.component').then(
        (m) => m.IncidentDetailComponent
      ),
  },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule { }