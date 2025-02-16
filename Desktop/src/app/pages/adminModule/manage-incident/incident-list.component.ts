import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CinemaDto } from '@app/core/models/cinema.models';
import { CreateIncidentDto, IncidentDto, UpdateIncidentDto } from '@app/core/models/incident.models';
import { TheaterDto } from '@app/core/models/theater.models';
import { AlertService } from '@app/core/services/alert.service';
import { AuthService } from '@app/core/services/auth.service';
import { CinemaService } from '@app/core/services/cinema.service';
import { IncidentService } from '@app/core/services/incident.service';
import { LoadingService } from '@app/core/services/loading.service';
import { TheaterService } from '@app/core/services/theater.service';
import { ContainerComponent } from '@app/layourt/sharedComponents/container/container.component';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import * as bootstrap from 'bootstrap';

@Component({
  selector: 'app-incident-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgbNavModule, ContainerComponent],
  templateUrl: './incident-list.component.html',
  styleUrl: './incident-list.component.scss'
})
export class IncidentListComponent implements OnInit{
  incidents: IncidentDto[] = [];
  cinemas: CinemaDto[] = [];
  cinemaIncidents: IncidentDto[] = []; 
  filteredIncidents: IncidentDto[] = [];
  allIncidents: IncidentDto[] = []; 
  selectedCinemaId: number | null = null;
  selectedTheaterId: number | null = null;
  theaters: TheaterDto[] = [];
  selectedFiles: File[] = [];
  mode: 'create' | 'edit' = 'create';
  incidentForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private incidentService: IncidentService,
    private theaterService: TheaterService,
    private cinemaService: CinemaService,
    private authService: AuthService,
    private alertService: AlertService,
    private loadingService: LoadingService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // this.incidentForm.statusChanges.subscribe(status => {
    //   console.log('Statut du formulaire :', status);
    // });
    this.initIncidentForm(); // Initialiser le formulaire
    this.loadIncidents();
    this.loadCinemas();

  }

  // Méthode pour charger la liste des incidents
  loadIncidents(): void {
    this.loadingService.show();
    this.incidentService.getAllIncidents().subscribe(
      (data: IncidentDto[]) => {
        this.incidents = data;
        this.loadingService.hide();
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des incidents.', 'info');
        this.loadingService.hide();
      }
    );
  }

  getCurrentUserName(): string {
    const user = this.authService.getCurrentUser(); 
    return user ? user.appUserId : 'Utilisateur inconnu';
  }

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
  // Charge la liste des salles pour un cinéma donné
  loadTheatersByCinema(cinemaId: number): void {
    this.loadingService.show();
    this.theaterService.getTheatersByCinema(cinemaId).subscribe(
      (data: TheaterDto[]) => {
        this.theaters = data;
        this.loadingService.hide();
      },
      (error) => {
        this.loadingService.hide();
        this.alertService.showAlert('Erreur lors du chargement des salles.', 'danger');
      }
    );
  }
 
  // Charge les incidents d'un cinéma spécifique
  loadIncidentsByCinema(cinemaId: number): void {
    this.loadingService.show();
    this.incidentService.getIncidentsByCinema(cinemaId).subscribe(
      (data: IncidentDto[]) => {
        this.cinemaIncidents = data;
        this.loadingService.hide();
      },
      (error) => {
        this.loadingService.hide();
        this.alertService.showAlert('Erreur lors du chargement des incidents du cinéma.', 'danger');
      }
    );
  }
   // Méthode appelée lors de la sélection d'un cinéma
   onCinemaSelect(event: any): void {
    const cinemaId = +event.target.value;
    this.selectedCinemaId = cinemaId;
    this.selectedTheaterId = null; // Réinitialiser la sélection de la salle
    this.loadTheatersByCinema(cinemaId); // Charger les salles du cinéma sélectionné
    this.filterIncidents(); // Filtrer les incidents
  }

  // Méthode appelée lors de la sélection d'une salle
  onTheaterSelect(event: any): void {
    const theaterId = +event.target.value;
    this.selectedTheaterId = theaterId;
    this.filterIncidents(); // Filtrer les incidents
  }
  // Filtre les incidents en fonction du cinéma ou de la salle sélectionnée
  filterIncidents(): void {
    if (this.selectedTheaterId) {
      // Filtrer par salle
      this.filteredIncidents = this.allIncidents.filter(
        (incident) => incident.theaterId === this.selectedTheaterId
      );
    } else if (this.selectedCinemaId) {
      // Filtrer par cinéma
      this.filteredIncidents = this.allIncidents.filter(
        (incident) => incident.theaterId === this.selectedCinemaId
      );
    } else {
      // Aucun filtre
      this.filteredIncidents = [];
    }
  }
  
  onCinemaChange(event: any): void {
    const cinemaId = event.target.value;
    
    if (cinemaId) {
      this.theaterService.getTheatersByCinema(cinemaId).subscribe(
        (data: TheaterDto[]) => {
          this.theaters = data;
          this.incidentForm.patchValue({ theaterId: '', theaterName: '' }); 
        }
      );
    } else {
      this.theaters = [];
      this.incidentForm.patchValue({ theaterId: '', theaterName: '' });
    }
  }
  
  
  initIncidentForm(incident?: IncidentDto): void {
    const today = new Date().toISOString().slice(0, 16); 
    const reportedBy = this.getCurrentUserName(); 
  
    if (this.mode === 'edit' && incident) {
      // Mode édition : utilisez UpdateIncidentDto
      this.incidentForm = this.fb.group({
        incidentId: [incident.incidentId, Validators.required],
        status: [incident.status, Validators.required],
        resolvedAt: [incident.resolvedAt],
        imageUrls: [incident.imageUrls || []], 
      });
    } else {
      // Mode création : utilisez CreateIncidentDto
      this.incidentForm = this.fb.group({
        theaterId: ['', Validators.required], 
        description: ['', Validators.required], 
        reportedBy: [reportedBy], 
        imageUrls: [[]], 
      });
    }
  }

  viewIncidentDetails(incidentId: number): void {
    this.router.navigate(['/admin/incident-detail', incidentId]); // Rediriger vers la page de détails
  }

  // Méthode pour ouvrir l'offcanvas en mode création
  openCreateOffcanvas(): void {
    this.mode = 'create';
    this.initIncidentForm(); // Réinitialiser le formulaire
    this.showManageIncidentOffcanvas();
  }

  // Méthode pour ouvrir l'offcanvas en mode édition
  openEditOffcanvas(incident: IncidentDto): void {
    this.mode = 'edit';
    this.initIncidentForm(incident); // Pré-remplir le formulaire
    this.showManageIncidentOffcanvas();
  }

  // Méthode pour afficher l'offcanvas
  private showManageIncidentOffcanvas(): void {
    const manageIncidentElement = document.getElementById('manageIncidentCanvas');
    if (manageIncidentElement) {
      const manageIncidentInstance = bootstrap.Offcanvas.getInstance(manageIncidentElement);
      if (!manageIncidentInstance) {
        new bootstrap.Offcanvas(manageIncidentElement).show();
      } else {
        manageIncidentInstance.show();
      }
    }
  }

  // Méthode pour soumettre le formulaire
  onSubmit(): void {
    if (this.incidentForm.invalid) {
      return;
    }

    this.loadingService.show();

    const formValue = this.incidentForm.value;


    if (formValue.resolvedAt) {
      const date = new Date(formValue.resolvedAt);
      formValue.resolvedAt = date.toISOString();
    }
    if (this.mode === 'create') {
      // Mode création : appeler la méthode createIncident
      this.incidentService.reportIncident(formValue as CreateIncidentDto).subscribe(
        (response) => this.handleSuccessResponse(response),
        (error) => this.handleError(error)
      );
    } else if (this.mode === 'edit') {
      // Mode édition : appeler la méthode updateIncident
      this.incidentService.updateIncident(formValue as UpdateIncidentDto).subscribe(
        (response) => this.handleSuccessResponse(response),
        (error) => this.handleError(error)
      );
    }
  }
// Gestion de la réponse réussie
private handleSuccessResponse(response: { Message: string }): void {
  this.loadingService.hide(); // Cacher le spinner après réussite
  this.alertService.showAlert(response.Message, 'success');
  this.loadIncidents(); // Rafraîchir la liste des cinémas
  // Fermer l'offcanvas après soumission
  const manageIncidentElement = document.getElementById('manageIncidentCanvas');
  if (manageIncidentElement) {
    const manageIncidentInstance = bootstrap.Offcanvas.getInstance(manageIncidentElement);
    if (manageIncidentInstance) {
      manageIncidentInstance.hide();
    }
  }
  if (this.mode === 'create') {
    // Réinitialiser le formulaire en mode création
    this.initIncidentForm();
  }
}

// Gestion des erreurs
private handleError(error: any): void {
  this.loadingService.hide(); // Cacher le spinner en cas d'erreur
  this.alertService.showAlert('Erreur lors de la soumission du formulaire.', 'danger', error);
}

onFileChange(event: any): void {
  const files = event.target.files;
  if (files && files.length > 0) {
    this.selectedFiles = Array.from(files); // Convertir FileList en tableau
  }
}

// Supprimer un cinéma
deleteIncident(IncidentId: number): void {
  if (confirm('Êtes-vous sûr de vouloir supprimer ce cinéma ?')) {
    this.loadingService.show();
    this.incidentService.deleteIncident(IncidentId).subscribe(
      (data: { Message: string }) => {
        this.alertService.showAlert(data.Message, 'success');
        this.loadIncidents();
      },
      (error) => {
        this.alertService.showAlert('Erreur lors de la suppression du cinéma.', 'danger');
        this.loadingService.hide();
      }
    );
  }
}

}
