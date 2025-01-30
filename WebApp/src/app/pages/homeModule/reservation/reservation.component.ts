import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { CinemaSelectorComponent } from './cinema-selector/cinema-selector.component';
import { MovieSelectorComponent } from './movie-selector/movie-selector.component';
import { SeatSelectorComponent } from './seat-selector/seat-selector.component';
import { ReservationConfirmationComponent } from './reservation-confirmation/reservation-confirmation.component';
import { PriceSummaryComponent } from './price-summary/price-summary.component';
import { ShowtimeSelectorComponent } from '../movie-list/showtime-selector/showtime-selector.component';
import { MovieDto } from '@app/core/models/movie.models';
import { CinemaDto } from '@app/core/models/cinema.models';

@Component({
  selector: 'app-reservation',
  standalone: true,
  imports: [CommonModule, CinemaSelectorComponent, MovieSelectorComponent,  SeatSelectorComponent, ReservationConfirmationComponent, PriceSummaryComponent, ShowtimeSelectorComponent],
  templateUrl: './reservation.component.html',
  styleUrl: './reservation.component.scss'
})
export class ReservationComponent {
  currentStep: number = 1;
  reservationDetails: any = {
    cinema: null,
    movie: null,
    session: null,
    seats: [],
    price: 0
  };

  onCinemaSelected(cinema: CinemaDto) {
    this.reservationDetails.cinema = cinema;
    this.nextStep();
  }

  onMovieSelected(movie: MovieDto) {
    this.reservationDetails.movie = movie;
    this.nextStep();
  }

  onSessionSelected(session: any) {
    this.reservationDetails.session = session;
    this.nextStep();
  }

  onSeatsSelected(seats: any[]) {
    this.reservationDetails.seats = seats;
    this.nextStep();
  }

  onReservationConfirmed() {
    // Envoyer les données de réservation au backend
    console.log('Réservation confirmée :', this.reservationDetails);
  }

  nextStep() {
    if (this.currentStep < 5) {
      this.currentStep++;
    }
  }

  previousStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

}
