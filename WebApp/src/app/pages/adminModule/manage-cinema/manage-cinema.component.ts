import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CinemaDto, CreateCinemaDto, UpdateCinemaDto } from '@app/core/models/cinema.models';
import { TheaterDto } from '@app/core/models/theater.models';
import { AlertService } from '@app/core/services/alert.service';
import { CinemaService } from '@app/core/services/cinema.service';
import { LoadingService } from '@app/core/services/loading.service';
import { TheaterService } from '@app/core/services/theater.service';
import { ContainerComponent } from '@app/layourt/sharedComponents/container/container.component';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import * as bootstrap from 'bootstrap';
import { ManageTheaterComponent } from './manage-theater/manage-theater.component';
import { ManageSeatsComponent } from './manage-theater/manage-seats/manage-seats.component';

@Component({
  selector: 'app-manage-cinema',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ManageTheaterComponent, ManageSeatsComponent, ContainerComponent, NgbNavModule],
  templateUrl: './manage-cinema.component.html',
  styleUrl: './manage-cinema.component.scss'
})
export class ManageCinemaComponent implements OnInit {
  cinemas: CinemaDto[] = [];
  mode: 'create' | 'edit' = 'create';
  cinemaForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private cinemaService: CinemaService,
    private theaterService: TheaterService,
    private alertService: AlertService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    this.initCinemaForm(); // Initialiser le formulaire
    this.loadCinemas();
  }

  loadCinemas(): void {
    this.loadingService.show();
    this.cinemaService.getAllCinemas().subscribe(
      (data: CinemaDto[]) => {
        this.cinemas = data;

        // Ajouter le champ theaterCount pour chaque cinéma
        this.cinemas.forEach(cinema => {
          this.theaterService.getTheatersByCinema(cinema.cinemaId).subscribe(
            (theaters: any[]) => {
              cinema.theaterCount = theaters.length; // Calcul du nombre de salles
            }
          );
        });

        this.loadingService.hide();
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des cinémas.', 'info');
        this.loadingService.hide();
      }
    );
  }

  // Initialisation du formulaire avec FormBuilder
  initCinemaForm(cinema?: CinemaDto): void {
    if (cinema) {
      // Mode édition : pré-remplir avec les données existantes
      this.cinemaForm = this.fb.group({
        cinemaId: [cinema.cinemaId],
        name: [cinema.name, Validators.required],
        address: [cinema.address, Validators.required],
        phoneNumber: [cinema.phoneNumber, Validators.required],
        city: [cinema.city, Validators.required],
        country: [cinema.country, Validators.required],
        openingHours: [cinema.openingHours]
      });
    } else {
      // Mode création : initialiser avec des valeurs vides
      this.cinemaForm = this.fb.group({
        cinemaId: [null], // Non requis en mode création
        name: ['', Validators.required],
        address: ['', Validators.required],
        phoneNumber: ['', Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
        openingHours: ['']
      });
    }
  }

  // Méthode pour ouvrir l'offcanvas en mode création
  openCreateOffcanvas(): void {
    this.mode = 'create';
    this.initCinemaForm(); // Réinitialiser le formulaire
    this.showManageCinemaOffcanvas();
  }

  // Méthode pour ouvrir l'offcanvas en mode édition
  openEditOffcanvas(cinema: CinemaDto): void {
    this.mode = 'edit';
    this.initCinemaForm(cinema); // Pré-remplir le formulaire
    this.showManageCinemaOffcanvas();
  }

  // Méthode pour afficher l'offcanvas
  private showManageCinemaOffcanvas(): void {
    const manageCinemaElement = document.getElementById('manageCinemaCanvas');
    if (manageCinemaElement) {
      const manageCinemaInstance = bootstrap.Offcanvas.getInstance(manageCinemaElement);
      if (!manageCinemaInstance) {
        new bootstrap.Offcanvas(manageCinemaElement).show();
      } else {
        manageCinemaInstance.show();
      }
    }
  }

  // Soumettre le formulaire
  submitForm(): void {
    if (this.cinemaForm.invalid) {
      this.alertService.showAlert('Veuillez remplir tous les champs obligatoires.', 'warning');
      return;
    }

    this.loadingService.show(); // Afficher le spinner de chargement

    const formValue = this.cinemaForm.value;

    if (this.mode === 'create') {
      // Mode création : appeler la méthode createCinema
      this.cinemaService.createCinema(formValue as CreateCinemaDto).subscribe(
        (response) => this.handleSuccessResponse(response),
        (error) => this.handleError(error)
      );
    } else if (this.mode === 'edit') {
      // Mode édition : appeler la méthode updateCinema
      this.cinemaService.updateCinema(formValue as UpdateCinemaDto).subscribe(
        (response) => this.handleSuccessResponse(response),
        (error) => this.handleError(error)
      );
    }
  }

  // Gestion de la réponse réussie
  private handleSuccessResponse(response: { Message: string }): void {
    this.loadingService.hide(); // Cacher le spinner après réussite
    this.alertService.showAlert(response.Message, 'success');
    this.loadCinemas(); // Rafraîchir la liste des cinémas
    // Fermer l'offcanvas après soumission
    const manageCinemaElement = document.getElementById('manageCinemaCanvas');
    if (manageCinemaElement) {
      const manageCinemaInstance = bootstrap.Offcanvas.getInstance(manageCinemaElement);
      if (manageCinemaInstance) {
        manageCinemaInstance.hide();
      }
    }
    if (this.mode === 'create') {
      // Réinitialiser le formulaire en mode création
      this.initCinemaForm();
    }
  }

  // Gestion des erreurs
  private handleError(error: any): void {
    this.loadingService.hide(); // Cacher le spinner en cas d'erreur
    this.alertService.showAlert('Erreur lors de la soumission du formulaire.', 'danger', error);
  }

  // Supprimer un cinéma
  deleteCinema(cinemaId: number): void {
    if (confirm('Êtes-vous sûr de vouloir supprimer ce cinéma ?')) {
      this.loadingService.show();
      this.cinemaService.deleteCinema(cinemaId).subscribe(
        (data: { Message: string }) => {
          this.alertService.showAlert(data.Message, 'success');
          this.loadCinemas();
        },
        (error) => {
          this.alertService.showAlert('Erreur lors de la suppression du cinéma.', 'danger');
          this.loadingService.hide();
        }
      );
    }
  }
}