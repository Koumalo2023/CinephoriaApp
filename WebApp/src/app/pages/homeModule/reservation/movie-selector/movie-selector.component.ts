import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { MovieDto } from '@app/core/models/movie.models';
import { MovieService } from '@app/core/services/movie.service';
import { MovieCardComponent } from '@app/layourt/sharedComponents/movie-card/movie-card.component';
import { MovieFilterComponent } from '../../movie-list/movie-filter/movie-filter.component';

@Component({
  selector: 'app-movie-selector',
  standalone: true,
  imports: [CommonModule, MovieCardComponent],
  templateUrl: './movie-selector.component.html',
  styleUrl: './movie-selector.component.scss'
})
export class MovieSelectorComponent implements OnInit, OnChanges {
  @Input() cinemaId: number | null = null; // Reçoit l'ID du cinéma sélectionné
  @Output() movieSelected = new EventEmitter<MovieDto>(); // Émet un événement avec les détails du film sélectionné

  movies: MovieDto[] = []; // Liste des films disponibles

  constructor(private movieService: MovieService) {}

  // Méthode appelée lors de l'initialisation du composant
  ngOnInit() {
    this.loadMovies();
  }

  // Méthode appelée lorsque l'ID du cinéma change
  ngOnChanges(changes: SimpleChanges) {
    if (changes['cinemaId'] && changes['cinemaId'].currentValue) {
      this.loadMovies();
    }
  }

  // Charge la liste des films disponibles pour le cinéma sélectionné
  loadMovies() {
    if (this.cinemaId) {
      this.movieService.getMoviesByCinemaId(this.cinemaId).subscribe({
        next: (movies) => {
          this.movies = movies;
        },
        error: (err) => {
          console.error('Erreur lors du chargement des films :', err);
        }
      });
    } else {
      this.movies = []; // Réinitialise la liste des films si aucun cinéma n'est sélectionné
    }
  }

  // Méthode appelée lorsque l'utilisateur sélectionne un film via MovieCardComponent
  onMovieSelected(movieId: number) {
    const selectedMovie = this.movies.find(movie => movie.movieId === movieId);
    if (selectedMovie) {
      this.movieSelected.emit(selectedMovie); // Émet les détails du film sélectionné
    }
  }
}
