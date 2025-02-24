import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MovieDetailsDto } from '@app/core/models/movie.models';
import { MovieService } from '@app/core/services/movie.service';
import { LoadingService } from '@app/core/services/loading.service';
import { AlertService } from '@app/core/services/alert.service';
import { ShowtimeDto } from '@app/core/models/showtime.models';
import { AuthService } from '@app/core/services/auth.service';
import { ShowtimeSelectorComponent } from '../showtime-selector/showtime-selector.component';
import { MovieRatingComponent } from '../movie-rating/movie-rating.component';

@Component({
  selector: 'app-movie-details',
  standalone: true,
  imports: [CommonModule, ShowtimeSelectorComponent, MovieRatingComponent],
  templateUrl: './movie-details.component.html',
  styleUrl: './movie-details.component.scss'
})
export class MovieDetailsComponent implements OnInit {
  movieId: number | null = null;
  movie: MovieDetailsDto | null = null;
  isLoading: boolean = false; 
  errorMessage: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private movieService: MovieService,
    private loadingService: LoadingService,
    private alertService: AlertService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const paramMovieId = params.get('movieId');
      console.log('Param movieId:', paramMovieId);

      if (paramMovieId) {
        this.movieId = +paramMovieId;
        console.log('Converted movieId:', this.movieId);

        if (!isNaN(this.movieId)) {
          this.loadMovieDetails();
        } else {
          this.errorMessage = 'ID du film non valide.';
          this.alertService.showAlert(this.errorMessage, 'warning');
        }
      } else {
        console.error('Aucun movieId trouvé dans les paramètres de l\'URL.');
      }
    });
  }


  private loadMovieDetails(): void {
    this.isLoading = true;
    this.movieService.getMovieDetails(this.movieId!).subscribe(
      (data: MovieDetailsDto) => {
        this.movie = data;
        this.isLoading = false;
      },
      (error) => {
        this.isLoading = false;
        this.errorMessage = 'Erreur lors du chargement des détails du film.';
        console.error(error);
        this.alertService.showAlert(this.errorMessage, 'danger');
      }
    );
  }


  onShowtimeSelected(showtime: ShowtimeDto): void {
    this.router.navigate(['/home/reservation'], {
      queryParams: {
        showtimeId: showtime.showtimeId,
        movieId: this.movieId,
        cinemaId: showtime.cinemaId,
        step: 4
      }
    });

  }


  onGoBack(): void {
    this.router.navigate(['/home/movies']);
  }
}

