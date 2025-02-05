import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MovieDto } from '@app/core/models/movie.models';
import { EnumService } from '@app/core/services/enum.service';

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
  @Input() imageMaxHeight: string = '200px';
  @Output() movieSelected = new EventEmitter<number>();
  Array = Array;
  constructor(private router: Router, public enumService: EnumService) {}

  get truncatedDescription(): string {
    return this.movie.description.length > 120
      ? this.movie.description.slice(0, 120) + '...'
      : this.movie.description;
  }

  showMovieDetails(): void {
    if (this.movie?.movieId) {
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
  getFormattedGenre(genre: number | string): string {
    if (typeof genre === 'number') {
      const genresArray = Object.values(EnumService.MovieGenre);
      return genresArray[genre] ?? 'Inconnu';
    }
    return EnumService.MovieGenre[genre as keyof typeof EnumService.MovieGenre] ?? 'Inconnu';
  }
  
  

  private navigateToMovieDetails(): void {
    if (this.movie?.movieId) {
      this.router.navigateByUrl(`/home/movie-details/${this.movie.movieId}`);
    } else {
      console.error('❌ Erreur : ID du film non valide.');
    }
  }

  getStarRating(score: number): { fullStars: number; halfStar: boolean; emptyStars: number } {
    const fullStars = Math.floor(score);  
    const hasHalfStar = score - fullStars >= 0.5; 
    const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);
  
    return {
      fullStars,
      halfStar: hasHalfStar,
      emptyStars,
    };
  }

  getMainPosterUrl(): string {
    if (this.movie?.posterUrls && this.movie.posterUrls.length > 0) {
      const mainUrl = this.movie.posterUrls[0];
      return mainUrl;
    }
    console.log('No poster URLs found, returning default image.');
    return 'https://upload.wikimedia.org/wikipedia/commons/6/65/No-Image-Placeholder.svg';
  }

  private emitMovieSelected(): void {
    if (this.movie?.movieId) {
      this.movieSelected.emit(this.movie.movieId);
    } else {
      console.error('❌ Erreur : ID du film non valide.');
    }
  }
}
