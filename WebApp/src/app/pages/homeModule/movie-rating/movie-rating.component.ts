import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MovieRatingDto, MovieReviewDto } from '@app/core/models/movie-rating.models';

@Component({
  selector: 'app-movie-rating',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movie-rating.component.html',
  styleUrl: './movie-rating.component.scss'
})
export class MovieRatingComponent {
  @Input() reviews: MovieRatingDto[] = [];
}
