import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-seat-selector',
  standalone: true,
  imports: [],
  templateUrl: './seat-selector.component.html',
  styleUrl: './seat-selector.component.scss'
})
export class SeatSelectorComponent {
  @Output() seatsSelected = new EventEmitter<any[]>(); 

  selectedSeats: any[] = [];

  // Méthode pour sélectionner un siège
  selectSeat(seat: any) {
    this.selectedSeats.push(seat);
    this.seatsSelected.emit(this.selectedSeats);
  }
}
