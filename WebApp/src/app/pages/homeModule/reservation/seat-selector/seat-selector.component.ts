import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { ReservationService } from '@app/core/services/reservation.service';
import { SeatDto } from '@app/core/models/seat.models';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-seat-selector',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './seat-selector.component.html',
  styleUrl: './seat-selector.component.scss'
})
export class SeatSelectorComponent implements OnChanges {
  @Input() showtimeId: number | null = null;
  @Input() selectedSeats: SeatDto[] = [];
  @Output() seatsSelected = new EventEmitter<any[]>();

  availableSeats: SeatDto[] = [];
  localSelectedSeats: SeatDto[] = [];

  constructor(private reservationService: ReservationService) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['showtimeId'] && this.showtimeId) {
      this.loadAvailableSeats();
    }
    if (changes['selectedSeats'] && this.selectedSeats) {
      this.localSelectedSeats = [...this.selectedSeats]; 
    }
  }

  loadAvailableSeats() {
    if (this.showtimeId) {
      this.reservationService.getAvailableSeats(this.showtimeId).subscribe({
        next: (seats) => {
          this.availableSeats = seats;
        },
        error: (err) => {
          console.error('Erreur lors du chargement des sièges disponibles :', err);
        }
      });
    }
  }

  selectSeat(seat: SeatDto) {
    if (!this.localSelectedSeats.includes(seat)) {
      this.localSelectedSeats.push(seat);
    } else {
      this.localSelectedSeats = this.localSelectedSeats.filter(s => s !== seat);
    }
  }

  confirmSelection() {
    if (this.localSelectedSeats.length > 0) {
      this.seatsSelected.emit(this.localSelectedSeats);
    } else {
      alert('Veuillez sélectionner au moins un siège.');
    }
  }
  
}