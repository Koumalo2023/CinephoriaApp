import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CinemaDto } from '@app/core/models/cinema.models';
import { ProjectionQuality } from '@app/core/models/enum.model';
import { CreateTheaterDto, TheaterDto, UpdateTheaterDto } from '@app/core/models/theater.models';
import { AlertService } from '@app/core/services/alert.service';
import { CinemaService } from '@app/core/services/cinema.service';
import { EnumService } from '@app/core/services/enum.service';
import { LoadingService } from '@app/core/services/loading.service';
import { TheaterService } from '@app/core/services/theater.service';
import * as bootstrap from 'bootstrap';

@Component({
  selector: 'app-manage-theater',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './manage-theater.component.html',
  styleUrl: './manage-theater.component.scss'
})
export class ManageTheaterComponent implements OnInit {
  cinemas: CinemaDto[] = [];
  theatersByCinema: { [key: number]: TheaterDto[] } = {};
  projectionQualityOptions: { index: number; value: string }[] =
  EnumService.getEnumOptions(EnumService.ProjectionQuality);
  mode: 'create' | 'edit' = 'create';

  theaterForm!: FormGroup;


  constructor(
    private fb: FormBuilder,
    private cinemaService: CinemaService,
    private theaterService: TheaterService,
    private alertService: AlertService,
    private loadingService: LoadingService,
    public enumService: EnumService
  ) { 
    this.theaterForm = this.fb.group({
      theaterId: [0],
      name: ['', Validators.required],
      seatCount: [0, [Validators.required, Validators.min(1)]],
      cinemaId: [0, Validators.required],
      projectionQuality: [ProjectionQuality.FourDX, Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadCinemasAndTheaters();
  }


  getFormattedProjectionQuality(projectionQuality: number | string): string {
      if (typeof projectionQuality === 'number') {
        const projectionQualityArray = Object.values(EnumService.ProjectionQuality);
        return projectionQualityArray[projectionQuality] ?? 'Inconnu';
      }
      return EnumService.ProjectionQuality[projectionQuality as keyof typeof EnumService.ProjectionQuality] ?? 'Inconnu';
    }

  // Méthode pour initialiser le formulaire
  resetTheaterForm(theater?: TheaterDto): void {
    if (theater) {
      this.theaterForm.patchValue({
        theaterId: theater.theaterId,
        name: theater.name,
        seatCount: theater.seatCount,
        cinemaId: theater.cinemaId,
        projectionQuality: theater.projectionQuality,
      });
    } else {
      this.theaterForm.reset({
        name: '',
        seatCount: 0,
        cinemaId: 0,
        projectionQuality: ProjectionQuality.FourDX,
      });
    }
  }
  

  loadCinemasAndTheaters(): void {
    this.cinemaService.getAllCinemas().subscribe(
      (cinemas: CinemaDto[]) => {
        this.cinemas = cinemas;
        this.loadingService.show();
        // Pour chaque cinéma, charger ses salles
        cinemas.forEach(cinema => {
          this.theaterService.getTheatersByCinema(cinema.cinemaId).subscribe(
            (theaters: TheaterDto[]) => {
              this.theatersByCinema[cinema.cinemaId] = theaters;
            }
          );
        });

        this.loadingService.hide();
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des cinémas .', 'info', error);

        this.loadingService.hide();
      }
    );
  }
  openCreateTheaterOffcanvas(cinemaId: number): void {
    this.theaterForm.reset({
      name: '',
      seatCount: 0,
      cinemaId: cinemaId,
      projectionQuality: ProjectionQuality.FourDX,
    });
    this.mode = 'create';
    this.showTheaterOffcanvas();
  }

  openEditTheaterOffcanvas(theater: TheaterDto): void {
    this.theaterForm.patchValue({
      theaterId: theater.theaterId,
      name: theater.name,
      seatCount: theater.seatCount,
      cinemaId: theater.cinemaId,
      projectionQuality: theater.projectionQuality,
    });
    this.mode = 'edit';
    this.showTheaterOffcanvas();
  }

  submitTheaterForm(): void {
    // Vérifier si le formulaire est invalide
    if (this.theaterForm.invalid) {
      this.alertService.showAlert('Veuillez corriger les erreurs dans le formulaire.', 'warning');
      return;
    }
  
    // Afficher le loader
    this.loadingService.show();
  
    // Récupérer les valeurs du formulaire
    const formValue = this.theaterForm.value;
  
    formValue.projectionQuality = Number(formValue.projectionQuality);
    
    // Définir l'action à exécuter en fonction du mode (create ou edit)
    const theaterAction$ = this.mode === 'create'
      ? this.theaterService.createTheater(formValue as CreateTheaterDto)
      : this.theaterService.updateTheater(formValue as UpdateTheaterDto);
  
    // Souscrire à l'action sélectionnée
    theaterAction$.subscribe(
      (response) => this.handleSuccessResponse(response), // Gestion de la réponse réussie
      (error) => this.handleError(error) // Gestion des erreurs
    );
  }

  private handleSuccessResponse(response: { Message: string }): void {
    this.loadingService.hide();
    this.alertService.showAlert("opération réalisée avec succès", 'success');
    this.loadCinemasAndTheaters();
  
    // Fermer l'offcanvas
    const theaterOffcanvasElement = document.getElementById('theaterOffcanvas');
    if (theaterOffcanvasElement) {
      const theaterOffcanvasInstance = bootstrap.Offcanvas.getInstance(theaterOffcanvasElement);
      if (theaterOffcanvasInstance) {
        theaterOffcanvasInstance.hide();
      }
    }
  
    // Réinitialiser le formulaire si c'est une création
    if (this.mode === 'create') {
      this.theaterForm.reset({
        name: '',
        seatCount: 0,
        cinemaId: 0,
        projectionQuality: ProjectionQuality.FourDX,
      });
    }
  }

  private showTheaterOffcanvas(): void {
    const offcanvasElement = document.getElementById('theaterOffcanvas'); // ID de l'offcanvas
  
    if (offcanvasElement) {
      // Vérifier si une instance existe déjà
      const offcanvasInstance = bootstrap.Offcanvas.getInstance(offcanvasElement);
  
      if (offcanvasInstance) {
        // Si l'instance existe, réutilisez-la
        offcanvasInstance.show();
      } else {
        // Sinon, créez une nouvelle instance et affichez l'offcanvas
        const newOffcanvas = new bootstrap.Offcanvas(offcanvasElement);
        newOffcanvas.show();
      }
    } else {
      console.error("L'élément offcanvas avec l'ID 'theaterOffcanvas' n'a pas été trouvé.");
    }
  }

  private handleError(error: any): void {
    this.loadingService.hide();
    const errorMessage = error?.error?.Message || 'Une erreur inconnue s\'est produite.';
    this.alertService.showAlert(errorMessage, 'danger');
    console.error('Erreur lors de la soumission du formulaire :', error);
  }


  deleteTheater(theaterId: number): void {
    if (confirm('Êtes-vous sûr de vouloir supprimer cette salle ?')) {
      this.loadingService.show();
      this.theaterService.deleteTheater(theaterId).subscribe(
        () => {
          this.alertService.showAlert('Salle supprimée avec succès.', 'success');
          this.loadCinemasAndTheaters();
          this.loadingService.hide();
        },
        (error) => {
          this.handleError(error);
        }
      );
    }
  }
}
