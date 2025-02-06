import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CinemaDto } from '@app/core/models/cinema.models';
import { Seat, SeatDto, UpdateSeatDto } from '@app/core/models/seat.models';
import { TheaterDto } from '@app/core/models/theater.models';
import { AlertService } from '@app/core/services/alert.service';
import { CinemaService } from '@app/core/services/cinema.service';
import { SeatsService } from '@app/core/services/seat.service';
import { TheaterService } from '@app/core/services/theater.service';

@Component({
  selector: 'app-manage-seats',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './manage-seats.component.html',
  styleUrl: './manage-seats.component.scss'
})
export class ManageSeatsComponent implements OnInit {
  cinemas: CinemaDto[] = []; // Liste des cinémas
  theaters: TheaterDto[] = []; // Salles associées au premier cinéma
  seats: Seat[] = []; // Sièges associés à la première salle
  selectedCinemaId: number | null = null; // ID du cinéma sélectionné
  selectedTheaterId: number | null = null; // ID de la salle sélectionnée
  seatForm!: FormGroup; // Formulaire pour mettre à jour un siège
  isEditMode = false; // Indique si nous sommes en mode édition
  seatId!: number; // ID du siège à mettre à jour

  constructor(
    private fb: FormBuilder,
    private cinemaService: CinemaService,
    private theaterService: TheaterService,
    private seatService: SeatsService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.initSeatForm(); // Initialiser le formulaire pour les sièges
    this.loadCinemas(); // Charger tous les cinémas
  }

  /**
   * Initialiser le formulaire pour les sièges.
   */
  initSeatForm(): void {
    this.seatForm = this.fb.group({
      seatNumber: ['', [Validators.required]],
      isAccessible: [false],
      isAvailable: [true]
    });
  }

  /**
   * Charger tous les cinémas.
   */
  loadCinemas(): void {
    this.cinemaService.getAllCinemas().subscribe(
      (cinemas: CinemaDto[]) => {
        this.cinemas = cinemas;

        if (this.cinemas.length > 0) {
          this.selectedCinemaId = this.cinemas[0].cinemaId; // Sélectionner automatiquement le premier cinéma
          this.loadTheatersByCinema(this.selectedCinemaId);
        }
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des cinémas.', 'danger');
        console.error('Erreur lors du chargement des cinémas :', error);
      }
    );
  }

  selectCinema(cinemaId: number): void {
    this.selectedCinemaId = cinemaId;
    this.loadTheatersByCinema(cinemaId);
  }

  /**
   * Charger les salles d'un cinéma donné.
   * @param cinemaId ID du cinéma
   */
  loadTheatersByCinema(cinemaId: number): void {
    this.theaterService.getTheatersByCinema(cinemaId).subscribe(
      (theaters: TheaterDto[]) => {
        this.theaters = theaters;

        if (this.theaters.length > 0) {
          this.selectedTheaterId = this.theaters[0].theaterId; // Sélectionner automatiquement la première salle
          this.loadSeatsByTheater(this.selectedTheaterId);
        }
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des salles.', 'danger');
        console.error('Erreur lors du chargement des salles :', error);
      }
    );
  }

  /**
   * Charger les sièges d'une salle donnée.
   * @param theaterId ID de la salle
   */
  loadSeatsByTheater(theaterId: number): void {
    this.seatService.getSeatsByTheaterId(theaterId).subscribe(
      (seats: Seat[]) => {
        this.seats = seats;
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des sièges.', 'danger');
        console.error('Erreur lors du chargement des sièges :', error);
      }
    );
  }

  /**
   * Charger les données d'un siège existant pour mise à jour.
   * @param seatId ID du siège
   */
  loadSeatForUpdate(seatId: number): void {
    this.isEditMode = true;
    this.seatId = seatId;

    const seat = this.seats.find(s => s.seatId === seatId);
    if (seat) {
      this.seatForm.patchValue({
        seatNumber: seat.seatNumber,
        isAccessible: seat.isAccessible,
        isAvailable: seat.isAvailable
      });
    }
  }

  /**
   * Mettre à jour un siège.
   */
  updateSeat(): void {
    if (this.seatForm.invalid) {
      this.alertService.showAlert('Veuillez corriger les erreurs dans le formulaire.', 'warning');
      return;
    }

    const updateSeatDto: UpdateSeatDto = {
      seatId: this.seatId,
      seatNumber: this.seatForm.get('seatNumber')?.value,
      isAccessible: this.seatForm.get('isAccessible')?.value,
      isAvailable: this.seatForm.get('isAvailable')?.value
    };

    this.seatService.updateSeat(updateSeatDto).subscribe(
      (response) => {
        this.alertService.showAlert('Siège mis à jour avec succès.', 'success');
        this.resetForm();
        this.loadSeatsByTheater(this.selectedTheaterId!); // Rafraîchir la liste des sièges
      },
      (error) => {
        this.alertService.showAlert('Une erreur est survenue lors de la mise à jour du siège.', 'danger');
        console.error('Erreur lors de la mise à jour du siège :', error);
      }
    );
  }

  /**
   * Réinitialiser le formulaire.
   */
  resetForm(): void {
    this.isEditMode = false;
    this.seatId = 0;
    this.seatForm.reset();
  }
}
