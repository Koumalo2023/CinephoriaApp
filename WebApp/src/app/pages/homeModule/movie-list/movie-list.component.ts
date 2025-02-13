import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FilterMoviesRequestDto, MovieDto } from '@app/core/models/movie.models';
import { MovieService } from '@app/core/services/movie.service';
import { MovieCardComponent } from '@app/layourt/sharedComponents/movie-card/movie-card.component';
import { Router } from '@angular/router';
import { MovieFilterComponent } from './movie-filter/movie-filter.component';

@Component({
  selector: 'app-movie-list',
  standalone: true,
  imports: [CommonModule, MovieCardComponent, MovieFilterComponent],
  templateUrl: './movie-list.component.html',
  styleUrl: './movie-list.component.scss'
})
export class MovieListComponent implements OnInit {
  @Input() showRecentMoviesOnly: boolean = false; 
  @Input() showFilter: boolean = true; 
  isLoading: boolean = false; 
  movies: MovieDto[] = [];
  filteredMovies: MovieDto[] = [];
  selectedMovieId: number | null = null;

  constructor(private movieService: MovieService, private router: Router) {}

  ngOnInit(): void {
    this.loadMovies();
  }

  // Charge les films en fonction des filtres
  loadMovies(filters: FilterMoviesRequestDto = { cinemaId: null, genre: null, date: null }): void {
    this.isLoading = true;
    this.movieService.filterMovies(filters).subscribe(
      (movies: MovieDto[]) => {
        this.filteredMovies = movies;
        this.isLoading = false;
      },
      (error) => {
        console.error('Error fetching movies:', error);
        this.isLoading = false;
      }
    );
  }

  // Méthode appelée lorsque les filtres changent
  onFilterChange(filters: FilterMoviesRequestDto): void {
    this.loadMovies(filters); 
  }

  
  onMovieSelected(movieId: number): void {
    this.router.navigate(['/movie', movieId]);
  }


}
