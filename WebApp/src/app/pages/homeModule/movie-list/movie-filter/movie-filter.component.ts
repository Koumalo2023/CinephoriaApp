import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CinemaDto } from '@app/core/models/cinema.models';
import { MovieGenre } from '@app/core/models/enum.model';
import { FilterMoviesRequestDto } from '@app/core/models/movie.models';
import { CinemaService } from '@app/core/services/cinema.service';
import { MovieService } from '@app/core/services/movie.service';

@Component({
  selector: 'app-movie-filter',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movie-filter.component.html',
  styleUrl: './movie-filter.component.scss'
})
export class MovieFilterComponent implements OnInit {
  @Output() filterChange = new EventEmitter<FilterMoviesRequestDto>();

  // Liste des genres de films
  movieGenres = Object.values(MovieGenre).filter((genre) => typeof genre === 'string');

  // Liste des cinémas
  cinemas: CinemaDto[] = [];

  // Variables pour stocker les filtres sélectionnés
  selectedGenre: MovieGenre | null = null;
  selectedCinema: number | null = null;
  selectedDate: Date | null = null;

  constructor(private movieService: MovieService, private cinemaService: CinemaService) {}

  ngOnInit(): void {
    this.loadCinemas(); // Charge la liste des cinémas au démarrage
  }

  // Charge la liste des cinémas
  loadCinemas(): void {
    this.cinemaService.getAllCinemas().subscribe(
      (cinemas: CinemaDto[]) => {
        this.cinemas = cinemas;
      },
      (error) => {
        console.error('Error fetching cinemas:', error);
      }
    );
  }

  // Gestion des changements de sélection
  onGenreChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.selectedGenre = target.value as MovieGenre || null;
  }

  onCinemaChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.selectedCinema = target.value ? parseInt(target.value, 10) : null;
  }

  // Gestion des changements de sélection
  onDateChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedDate = input.value ? new Date(input.value) : null; // Convertir en Date
  }
  

  applyFilters(): void {
    const filters: FilterMoviesRequestDto = {
      cinemaId: this.selectedCinema,
      genre: this.selectedGenre,
      date: this.selectedDate ? new Date(this.selectedDate) : null 
    };
  
    this.filterChange.emit(filters);
  }
  
}
