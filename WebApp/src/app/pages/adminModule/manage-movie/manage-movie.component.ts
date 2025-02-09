import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MovieDto, CreateMovieDto, UpdateMovieDto } from '@app/core/models/movie.models';
import { MovieService } from '@app/core/services/movie.service';
import { AlertService } from '@app/core/services/alert.service';
import { LoadingService } from '@app/core/services/loading.service';
import { CommonModule } from '@angular/common';
import * as bootstrap from 'bootstrap';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { ContainerComponent } from '@app/layourt/sharedComponents/container/container.component';
import { EnumService } from '@app/core/services/enum.service';
import { ManageShowtimeComponent } from './manage-showtime/manage-showtime.component';
import { ManageReviewsComponent } from './manage-reviews/manage-reviews.component';

@Component({
  selector: 'app-manage-movie',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ManageReviewsComponent, ManageShowtimeComponent, ContainerComponent, NgbNavModule],
  templateUrl: './manage-movie.component.html',
  styleUrl: './manage-movie.component.scss'
})
export class ManageMovieComponent implements OnInit {
  movies: MovieDto[] = [];
  mode: 'create' | 'edit' = 'create';
  movieGenres : { index: number; value: string }[] =
  EnumService.getEnumOptions(EnumService.MovieGenre);
  movieForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private movieService: MovieService,
    private alertService: AlertService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    this.initMovieForm();
    this.loadMovies();
  }

  loadMovies(): void {
    this.loadingService.show();
    this.movieService.getAllMovies().subscribe(
      (data: MovieDto[]) => {
        this.movies = data;
        this.loadingService.hide();
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des films.', 'info');
        this.loadingService.hide();
      }
    );
  }

  getFormattedGenre(genre: number | string): string {
    if (typeof genre === 'number') {
      const genresArray = Object.values(EnumService.MovieGenre);
      return genresArray[genre] ?? 'Inconnu';
    }
    return EnumService.MovieGenre[genre as keyof typeof EnumService.MovieGenre] ?? 'Inconnu';
  }

  // Initialisation du formulaire avec FormBuilder
  initMovieForm(movie?: MovieDto): void {
    if (movie) {
      // Mode édition : pré-remplir avec les données existantes
      this.movieForm = this.fb.group({
        movieId: [movie.movieId],
        title: [movie.title, Validators.required],
        description: [movie.description, Validators.required],
        genre: [movie.genre, Validators.required],
        duration: [movie.duration, Validators.required],
        director: [movie.director, Validators.required],
        releaseDate: [movie.releaseDate, Validators.required],
        minimumAge: [movie.minimumAge, Validators.required],
        isFavorite: [movie.isFavorite],
        posterUrls: [movie.posterUrls || []]
      });
    } else {
      // Mode création : initialiser avec des valeurs vides
      this.movieForm = this.fb.group({
        movieId: 0,
        title: ['', Validators.required],
        description: ['', Validators.required],
        genre: ['', Validators.required],
        duration: ['', Validators.required],
        director: ['', Validators.required],
        releaseDate: ['', Validators.required],
        minimumAge: ['', Validators.required],
        isFavorite: [false],
        posterUrls: [[]]
      });
    }
  }

  // Méthode pour ouvrir l'offcanvas en mode création
  openCreateOffcanvas(): void {
    this.mode = 'create';
    this.initMovieForm(); 
    this.showManageMovieOffcanvas();
  }

  // Méthode pour ouvrir l'offcanvas en mode édition
  openEditOffcanvas(movie: MovieDto): void {
    this.mode = 'edit';
    this.initMovieForm(movie);
    this.showManageMovieOffcanvas();
  }

  // Méthode pour afficher l'offcanvas
  private showManageMovieOffcanvas(): void {
    const manageMovieElement = document.getElementById('manageMovieCanvas');
    if (manageMovieElement) {
      const manageMovieInstance = bootstrap.Offcanvas.getInstance(manageMovieElement);
      if (!manageMovieInstance) {
        new bootstrap.Offcanvas(manageMovieElement).show();
      } else {
        manageMovieInstance.show();
      }
    }
  }

  // Soumettre le formulaire
  submitForm(): void {
    if (this.movieForm.invalid) {
      this.alertService.showAlert('Veuillez remplir tous les champs obligatoires.', 'warning');
      return;
    }

    this.loadingService.show();

    const formValue = this.movieForm.value;

    formValue.genre = Number(formValue.genre);

    if (formValue.releaseDate) {
      const date = new Date(formValue.releaseDate);
      formValue.releaseDate = date.toISOString();
    }

    // Définir l'action à exécuter en fonction du mode (create ou edit)
        const movieAction$ = this.mode === 'create'
          ? this.movieService.createMovie(formValue as CreateMovieDto)
          : this.movieService.updateMovie(formValue as UpdateMovieDto);
      
        // Souscrire à l'action sélectionnée
        movieAction$.subscribe(
          (response) => this.handleSuccessResponse(response), // Gestion de la réponse réussie
          (error) => this.handleError(error) // Gestion des erreurs
        );
  }

  // Gestion de la réponse réussie
  private handleSuccessResponse(response: { Message: string }): void {
    this.loadingService.hide(); 
    this.alertService.showAlert('Action réalisée avec succès', 'success');
    this.loadMovies(); // Rafraîchir la liste des films
    // Fermer l'offcanvas après soumission
    const manageMovieElement = document.getElementById('manageMovieCanvas');
    if (manageMovieElement) {
      const manageMovieInstance = bootstrap.Offcanvas.getInstance(manageMovieElement);
      if (manageMovieInstance) {
        manageMovieInstance.hide();
      }
    }
    if (this.mode === 'create') {
      // Réinitialiser le formulaire en mode création
      this.initMovieForm();
    }
  }

  // Gestion des erreurs
  private handleError(error: any): void {
    this.loadingService.hide(); // Cacher le spinner en cas d'erreur
    this.alertService.showAlert('Erreur lors de la soumission du formulaire.', 'danger', error);
  }

  // Supprimer un film
  deleteMovie(movieId: number): void {
    if (confirm('Êtes-vous sûr de vouloir supprimer ce film ?')) {
      this.loadingService.show();
      this.movieService.deleteMovie(movieId).subscribe(
        (data: { Message: string }) => {
          this.alertService.showAlert(data.Message, 'success');
          this.loadMovies();
        },
        (error) => {
          this.alertService.showAlert('Erreur lors de la suppression du film.', 'danger');
          this.loadingService.hide();
        }
      );  
    }
  }

  // Télécharger une affiche pour un film
  uploadPoster(movieId: number, file: File): void {
    this.loadingService.show();
    this.movieService.uploadMoviePoster(movieId, file).subscribe(
      (response) => {
        this.alertService.showAlert(response.Message, 'success');
        this.loadMovies();
        this.loadingService.hide();
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du téléchargement de l\'affiche.', 'danger');
        this.loadingService.hide();
      }
    );
  }

  // Supprimer une affiche d'un film
  deletePoster(movieId: number, posterUrl: string): void {
    if (confirm('Êtes-vous sûr de vouloir supprimer cette affiche ?')) {
      this.loadingService.show();
      this.movieService.deleteMoviePoster(movieId, posterUrl).subscribe(
        (response) => {
          this.alertService.showAlert(response.Message, 'success');
          this.loadMovies();
          this.loadingService.hide();
        },
        (error) => {
          this.alertService.showAlert('Erreur lors de la suppression de l\'affiche.', 'danger');
          this.loadingService.hide();
        }
      );
    }
  }

  // Méthode pour gérer le téléchargement d'une nouvelle affiche
onPosterUpload(event: Event): void {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files.length > 0) {
    const file = input.files[0]; 

    // Vérifier si le fichier est une image
    if (file.type.startsWith('image/')) {
      const movieId = this.movieForm.get('movieId')?.value; 
      if (movieId) {
        this.uploadPoster(movieId, file);
      } else {
        this.alertService.showAlert('Impossible de télécharger l\'affiche sans ID de film.', 'warning');
      }
    } else {
      this.alertService.showAlert('Veuillez sélectionner un fichier image valide.', 'warning');
    }
  }
}
}