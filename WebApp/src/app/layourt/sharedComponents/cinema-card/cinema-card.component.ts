import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { CinemaDto } from '@app/core/models/cinema.models';

@Component({
  selector: 'app-cinema-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cinema-card.component.html',
  styleUrl: './cinema-card.component.scss'
})
export class CinemaCardComponent {
  @Input() cinema!: CinemaDto ;

}
