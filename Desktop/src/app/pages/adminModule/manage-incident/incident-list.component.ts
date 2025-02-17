import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CinemaDto } from '@app/core/models/cinema.models';
import { CreateIncidentDto, IncidentDto, IncidentStatus, UpdateIncidentDto } from '@app/core/models/incident.models';
import { TheaterDto } from '@app/core/models/theater.models';
import { AlertService } from '@app/core/services/alert.service';
import { AuthService } from '@app/core/services/auth.service';
import { CinemaService } from '@app/core/services/cinema.service';
import { EnumService } from '@app/core/services/enum.service';
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
    
    this.initIncidentForm();
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
    this.selectedTheaterId = null; 
    this.loadTheatersByCinema(cinemaId); 
    this.filterIncidents(); 
  }

  // Méthode appelée lors de la sélection d'une salle
  onTheaterSelect(event: any): void {
    const theaterId = +event.target.value;
    this.selectedTheaterId = theaterId;
    this.filterIncidents(); 
  }
  // Filtre les incidents en fonction du cinéma ou de la salle sélectionnée
  filterIncidents(): void {
    if (this.selectedTheaterId) {
      
      this.filteredIncidents = this.allIncidents.filter(
        (incident) => incident.theaterId === this.selectedTheaterId
      );
    } else if (this.selectedCinemaId) {
      
      this.filteredIncidents = this.allIncidents.filter(
        (incident) => incident.theaterId === this.selectedCinemaId
      );
    } else {
      
      this.filteredIncidents = [];
    }
  }
  
  onCinemaChange(event: any): void {
    const cinemaId = event.target.value;
  
    if (cinemaId) {
      this.theaterService.getTheatersByCinema(cinemaId).subscribe(
        (data: TheaterDto[]) => {
          this.theaters = data;
          this.incidentForm.patchValue({ theaterId: null, theaterName: '' }); 
        }
      );
    } else {
      this.theaters = [];
      this.incidentForm.patchValue({ theaterId: null, theaterName: '' });
    }
  }
  
  
  initIncidentForm(incident?: IncidentDto): void {
    const today = new Date().toISOString().slice(0, 16); 
    const reportedBy = this.getCurrentUserName(); 
  
    if (this.mode === 'edit' && incident) {
      this.incidentForm = this.fb.group({
        incidentId: [incident.incidentId, Validators.required],
        theaterId: [incident.theaterId ?? null, Validators.required],
        description: [incident.description, Validators.required],
        status: [incident.status, Validators.required],
        resolvedAt: [incident.resolvedAt],
        imageUrls: [incident.imageUrls || []], 
      });
    } else {
      this.incidentForm = this.fb.group({
        theaterId: ['', Validators.required], 
        description: ['', Validators.required], 
        reportedBy: [reportedBy], 
        imageUrls: [[]], 
      });
    }
  }
  // Méthode pour tronquer la description
truncateDescription(description: string, maxLength: number = 50): string {
  if (description.length > maxLength) {
    return description.substring(0, maxLength) + '...';
  }
  return description;
}

// Méthode pour obtenir le statut en chaîne de caractères
getStatusString(status: number): string {
  return EnumService.IncidentStatusReverseMapping[status] || 'Inconnu';
}
  
  

  viewIncidentDetails(incidentId: number): void {
    this.loadIncidentImages(incidentId); 
    this.router.navigate(['/admin/incident-detail', incidentId]); 
  }

  // Méthode pour récupérer les images d'un incident
loadIncidentImages(incidentId: number): void {
  this.incidentService.getIncidentImages(incidentId).subscribe(
    (imageUrls: string[]) => {
      const incident = this.incidents.find(inc => inc.incidentId === incidentId);
      if (incident) {
        incident.imageUrls = imageUrls; 
      }
    },
    (error) => {
      console.error('Erreur lors du chargement des images', error);
    }
  );
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

  onSubmit(): void {
    if (this.incidentForm.invalid) {
      return;
    }
  
    this.loadingService.show();
  
    const formValue = this.incidentForm.value;
 
    if (formValue.resolvedAt) {
      formValue.resolvedAt = new Date(formValue.resolvedAt).toISOString();
    }
  

    const statusNumber = EnumService.getIncidentStatusNumber(formValue.status);
    if (statusNumber === null) {
      this.alertService.showAlert('Statut invalide', 'danger');
      return;
    }
    formValue.status = statusNumber; 
  

    if (formValue.theaterId !== null && formValue.theaterId !== undefined && formValue.theaterId !== '') {
      formValue.theaterId = Number(formValue.theaterId);
    } else {
      formValue.theaterId = 0; 
    }
  
    if (this.mode === 'create') {
      this.incidentService.reportIncident(formValue as CreateIncidentDto).subscribe(
        (response) => {
          this.incidentService.getAllIncidents().subscribe(
            (incidents: IncidentDto[]) => {
              const lastIncident = incidents[incidents.length - 1]; 
              const incidentId = lastIncident.incidentId; 
              this.uploadImages(incidentId); 
              this.handleSuccessResponse(response);
            },
            (error) => this.handleError(error)
          );
        },
        (error) => this.handleError(error)
      );
    } else if (this.mode === 'edit') {
      this.incidentService.updateIncident(formValue as UpdateIncidentDto).subscribe(
        (response) => {
          const incidentId = formValue.incidentId; 
          this.uploadImages(incidentId); 
          this.handleSuccessResponse(response);
        },
        (error) => this.handleError(error)
      );
    }
  }
  

  private uploadImages(incidentId: number): void {
  if (this.selectedFiles.length > 0) {
    this.selectedFiles.forEach(file => {
      this.incidentService.uploadIncidentImage(incidentId, file).subscribe(
        (response) => {
          console.log('Image uploadée avec succès:', response.Url);
        },
        (error) => {
          console.error('Erreur lors de l\'upload de l\'image', error);
        }
      );
    });
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
