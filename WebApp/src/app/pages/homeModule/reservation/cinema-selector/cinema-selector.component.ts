import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { CinemaDto } from '@app/core/models/cinema.models';
import { CinemaService } from '@app/core/services/cinema.service';
import { CinemaCardComponent } from '@app/layourt/sharedComponents/cinema-card/cinema-card.component';

@Component({
  selector: 'app-cinema-selector',
  standalone: true,
  imports: [CommonModule, CinemaCardComponent],
  templateUrl: './cinema-selector.component.html',
  styleUrl: './cinema-selector.component.scss'
})
export class CinemaSelectorComponent {
  @Output() cinemaSelected = new EventEmitter<CinemaDto>();

  cinemas: CinemaDto[] = []; 
  selectedCinemaId: number | null = null;
  selectedCinema: CinemaDto | null = null;

  constructor(private cinemaService: CinemaService) {}

  // Méthode pour charger les cinémas disponibles
  ngOnInit() {
    this.loadCinemas();
  }

  
  loadCinemas() {
    this.cinemaService.getAllCinemas().subscribe({
      next: (cinemas) => {
        this.cinemas = cinemas;
      },
      error: (err) => {
        console.error('Erreur lors du chargement des cinémas :', err);
      }
    });
  }

  // Méthode appelée lorsque l'utilisateur sélectionne un cinéma
  onCinemaChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    const cinemaId = Number(target.value); // Convertit l'ID en nombre
    this.selectedCinemaId = cinemaId;

    // Récupère les détails du cinéma sélectionné
    if (cinemaId) {
      this.cinemaService.getCinemaById(cinemaId).subscribe({
        next: (cinema) => {
          this.cinemaSelected.emit(cinema); // Émet les détails du cinéma sélectionné
        },
        error: (err) => {
          console.error('Erreur lors de la récupération du cinéma :', err);
        }
      });
    }
  }
}
