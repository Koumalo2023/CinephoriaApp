import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@app/core/guards/auth.guard';


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
    loadComponent: () => import('@app/pages/homeModule/movie-list/movie-details/movie-details.component').then(m => m.MovieDetailsComponent) 
  },
  { 
    path: 'reservation', 
    loadComponent: () => import('@app/pages/homeModule/reservation/reservation.component').then(m => m.ReservationComponent) 
  },
  { 
    path: 'profile',
    canActivate: [AuthGuard], 
    data: { expectedRole: 'Admin, Employee, User' },  
    loadComponent: () => import('@app/pages/userModule/user-profile.component').then(m => m.UserProfileComponent) 
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HomeRoutingModule {}