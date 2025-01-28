import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';


const routes: Routes = [
  {
    path: '',
    children: [
      // Route pour la page d'accueil
      {
        path: 'home',
        loadComponent: () =>
          import('@app/pages/homeModule/home-movie/home-movie.component').then(
            (m) => m.HomeMovieComponent
          ),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HomeRoutingModule {}