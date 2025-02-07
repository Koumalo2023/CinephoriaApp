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
import { CinemaDto } from '@app/core/models/cinema.models';
import { TheaterDto } from '@app/core/models/theater.models';
import { CinemaService } from '@app/core/services/cinema.service';
import { TheaterService } from '@app/core/services/theater.service';

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
  cinemas: CinemaDto[] = [];
  theaters: TheaterDto[] = []; 
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
    private cinemaService: CinemaService,
    private theaterService: TheaterService, 
    private alertService: AlertService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    this.initShowtimeForm();
    this.loadShowtimes();
    this.loadMovies();
    this.loadCinemas();
    this.loadTheaters();
  }

  // Charger les films
  loadMovies(): void {
    this.movieService.getAllMovies().subscribe(
      (data: MovieDto[]) => {
        this.movies = data;
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des films.', 'danger');
      }
    );
  }

  // Charger les cinémas
  loadCinemas(): void {
    this.cinemaService.getAllCinemas().subscribe(
      (data: CinemaDto[]) => {
        this.cinemas = data;
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des cinémas.', 'danger');
      }
    );
  }

  // Charger les salles d'un cinéma spécifique à partir de l'identifiant du cinéma
loadTheaters(cinemaId?: number): void {
  if (cinemaId) {
    this.theaterService.getTheatersByCinema(cinemaId).subscribe(
      (data: TheaterDto[]) => {
        this.theaters = data;
      },
      (error) => {
        console.log('Erreur lors du chargement des salles pour ce cinéma.', 'danger');
      }
    );
  } else {
    console.log('Erreur lors du chargement des salles.', 'danger');
  }
}

loadShowtimes(): void {
  this.loadingService.show();
  this.showtimeService.getAllShowtimes().subscribe(
    (data: ShowtimeDto[]) => {
      this.showtimes = data.filter(showtime => !!showtime); 
     
      this.showtimesByMovie = {};
      this.showtimes.forEach(showtime => {
        if (!this.showtimesByMovie[showtime.movieId]) {
          this.showtimesByMovie[showtime.movieId] = [];
        }
        this.showtimesByMovie[showtime.movieId].push(showtime);
       
      });
      

      this.loadingService.hide();
    },
    (error) => {
      this.loadingService.hide();
    }
  );
}

getShowtimesForMovie(movieId: number): ShowtimeDto[] {
  return this.showtimesByMovie[movieId] || [];
}


// Méthode pour ouvrir l'offcanvas en mode édition
openEditShowtimeOffcanvas(showtime: ShowtimeDto): void {
  this.mode = 'edit';
  this.initShowtimeForm(showtime); 
  this.showManageShowtimeOffcanvas();
}

// Récupérer le nom de la salle en fonction de son ID
getTheaterName(theaterId: number): string {
  const theater = this.theaters.find(t => t.theaterId === theaterId);
  return theater ? theater.name : 'Inconnu';
}

// Récupérer le nom du cinéma en fonction de son ID
getCinemaName(cinemaId: number): string {
  const cinema = this.cinemas.find(c => c.cinemaId === cinemaId);
  return cinema ? cinema.name : 'Inconnu';
}

 // Initialisation du formulaire avec FormBuilder
// Initialisation du formulaire avec FormBuilder
initShowtimeForm(showtime?: CreateShowtimeDto | UpdateShowtimeDto): void {
  let qualityIndex: number | string = '';

  if (showtime) {
    // Si c'est une mise à jour (UpdateShowtimeDto), convertir quality en index numérique
    if ('showtimeId' in showtime) {
      qualityIndex = EnumService.getEnumOptions(EnumService.ProjectionQuality)
        .find(option => option.value === showtime.quality)?.index || '';
    } else {
      // Si c'est une création (CreateShowtimeDto), conserver la valeur brute ou vide
      qualityIndex = showtime.quality || '';
    }
  }

  this.showtimeForm = this.fb.group({
    showtimeId: [showtime?.showtimeId || 0],
    movieId: [showtime?.movieId || '', Validators.required],
    theaterId: [showtime?.theaterId || '', Validators.required],
    cinemaId: [showtime?.cinemaId || '', Validators.required],
    startTime: [showtime?.startTime || '', Validators.required],
    quality: [qualityIndex || '', Validators.required],
    endTime: [showtime?.endTime || '', Validators.required],
    priceAdjustment: [showtime?.priceAdjustment || '', Validators.required],
    isPromotion: [showtime?.isPromotion || false]
  });
}

  // Méthode pour ouvrir l'offcanvas en mode création avec l'ID du film passé en paramètre
  openCreateShowtimeOffcanvas(movie: MovieDto | null): void {
    if (!movie) {
      this.alertService.showAlert('Aucun film sélectionné.', 'warning');
      return;
    }
    this.mode = 'create';
    this.initShowtimeForm();
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

  // Méthode appelée lorsque le cinéma est sélectionné
onCinemaChange(cinemaId: number): void {
  if (cinemaId) {
    this.theaterService.getTheatersByCinema(cinemaId).subscribe(
      (data: TheaterDto[]) => {
        this.theaters = data;
        // Réinitialiser le champ "Salle" dans le formulaire
        this.showtimeForm.patchValue({ theaterId: '' });
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des salles pour ce cinéma.', 'danger');
      }
    );
  } else {
    this.theaters = []; // Si aucun cinéma n'est sélectionné, vider la liste des salles
    this.showtimeForm.patchValue({ theaterId: '' });
  }
}

// Méthode appelée lorsque la salle est sélectionnée
onTheaterChange(theaterId: number): void {
  if (theaterId) {
    const selectedTheater = this.theaters.find(theater => theater.theaterId === theaterId);
    if (selectedTheater && selectedTheater.projectionQuality) {
      // Convertir la qualité de projection en index numérique
      const qualityIndex = EnumService.getEnumOptions(EnumService.ProjectionQuality)
      // Mettre à jour le champ "Qualité de projection" dans le formulaire
      this.showtimeForm.patchValue({ quality: qualityIndex });
    }
  } else {
    // Si aucune salle n'est sélectionnée, réinitialiser la qualité de projection
    this.showtimeForm.patchValue({ quality: '' });
  }
}

  // Soumettre le formulaire
  submitForm(): void {
    if (this.showtimeForm.invalid) {
      this.alertService.showAlert('Veuillez remplir tous les champs obligatoires.', 'warning');
      return;
    }

    this.loadingService.show();

    const formValue = this.showtimeForm.value;

    formValue.quality = Number(formValue.quality);
    formValue.cinemaId = Number(formValue.cinemaId);
    formValue.movieId = Number(formValue.movieId);
    formValue.theaterId = Number(formValue.theaterId);

    if (formValue.startTime) {
      const date = new Date(formValue.startTime);
      formValue.startTime = date.toISOString();
    }

    if (formValue.endTime) {
      const date = new Date(formValue.endTime);
      formValue.endTime = date.toISOString();
    }

    
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
    this.loadShowtimes(); // Rafraîchir la liste des séances
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
          this.loadShowtimes();
        },
        (error) => {
          this.alertService.showAlert('Erreur lors de la suppression de la séance.', 'danger');
          this.loadingService.hide();
        }
      );
    }
  }
}