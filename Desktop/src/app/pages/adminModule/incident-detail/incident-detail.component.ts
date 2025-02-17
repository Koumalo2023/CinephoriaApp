import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { IncidentDto, IncidentStatus, IncidentStatusUpdateDto } from '@app/core/models/incident.models';
import { AlertService } from '@app/core/services/alert.service';
import { EnumService } from '@app/core/services/enum.service';
import { IncidentService } from '@app/core/services/incident.service';
import { LoadingService } from '@app/core/services/loading.service';
import { NgbModal, NgbNavModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import * as bootstrap from 'bootstrap';

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

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private incidentService: IncidentService,
    private fb: FormBuilder,
    private loadingService: LoadingService,
    private alertService: AlertService
  ) { }

  ngOnInit(): void {
    this.loadIncidentDetails();
    this.initStatusUpdateForm();
  }


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


  initStatusUpdateForm(): void {
    this.statusUpdateForm = this.fb.group({
      status: ['', Validators.required],
      resolvedAt: ['']
    });
  }

  openStatusUpdateOffcanvas(): void {
    const statusUpdateOffcanvasElement = document.getElementById('statusUpdateOffcanvas');
    if (statusUpdateOffcanvasElement) {
      const statusUpdateOffcanvasInstance = bootstrap.Offcanvas.getInstance(statusUpdateOffcanvasElement);
      if (!statusUpdateOffcanvasInstance) {
        new bootstrap.Offcanvas(statusUpdateOffcanvasElement).show();
      } else {
        statusUpdateOffcanvasInstance.show();
      }
    }
  }



  onStatusUpdateSubmit(): void {
    if (this.statusUpdateForm.invalid || !this.incident) {
      return;
    }

    let resolvedAt = this.statusUpdateForm.value.resolvedAt;
    if (resolvedAt) {
      resolvedAt = new Date(resolvedAt).toISOString();
    }


    const statusString: string = this.statusUpdateForm.value.status;

    const statusNumber = EnumService.getIncidentStatusNumber(statusString);

    if (statusNumber === null) {
      this.alertService.showAlert('Statut invalide', 'danger');
      return;
    }

    const incidentStatusUpdateDto: IncidentStatusUpdateDto = {
      incidentId: this.incident.incidentId,
      status: statusNumber as unknown as IncidentStatus,
      resolvedAt: resolvedAt
    };

    this.loadingService.show();
    this.incidentService.updateIncidentStatus(incidentStatusUpdateDto).subscribe(
      (response) => {
        this.loadingService.hide();
        this.alertService.showAlert("tatus mis à jour avec succès", 'success');
        this.loadIncidentDetails();
        this.closeStatusUpdateOffcanvas();
      },
      (error) => {
        this.loadingService.hide();
        this.alertService.showAlert('Erreur lors de la mise à jour du statut.', 'danger');
      }
    );
  }


  // Méthode pour obtenir le statut en chaîne de caractères
  getStatusString(status: IncidentStatus | undefined): string {
    if (status === undefined) {
      return 'Inconnu';
    }
    return EnumService.IncidentStatusReverseMapping[status] || 'Inconnu';
  }



  closeStatusUpdateOffcanvas(): void {
    const statusUpdateOffcanvasElement = document.getElementById('statusUpdateOffcanvas');
    if (statusUpdateOffcanvasElement) {
      const statusUpdateOffcanvasInstance = bootstrap.Offcanvas.getInstance(statusUpdateOffcanvasElement);
      if (statusUpdateOffcanvasInstance) {
        statusUpdateOffcanvasInstance.hide();
      }
    }
  }

  goBackToList(): void {
    this.router.navigate(['/admin/incident-list']);
  }
}