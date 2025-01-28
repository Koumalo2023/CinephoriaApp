import { Component } from '@angular/core';
import { MovieCardComponent } from '@app/layourt/sharedComponents/movie-card/movie-card.component';

@Component({
  selector: 'app-home-movie',
  standalone: true,
  imports: [MovieCardComponent],
  templateUrl: './home-movie.component.html',
  styleUrl: './home-movie.component.scss'
})
export class HomeMovieComponent {

}
