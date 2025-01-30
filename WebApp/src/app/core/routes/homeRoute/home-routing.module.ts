import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';


const routes: Routes = [
  { 
    path: 'home', 
    loadComponent: () => import('@app/pages/homeModule/home-movie/home-movie.component').then(m => m.HomeMovieComponent) 
  },
  { 
    path: 'movie-list', 
    loadComponent: () => import('@app/pages/homeModule/movie-list/movie-list.component').then(m => m.MovieListComponent) 
  },
  { 
    path: 'movie-details/:movieId', 
    loadComponent: () => import('@app/pages/homeModule/movie-details/movie-details.component').then(m => m.MovieDetailsComponent) 
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HomeRoutingModule {}