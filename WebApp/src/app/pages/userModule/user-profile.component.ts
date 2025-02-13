import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateMovieRatingDto } from '@app/core/models/movie-rating.models';
import { UserReservationDto } from '@app/core/models/reservation.models';
import { UpdateAppUserDto, UserDto } from '@app/core/models/user.models';
import { DefaultImagePipe } from '@app/core/pipes/default-image.pipe';
import { AlertService } from '@app/core/services/alert.service';
import { AuthService } from '@app/core/services/auth.service';
import { LoadingService } from '@app/core/services/loading.service';
import { MovieRatingService } from '@app/core/services/movie-rating.service';
import { ReservationService } from '@app/core/services/reservation.service';
import { ContainerComponent } from '@app/layourt/sharedComponents/container/container.component';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import * as bootstrap from 'bootstrap';


@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, ContainerComponent, NgbNavModule, DefaultImagePipe],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.scss'
})
export class UserProfileComponent implements OnInit {
  userProfileForm!: FormGroup;
  passwordForm!: FormGroup;
  userProfile!: UserDto;
  userReservations: UserReservationDto[] = [];
  selectedFile: File | null = null;
  imageMaxHeight: string = '200px';
  createMovieRatingDto: CreateMovieRatingDto = {
    movieId: 0,
    appUserId: '',
    rating: 0,
    comment: ''
  };

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private reservationService: ReservationService,
    private movieRatingService: MovieRatingService,
    private alertService: AlertService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    this.initializeForms();
    this.loadCurrentUser();
      
  }

  /**
   * Initialise les formulaires pour le profil utilisateur et le mot de passe.
   */
  private initializeForms(): void {
    this.userProfileForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      userName: ['', Validators.required], // Modifie username afin qu'il prenne email par défaut
      phoneNumber: [''],
      profilePictureUrl: ['']
    });

    this.passwordForm = this.fb.group(
      {
        oldPassword: ['', Validators.required],
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', Validators.required],
      },
      { validators: this.passwordsMustMatch }
    );
  }

  
  private loadCurrentUser(): void {
    const currentUser = this.authService.getCurrentUser();
    console.log(currentUser);
    if (currentUser) {
      this.loadUserProfile(currentUser.appUserId);
      
      this.loadUserReservations(currentUser.appUserId);
    } else {
      this.alertService.showAlert('Aucun utilisateur connecté.', 'warning');
    }
  }

 
  private loadUserProfile(userId: string): void {
    this.loadingService.show(); // Afficher le spinner de chargement
    this.authService.getUserProfile(userId).subscribe(
      (profile: UserDto) => {
        this.userProfile = profile;
        this.userProfileForm.patchValue(profile);
        this.loadingService.hide(); // Cacher le spinner après chargement
      },
      (error) => {
        this.loadingService.hide(); // Cacher le spinner en cas d'erreur
        this.alertService.showAlert('Erreur lors du chargement du profil utilisateur.',"danger", error);
      }
    );
  }

  // getMainPosterUrl(): string {
  //   if (!this.userProfile || !this.userProfile.profilePictureUrl) {
  //     console.log('No valid profile picture URL found, returning default image.');
  //     return 'https://upload.wikimedia.org/wikipedia/commons/6/65/No-Image-Placeholder.svg';
  //   }
  
  //   const mainUrl = this.userProfile.profilePictureUrl;
  //   try {
  //     // Vérifie si l'URL est valide
  //     new URL(mainUrl);
  //     return mainUrl;
  //   } catch (error) {
  //     console.warn('Invalid profile picture URL:', mainUrl);
  //     return 'https://upload.wikimedia.org/wikipedia/commons/6/65/No-Image-Placeholder.svg';
  //   }
  // }

  // onImageLoadError(event: Event): void {
  //   const imgElement = event.target as HTMLImageElement;
  //   imgElement.src = 'https://upload.wikimedia.org/wikipedia/commons/6/65/No-Image-Placeholder.svg';
  // }

  private loadUserReservations(userId: string): void {
    this.loadingService.show(); // Afficher le spinner de chargement
    this.reservationService.getUserReservations(userId).subscribe(
      (reservations: UserReservationDto[]) => {
        this.userReservations = reservations;
        this.loadingService.hide(); // Cacher le spinner après chargement
      },
      (error) => {
        this.loadingService.hide(); // Cacher le spinner en cas d'erreur
        this.alertService.showAlert('Erreur lors du chargement des réservations.', "danger", error);
      }
    );
  }

  /**
   * Annule une réservation.
   * @param reservationId L'ID de la réservation à annuler.
   */
  cancelReservation(reservationId: number): void {
    if (!confirm('Êtes-vous sûr de vouloir annuler cette réservation ?')) return;

    this.loadingService.show(); // Afficher le spinner de chargement
    this.reservationService.cancelReservation(reservationId).subscribe(
      (response) => {
        this.loadingService.hide(); // Cacher le spinner après réussite
        this.alertService.showAlert('Réservation annulée avec succès.', 'success');
        this.userReservations = this.userReservations.filter(r => r.reservationId !== reservationId);
      },
      (error) => {
        this.loadingService.hide(); // Cacher le spinner en cas d'erreur
        this.alertService.showAlert('Erreur lors de l\'annulation de la réservation.', error);
      }
    );
  }

  /**
   * Ouvre l'Offcanvas pour noter un film.
   * @param reservationId L'ID de la réservation associée au film.
   */
  rateShowtime(reservationId: number): void {
    const selectedReservation = this.userReservations.find(r => r.reservationId === reservationId);
    if (!selectedReservation) {
      this.alertService.showAlert('Réservation introuvable.', 'warning');
      return;
    }

    this.createMovieRatingDto.movieId = selectedReservation.movieId;
    this.createMovieRatingDto.appUserId = this.authService.getCurrentUser()?.appUserId || '';

    // Afficher l'Offcanvas pour la notation
    const offcanvasElement = document.getElementById('reviewCanvas');
    if (offcanvasElement) {
      const offcanvasInstance = bootstrap.Offcanvas.getInstance(offcanvasElement);
      if (!offcanvasInstance) {
        new bootstrap.Offcanvas(offcanvasElement).show();
      } else {
        offcanvasInstance.show();
      }
    }
  }

    formatTimeRange(startTime: Date, endTime: Date): string {
    const startHour = new Date(startTime).getHours();
    const startMinute = new Date(startTime).getMinutes().toString().padStart(2, '0');
    const endHour = new Date(endTime).getHours();
    const endMinute = new Date(endTime).getMinutes().toString().padStart(2, '0');

    return `${startHour}h${startMinute}-${endHour}h${endMinute}`;
}

  /**
   * Soumet une note et un commentaire pour un film.
   */
  submitReview(): void {
    if (!this.createMovieRatingDto.movieId || !this.createMovieRatingDto.appUserId) {
      this.alertService.showAlert('Les données du film ou de l\'utilisateur sont manquantes.', 'warning');
      return;
    }

    this.loadingService.show(); // Afficher le spinner de chargement
    this.movieRatingService.submitMovieReview(this.createMovieRatingDto).subscribe(
      (response) => {
        this.loadingService.hide(); // Cacher le spinner après réussite
        this.alertService.showAlert('Votre avis a été soumis avec succès.', "success");

        // Fermer l'Offcanvas après soumission
        const offcanvasElement = document.getElementById('reviewCanvas');
        if (offcanvasElement) {
          const offcanvasInstance = bootstrap.Offcanvas.getInstance(offcanvasElement);
          if (offcanvasInstance) {
            offcanvasInstance.hide();
          }
        }

        // Réinitialiser le formulaire
        this.createMovieRatingDto.rating = 0;
        this.createMovieRatingDto.comment = '';
      },
      (error) => {
        this.loadingService.hide(); // Cacher le spinner en cas d'erreur
        this.alertService.showAlert('Erreur lors de la soumission de l\'avis.',"danger" ,error);
      }
    );
  }

  // Validateur personnalisé pour vérifier que les mots de passe correspondent
  passwordsMustMatch(formGroup: FormGroup): any {
    const passwordControl = formGroup.get('password');
    const confirmPasswordControl = formGroup.get('confirmPassword');

    if (!passwordControl || !confirmPasswordControl) {
      return null;
    }

    if (passwordControl.value !== confirmPasswordControl.value) {
      confirmPasswordControl.setErrors({ passwordsNotMatch: true });
    } else {
      confirmPasswordControl.setErrors(null);
    }

    return null;
  }

  /**
   * Gère la sélection d'un fichier pour la photo de profil.
   * @param event L'événement de sélection de fichier.
   */
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  /**
   * Télécharge une nouvelle image de profil pour l'utilisateur.
   */
  uploadProfileImage(): void {
    if (!this.selectedFile) {
      this.alertService.showAlert('Veuillez sélectionner une image avant de télécharger.', 'warning');
      return;
    }

    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.alertService.showAlert('Impossible de télécharger l\'image sans utilisateur connecté.', 'warning');
      return;
    }

    this.loadingService.show();
    this.authService.uploadUserProfile(currentUser.appUserId, this.selectedFile).subscribe(
      (response) => {
        this.loadingService.hide();
        this.userProfile.profilePictureUrl = response.Url;
        this.userProfileForm.patchValue({ profilePictureUrl: response.Url });
        this.alertService.showAlert('Image de profil téléchargée avec succès.', 'success');
      },
      (error) => {
        this.loadingService.hide();
        this.alertService.showAlert('Erreur lors du téléchargement de l\'image de profil.', 'danger');
      }
    );
  }

  /**
   * Supprime l'image de profil actuelle de l'utilisateur.
   */
  deleteProfileImage(): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser || !this.userProfile.profilePictureUrl) {
      this.alertService.showAlert('Impossible de supprimer l\'image sans utilisateur connecté ou image existante.', 'warning');
      return;
    }

    this.loadingService.show();
    this.authService.deleteUserProfileImage(currentUser.appUserId, this.userProfile.profilePictureUrl).subscribe(
      () => {
        this.loadingService.hide();
        this.userProfile.profilePictureUrl = '';
        this.userProfileForm.patchValue({ profilePictureUrl: '' });
        this.alertService.showAlert('Image de profil supprimée avec succès.', 'success');
      },
      (error) => {
        this.loadingService.hide(); // Cacher le spinner en cas d'erreur
        this.alertService.showAlert('Erreur lors de la suppression de l\'image de profil.', 'danger');
      }
    );
  }

  /**
   * Met à jour les informations du profil utilisateur.
   */
  updateUserProfile(): void {
    if (this.userProfileForm.invalid) {
      this.alertService.showAlert('Veuillez corriger les erreurs dans le formulaire.', 'warning');
      return;
    }

    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.alertService.showAlert('Impossible de mettre à jour le profil sans utilisateur connecté.', 'warning');
      return;
    }

    this.loadingService.show();
    const updatedProfile: UpdateAppUserDto = this.userProfileForm.value;
    this.authService.updateUserProfile(currentUser.appUserId, updatedProfile).subscribe(
      () => {
        this.loadingService.hide(); // Cacher le spinner après réussite
        this.alertService.showAlert('Profil utilisateur mis à jour avec succès.', 'success');
      },
      (error) => {
        this.loadingService.hide(); // Cacher le spinner en cas d'erreur
        this.alertService.showAlert('Erreur lors de la mise à jour du profil utilisateur.', 'danger', error);
      }
    );
  }

  onSubmitPassword(): void {
    if (this.passwordForm.invalid) {
      this.alertService.showAlert('Veuillez corriger les erreurs dans le formulaire.', 'warning');
      return;
    }
  
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.alertService.showAlert('Impossible de mettre à jour le mot de passe sans utilisateur connecté.', 'warning');
      return;
    }
  
    this.loadingService.show(); // Afficher le spinner de chargement
  
    const passwordData = this.passwordForm.value;
  
    // Appeler la méthode updateUserPassword avec un seul argument
    this.authService.updateUserPassword(currentUser.appUserId, passwordData).subscribe(
      () => {
        this.loadingService.hide(); // Cacher le spinner après réussite
        this.alertService.showAlert('Mot de passe mis à jour avec succès.', 'success');
      },
      (error) => {
        this.loadingService.hide(); // Cacher le spinner en cas d'erreur
        let errorMessage = 'Erreur lors de la mise à jour du mot de passe.';
        if (error.error && error.error.Message) {
          errorMessage = error.error.Message; // Utiliser le message d'erreur du back-end
        }
        this.alertService.showAlert(errorMessage, 'danger');
      }
    );
  }
}
