import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ShowtimeDto } from '@app/core/models/showtime.models';

@Component({
  selector: 'app-showtime-selector',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './showtime-selector.component.html',
  styleUrl: './showtime-selector.component.scss'
})
export class ShowtimeSelectorComponent {
  @Input() showtimes: ShowtimeDto[] = [];
  @Output() showtimeSelected = new EventEmitter<ShowtimeDto>();

  onShowtimeSelected(showtime: ShowtimeDto): void {
    console.log('Séance sélectionnée :', showtime);
    this.showtimeSelected.emit(showtime); // Émet l'événement au parent
  }
}
