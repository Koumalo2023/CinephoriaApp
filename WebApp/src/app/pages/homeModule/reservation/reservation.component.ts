import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { CinemaSelectorComponent } from './cinema-selector/cinema-selector.component';
import { MovieSelectorComponent } from './movie-selector/movie-selector.component';
import { SeatSelectorComponent } from './seat-selector/seat-selector.component';
import { ReservationConfirmationComponent } from './reservation-confirmation/reservation-confirmation.component';
import { MovieDto } from '@app/core/models/movie.models';
import { CinemaDto } from '@app/core/models/cinema.models';
import { RedirectService } from '@app/core/services/redirect.service';
import { ReservationService } from '@app/core/services/reservation.service';
import { ShowtimeSelectorComponent } from '../movie-list/showtime-selector/showtime-selector.component';
import { AuthService } from '@app/core/services/auth.service';
import { CreateReservationDto } from '@app/core/models/reservation.models';
import { ActivatedRoute } from '@angular/router';
import { CinemaService } from '@app/core/services/cinema.service';
import { MovieService } from '@app/core/services/movie.service';
import { ShowtimeService } from '@app/core/services/showtime.service';
import { DateFormatPipe } from '@app/core/pipes/date-format.pipe';

@Component({
  selector: 'app-reservation',
  standalone: true,
  imports: [CommonModule, CinemaSelectorComponent, ShowtimeSelectorComponent, MovieSelectorComponent,  SeatSelectorComponent, ReservationConfirmationComponent, DateFormatPipe],
  templateUrl: './reservation.component.html',
  styleUrl: './reservation.component.scss'
})
export class ReservationComponent {
  isLoading: boolean = false; 

  currentStep: number = 1;
  reservationDetails: any = {
    cinema: null,
    movie: null,
    session: null,
    seats: [],
    price: 0
  };

  constructor( private redirectService: RedirectService, 
    private route: ActivatedRoute,
    private cinemaService: CinemaService,
    private movieService: MovieService,
    private showtimeService: ShowtimeService 
  ) {
    // Récupérer l'étape actuelle depuis RedirectService
    this.currentStep = this.redirectService.getCurrentStep();

    // Récupérer les paramètres de requête
    this.route.queryParams.subscribe(params => {
      const showtimeId = params['showtimeId'];
      const movieId = params['movieId'];
      const cinemaId = params['cinemaId'];
      const step = params['step'];

      if (step) {
        this.currentStep = +step; 
      }

      if (showtimeId && movieId && cinemaId) {
        this.loadReservationDetails(showtimeId, movieId, cinemaId);
      }
    });
  }

  onCinemaSelected(cinema: CinemaDto) {
    this.reservationDetails.cinema = cinema;
    this.nextStep();
  }

  onMovieSelected(movie: MovieDto) {
    this.reservationDetails.movie = movie;
    this.nextStep();
  }
  // Passe l'ID du film à ShowtimeSelectorComponent
  get selectedMovieId(): number | null {
    return this.reservationDetails.movie ? this.reservationDetails.movie.movieId : null;
  }

  onSessionSelected(session: any) {
    this.reservationDetails.session = session;
    this.nextStep();
  }
  // Ajoutez cette méthode pour passer l'ID de la séance à SeatSelectorComponent
  get selectedShowtimeId(): number | null {
    return this.reservationDetails.session ? this.reservationDetails.session.showtimeId : null;
  }

  onSeatsSelected(seats: any[]) {
    this.reservationDetails.seats = seats;
    this.nextStep();
  }
  // Ajoutez cette méthode pour passer les sièges sélectionnés à SeatSelectorComponent
  get selectedSeats(): any[] {
    return this.reservationDetails.seats;
  }

  onReservationConfirmed() {
    // Envoyer les données de réservation au backend
    console.log('Réservation confirmée :', this.reservationDetails);
  }

  // reservation.component.ts
private loadReservationDetails(showtimeId: number, movieId: number, cinemaId: number): void {
  this.isLoading = true;
  // Charger les détails du cinéma
  this.cinemaService.getCinemaById(cinemaId).subscribe(cinema => {
    this.reservationDetails.cinema = cinema;
    this.isLoading = false;
  });

  // Charger les détails du film
  this.movieService.getMovieDetails(movieId).subscribe(movie => {
    this.reservationDetails.movie = movie;
    this.isLoading = false;
  });

  // Charger les détails de la séance
  this.showtimeService.getShowtimeDetails(showtimeId).subscribe(session => {
    this.reservationDetails.session = session;
    this.isLoading = false;
  });
}

  nextStep() {
    if (this.currentStep < 5) {
      this.currentStep++;
      this.redirectService.setCurrentStep(this.currentStep); 
    }
  }

  previousStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
      this.redirectService.setCurrentStep(this.currentStep);
    }
  }

  // Méthode pour mettre à jour les détails de la réservation
  updateReservationDetails(details: any) {
    this.reservationDetails = { ...this.reservationDetails, ...details };
  }

}
