import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AuthService } from '@app/core/services/auth.service';
import { ReservationService } from '@app/core/services/reservation.service';
import { LoginPromptComponent } from '@app/pages/authModule/login-prompt/login-prompt.component';
import { CreateReservationDto } from '@app/core/models/reservation.models'; // Importez le modèle CreateReservationDto

@Component({
  selector: 'app-reservation-confirmation',
  standalone: true,
  imports: [CommonModule, LoginPromptComponent],
  templateUrl: './reservation-confirmation.component.html',
  styleUrl: './reservation-confirmation.component.scss'
})
export class ReservationConfirmationComponent {
  @Input() reservationDetails: any;
  isLoggedIn: boolean = false;
  @Output() confirmed = new EventEmitter<void>();

  constructor(
    private authService: AuthService,
    private reservationService: ReservationService
  ) {
    this.isLoggedIn = this.authService.isLoggedIn();
  }

  // Calcule le prix total en fonction des sièges sélectionnés et de la qualité
  calculateTotalPrice(): number {
    if (!this.reservationDetails || !this.reservationDetails.seats || !this.reservationDetails.session) {
      return 0;
    }

    const seatCount = this.reservationDetails.seats.length;
    const pricePerSeat = this.reservationDetails.session.price;
    return seatCount * pricePerSeat;
  }

  // Méthode pour formater les sièges
  getFormattedSeats(): string {
    if (!this.reservationDetails?.seats) return 'Aucun siège sélectionné';
    return this.reservationDetails.seats.map((seat: any) => seat.seatNumber).join(', ');
  }

  // Méthode pour confirmer la réservation
  confirmReservation() {
    if (this.isLoggedIn && this.reservationDetails) {
      // Récupérer l'utilisateur connecté
    const appUser = this.authService.getCurrentUser();

    // Vérifier si l'utilisateur est connecté et si appUserId est défini
    if (!appUser || !appUser.appUserId) {
      console.error('Aucun utilisateur connecté ou ID utilisateur manquant.');
      return;
    }

    // Récupérer l'ID de l'utilisateur
    const appUserId = appUser.appUserId;

    // Créer l'objet CreateReservationDto
    const createReservationDto: CreateReservationDto = {
      appUserId: appUserId,
      showtimeId: this.reservationDetails.session.showtimeId,
      seatNumbers: this.reservationDetails.seats.map((seat: any) => seat.seatNumber)
    };

    // Appeler la méthode createReservation du service
    this.reservationService.createReservation(createReservationDto).subscribe({
      next: (response) => {
        console.log('Réservation créée avec succès :', response.Message);
        this.reservationService.clearCurrentReservation();
          this.confirmed.emit(); 
      },
      error: (error) => {
        console.error('Erreur lors de la création de la réservation :', error);
        // Vous pouvez afficher un message d'erreur ici
      }
    });
    }
  }
}