import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ShowtimeDto, CreateShowtimeDto, UpdateShowtimeDto } from '@app/core/models/showtime.models';
import { ShowtimeService } from '@app/core/services/showtime.service';
import { AlertService } from '@app/core/services/alert.service';
import { LoadingService } from '@app/core/services/loading.service';
import { CommonModule } from '@angular/common';
import * as bootstrap from 'bootstrap';
import { MovieService } from '@app/core/services/movie.service';
import { EnumService } from '@app/core/services/enum.service';
import { MovieDto } from '@app/core/models/movie.models';

@Component({
  selector: 'app-manage-showtime',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './manage-showtime.component.html',
  styleUrl: './manage-showtime.component.scss'
})
export class ManageShowtimeComponent implements OnInit {
  showtimes: ShowtimeDto[] = [];
  movies: MovieDto[] = [];
  mode: 'create' | 'edit' = 'create';
  showtimeForm!: FormGroup;
  movieId!: number; 
  showtimesByMovie: { [key: number]: ShowtimeDto[] } = {};
  projectionQualityOptions: { index: number; value: string }[] =
  EnumService.getEnumOptions(EnumService.ProjectionQuality);

  constructor(
    private fb: FormBuilder,
    private showtimeService: ShowtimeService,
    private movieService: MovieService,
    private alertService: AlertService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    this.initShowtimeForm(); // Initialiser le formulaire
    this.loadMoviesAndShowtimes();
  }

  // Charger les films et leurs séances
  loadMoviesAndShowtimes(): void {
    this.loadingService.show();
    this.movieService.getAllMovies().subscribe(
      (movies: MovieDto[]) => {
        this.movies = movies;

        // Pour chaque film, charger ses séances
        this.movies.forEach(movie => {
          this.movieService.getMovieSessions(movie.movieId).subscribe(
            (showtimes: ShowtimeDto[]) => {
              this.showtimesByMovie[movie.movieId] = showtimes;
            }
          );
        });

        this.loadingService.hide();
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des films et des séances.', 'info');
        this.loadingService.hide();
      }
    );
  }

  // Initialisation du formulaire avec FormBuilder
  initShowtimeForm(showtime?: ShowtimeDto): void {
    if (showtime) {
      // Mode édition : pré-remplir avec les données existantes
      this.showtimeForm = this.fb.group({
        showtimeId: [showtime.showtimeId],
        movieId: [showtime.movieId, Validators.required],
        theaterId: [showtime.theaterId, Validators.required],
        cinemaId: [showtime.cinemaId, Validators.required],
        startTime: [showtime.startTime, Validators.required],
        quality: [showtime.quality, Validators.required],
        endTime: [showtime.endTime, Validators.required],
        priceAdjustment: [showtime.priceAdjustment, Validators.required],
        isPromotion: [showtime.isPromotion]
      });
    } else {
      // Mode création : initialiser avec des valeurs vides
      this.showtimeForm = this.fb.group({
        showtimeId: [null], // Non requis en mode création
        movieId: [this.movieId, Validators.required],
        theaterId: ['', Validators.required],
        cinemaId: ['', Validators.required],
        startTime: ['', Validators.required],
        quality: ['', Validators.required],
        endTime: ['', Validators.required],
        priceAdjustment: ['', Validators.required],
        isPromotion: [false]
      });
    }
  }

  // Méthode pour ouvrir l'offcanvas en mode création
  openCreateShowtimeOffcanvas(showtime: number): void {
    this.mode = 'create';
    this.initShowtimeForm(); // Réinitialiser le formulaire
    this.showManageShowtimeOffcanvas();
  }

  // Méthode pour ouvrir l'offcanvas en mode édition
  openEditShowtimeOffcanvas(showtime: ShowtimeDto): void {
    this.mode = 'edit';
    this.initShowtimeForm(showtime); // Pré-remplir le formulaire
    this.showManageShowtimeOffcanvas();
  }

  // Méthode pour afficher l'offcanvas
  private showManageShowtimeOffcanvas(): void {
    const manageShowtimeElement = document.getElementById('manageShowtimeCanvas');
    if (manageShowtimeElement) {
      const manageShowtimeInstance = bootstrap.Offcanvas.getInstance(manageShowtimeElement);
      if (!manageShowtimeInstance) {
        new bootstrap.Offcanvas(manageShowtimeElement).show();
      } else {
        manageShowtimeInstance.show();
      }
    }
  }

  getFormattedProjectionQuality(projectionQuality: number | string): string {
    if (typeof projectionQuality === 'number') {
      const projectionQualityArray = Object.values(EnumService.ProjectionQuality);
      return projectionQualityArray[projectionQuality] ?? 'Inconnu';
    }
    return EnumService.ProjectionQuality[projectionQuality as keyof typeof EnumService.ProjectionQuality] ?? 'Inconnu';
  }

  // Soumettre le formulaire
  submitForm(): void {
    if (this.showtimeForm.invalid) {
      this.alertService.showAlert('Veuillez remplir tous les champs obligatoires.', 'warning');
      return;
    }

    this.loadingService.show();

    const formValue = this.showtimeForm.value;

    formValue.projectionQuality = Number(formValue.projectionQuality);

    // Définir l'action à exécuter en fonction du mode (create ou edit)
        const showtimeAction$ = this.mode === 'create'
          ? this.showtimeService.createShowtime(formValue as CreateShowtimeDto)
          : this.showtimeService.updateShowtime(formValue as UpdateShowtimeDto);

    // Souscrire à l'action sélectionnée
    showtimeAction$.subscribe(
      (response) => this.handleSuccessResponse(response), // Gestion de la réponse réussie
      (error) => this.handleError(error) // Gestion des erreurs
    );
  }

  // Gestion de la réponse réussie
  private handleSuccessResponse(response: { Message: string }): void {
    this.loadingService.hide(); // Cacher le spinner après réussite
    this.alertService.showAlert('Opération réussie!', 'success');
    this.loadMoviesAndShowtimes(); // Rafraîchir la liste des séances
    // Fermer l'offcanvas après soumission
    const manageShowtimeElement = document.getElementById('manageShowtimeCanvas');
    if (manageShowtimeElement) {
      const manageShowtimeInstance = bootstrap.Offcanvas.getInstance(manageShowtimeElement);
      if (manageShowtimeInstance) {
        manageShowtimeInstance.hide();
      }
    }
    if (this.mode === 'create') {
      // Réinitialiser le formulaire en mode création
      this.initShowtimeForm();
    }
  }

  // Gestion des erreurs
  private handleError(error: any): void {
    this.loadingService.hide(); // Cacher le spinner en cas d'erreur
    this.alertService.showAlert('Erreur lors de la soumission du formulaire.', 'danger', error);
  }

  // Supprimer une séance
  deleteShowtime(showtimeId: number): void {
    if (confirm('Êtes-vous sûr de vouloir supprimer cette séance ?')) {
      this.loadingService.show();
      this.showtimeService.deleteShowtime(showtimeId).subscribe(
        (data: { Message: string }) => {
          this.alertService.showAlert(data.Message, 'success');
          this.loadMoviesAndShowtimes();
        },
        (error) => {
          this.alertService.showAlert('Erreur lors de la suppression de la séance.', 'danger');
          this.loadingService.hide();
        }
      );
    }
  }
}