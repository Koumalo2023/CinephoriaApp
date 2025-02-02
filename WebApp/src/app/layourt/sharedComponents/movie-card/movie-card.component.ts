import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MovieDto } from '@app/core/models/movie.models';

@Component({
  selector: 'app-movie-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movie-card.component.html',
  styleUrl: './movie-card.component.scss'
})
export class MovieCardComponent {
  @Input() movie!: MovieDto;
  @Input() mode: 'details' | 'sessions' = 'details';
  @Output() movieSelected = new EventEmitter<number>();

  constructor(private router: Router) {}

  get truncatedDescription(): string {
    return this.movie.description.length > 120
      ? this.movie.description.slice(0, 120) + '...'
      : this.movie.description;
  }

  showMovieDetails(): void {
    if (this.movie?.movieId) {
      console.log(`🔍 Navigation vers : /home/movie-details/${this.movie.movieId}`);
      this.router.navigateByUrl(`/home/movie-details/${this.movie.movieId}`);
    } else {
      console.error('❌ Erreur : ID du film non valide.');
    }
  }

  @HostListener('click')
  onCardClick(): void {
    if (this.mode === 'details') {
      this.navigateToMovieDetails();
    } else if (this.mode === 'sessions') {
      this.emitMovieSelected();
    }
  }

  // Méthode pour naviguer vers les détails du film
  private navigateToMovieDetails(): void {
    if (this.movie?.movieId) {
      console.log(`🔍 Navigation vers : /home/movie-details/${this.movie.movieId}`);
      this.router.navigateByUrl(`/home/movie-details/${this.movie.movieId}`);
    } else {
      console.error('❌ Erreur : ID du film non valide.');
    }
  }

  // Méthode pour émettre l'ID du film sélectionné
  private emitMovieSelected(): void {
    if (this.movie?.movieId) {
      this.movieSelected.emit(this.movie.movieId);
    } else {
      console.error('❌ Erreur : ID du film non valide.');
    }
  }
}
