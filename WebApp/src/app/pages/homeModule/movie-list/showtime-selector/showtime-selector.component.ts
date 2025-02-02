import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { ShowtimeDto } from '@app/core/models/showtime.models';
import { MovieService } from '@app/core/services/movie.service';

@Component({
  selector: 'app-showtime-selector',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './showtime-selector.component.html',
  styleUrl: './showtime-selector.component.scss'
})
export class ShowtimeSelectorComponent implements OnChanges {
  @Input() showtimes: ShowtimeDto[] = [];
  @Input() movieId: number | null = null;
  @Output() showtimeSelected = new EventEmitter<ShowtimeDto>();

  constructor(private movieService: MovieService) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['movieId'] && this.movieId) {
      this.loadShowtimes();
    }
  }

  loadShowtimes() {
    if (this.movieId) {
      this.movieService.getMovieSessions(this.movieId).subscribe({
        next: (showtimes) => {
          this.showtimes = showtimes;
        },
        error: (err) => {
          console.error('Erreur lors du chargement des séances :', err);
        }
      });
    } else {
      this.showtimes = [];
    }
  }
  
  onShowtimeSelected(showtime: ShowtimeDto): void {
    console.log('Séance sélectionnée :', showtime);
    this.showtimeSelected.emit(showtime);
  }
}
