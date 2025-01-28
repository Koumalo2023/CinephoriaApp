import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-movie-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movie-card.component.html',
  styleUrl: './movie-card.component.scss'
})
export class MovieCardComponent {
  // Propriété d'entrée pour passer des données dynamiques
  @Input() movie: {
    image: string;       // URL de l'image du film
    title: string;       // Titre du film
    genres: string[];    // Liste des genres
    description: string; // Description du film
  } = {
    image: '',
    title: '',
    genres: [],
    description: ''
  };
}
