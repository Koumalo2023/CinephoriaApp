import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-price-summary',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './price-summary.component.html',
  styleUrl: './price-summary.component.scss'
})
export class PriceSummaryComponent {
  @Input() reservationDetails: any; // Reçoit les détails de la réservation depuis le parent

  constructor() {}

  // Calcule le prix total en fonction des sièges sélectionnés et de la qualité
  calculateTotalPrice(): number {
    if (!this.reservationDetails || !this.reservationDetails.seats || !this.reservationDetails.session) {
      return 0;
    }

    const seatCount = this.reservationDetails.seats.length;
    const pricePerSeat = this.reservationDetails.session.price; // Supposons que le prix par siège est défini dans la séance
    return seatCount * pricePerSeat;
  }

}
