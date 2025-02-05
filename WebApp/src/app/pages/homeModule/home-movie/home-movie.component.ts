import { Component} from '@angular/core';
import { MovieDto } from '@app/core/models/movie.models';
import { Router } from '@angular/router';
import { MovieCardComponent } from '@app/layourt/sharedComponents/movie-card/movie-card.component';
import { CommonModule } from '@angular/common';
import { MovieService } from '@app/core/services/movie.service';
import { ContainerComponent } from '@app/layourt/sharedComponents/container/container.component';

@Component({
  selector: 'app-home-movie',
  standalone: true,
  imports: [CommonModule, MovieCardComponent, ContainerComponent],
  templateUrl: './home-movie.component.html',
  styleUrl: './home-movie.component.scss'
})
export class HomeMovieComponent  {
  movies: MovieDto[] = []; 
  isLoading: boolean = false; 
  isLoggedIn: boolean = false; 

  constructor(private router: Router, private movieService: MovieService) {}

  ngOnInit(): void {
    this.loadRecentMovies();
  }

  loadRecentMovies(): void {
    this.isLoading = true;
    this.movieService.getRecentMovies().subscribe(
      (movies) => {
        this.movies = movies;
        this.isLoading = false;
      },
      (error) => {
        console.error('Error fetching recent movies:', error);
        this.isLoading = false;
      }
    );
  }

  showMovieDetails(movieId: number): void {
    this.router.navigate(['/movie', movieId]);
  }
}
