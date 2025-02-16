import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { IncidentDto, IncidentStatusUpdateDto } from '@app/core/models/incident.models';
import { AlertService } from '@app/core/services/alert.service';
import { IncidentService } from '@app/core/services/incident.service';
import { LoadingService } from '@app/core/services/loading.service';
import { ContainerComponent } from '@app/layourt/sharedComponents/container/container.component';
import { NgbModal, NgbNavModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-incident-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgbNavModule],
  templateUrl: './incident-detail.component.html',
  styleUrl: './incident-detail.component.scss'
})
export class IncidentDetailComponent implements OnInit {
  incident: IncidentDto | null = null; 
  statusUpdateForm!: FormGroup; 
  
  selectedImage: string | null = null;
  @ViewChild('imageModal') imageModal!: TemplateRef<any>; 
  @ViewChild('statusUpdateOffcanvas') statusUpdateOffcanvas!: TemplateRef<any>;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private incidentService: IncidentService,
    private fb: FormBuilder,
    private modalService: NgbModal,
    private offcanvasService: NgbOffcanvas,
    private loadingService: LoadingService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.loadIncidentDetails();
    this.initStatusUpdateForm();
  }

  // Charge les détails de l'incident
  loadIncidentDetails(): void {
    const incidentId = this.route.snapshot.paramMap.get('id');
    if (incidentId) {
      this.loadingService.show();
      this.incidentService.getIncidentDetails(+incidentId).subscribe(
        (data: IncidentDto) => {
          this.incident = data;
          this.loadingService.hide();
        },
        (error) => {
          this.loadingService.hide();
          this.alertService.showAlert('Erreur lors du chargement des détails de l\'incident.', 'danger');
        }
      );
    }
  }

  // Initialise le formulaire de mise à jour du statut
  initStatusUpdateForm(): void {
    this.statusUpdateForm = this.fb.group({
      status: ['', Validators.required],
      resolvedAt: ['']
    });
  }

  // Ouvre l'offcanvas pour mettre à jour le statut
  openStatusUpdateOffcanvas(): void {
    this.offcanvasService.open(this.statusUpdateOffcanvas, { position: 'end' });
  }

  // Soumet le formulaire de mise à jour du statut
  onStatusUpdateSubmit(): void {
    if (this.statusUpdateForm.invalid || !this.incident) {
      return;
    }

    const incidentStatusUpdateDto: IncidentStatusUpdateDto = {
      incidentId: this.incident.incidentId,
      status: this.statusUpdateForm.value.status,
      resolvedAt: this.statusUpdateForm.value.resolvedAt
    };

    this.loadingService.show();
    this.incidentService.updateIncidentStatus(incidentStatusUpdateDto).subscribe(
      (response) => {
        this.loadingService.hide();
        this.alertService.showAlert(response.Message, 'success');
        this.loadIncidentDetails(); // Recharger les détails de l'incident
        this.offcanvasService.dismiss('statusUpdateOffcanvas'); // Fermer l'offcanvas
      },
      (error) => {
        this.loadingService.hide();
        this.alertService.showAlert('Erreur lors de la mise à jour du statut.', 'danger');
      }
    );
  }
 // Ouvre une modal pour agrandir une image
 openImageModal(imageUrl: string): void {
  this.selectedImage = imageUrl;
  this.modalService.open(this.imageModal, { size: 'lg', centered: true });
}
  
  goBackToList(): void {
    this.router.navigate(['/admin/incident-list']);
  }
}
