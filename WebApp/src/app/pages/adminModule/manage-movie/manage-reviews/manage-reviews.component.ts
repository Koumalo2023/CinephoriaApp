import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateMovieRatingDto, MovieRatingDto, MovieReviewDto, UpdateMovieRatingDto } from '@app/core/models/movie-rating.models';
import { AlertService } from '@app/core/services/alert.service';
import { LoadingService } from '@app/core/services/loading.service';
import { MovieRatingService } from '@app/core/services/movie-rating.service';

@Component({
  selector: 'app-manage-reviews',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './manage-reviews.component.html',
  styleUrl: './manage-reviews.component.scss'
})
export class ManageReviewsComponent implements OnInit {
  reviews: MovieRatingDto[] = [];
  movieId: number = 0; // ID du film pour lequel on affiche les avis
  reviewForm!: FormGroup;
  mode: 'create' | 'edit' = 'create';
  selectedReviewId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private reviewService: MovieRatingService,
    private alertService: AlertService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    this.initReviewForm();
  }

  /**
   * Initialise le formulaire pour soumettre ou modifier un avis.
   */
  initReviewForm(review?: Partial<UpdateMovieRatingDto>): void {
    this.reviewForm = this.fb.group({
      movieRatingId:[review?.comment ],
      movieId: [this.movieId, Validators.required],
      appUserId: ['current-user-id', Validators.required], // Remplacez 'current-user-id' par l'ID réel de l'utilisateur connecté
      rating: [review?.rating || '', [Validators.required, Validators.min(1), Validators.max(5)]],
      comment: [review?.comment || '', Validators.maxLength(500)]
    });
  }

  /**
   * Charge la liste des avis pour un film donné.
   */
  loadReviews(movieId: number): void {
    this.loadingService.show();
    this.movieId = movieId;
    this.reviewService.getMovieReviews(movieId).subscribe(
      (data: MovieRatingDto[]) => {
        this.reviews = data;
        this.loadingService.hide();
      },
      (error) => {
        this.alertService.showAlert('Erreur lors du chargement des avis.', 'danger');
        this.loadingService.hide();
      }
    );
  }

  /**
   * Soumet un nouvel avis.
   */
  submitReview(): void {
    if (this.reviewForm.invalid) {
      this.alertService.showAlert('Veuillez remplir tous les champs obligatoires.', 'warning');
      return;
    }

    const createDto: CreateMovieRatingDto = {
      movieId: this.reviewForm.value.movieId,
      appUserId: this.reviewForm.value.appUserId,
      rating: this.reviewForm.value.rating,
      comment: this.reviewForm.value.comment
    };

    this.loadingService.show();
    this.reviewService.submitMovieReview(createDto).subscribe(
      (response) => {
        this.alertService.showAlert(response.Message, 'success');
        this.loadReviews(this.movieId); // Rafraîchir la liste des avis
        this.resetForm();
      },
      (error) => {
        this.alertService.showAlert('Erreur lors de la soumission de l\'avis.', 'danger');
        this.loadingService.hide();
      }
    );
  }

  /**
   * Met à jour un avis existant.
   */
  updateReview(): void {
    if (this.reviewForm.invalid || !this.selectedReviewId) {
      this.alertService.showAlert('Veuillez sélectionner un avis à modifier.', 'warning');
      return;
    }

    const updateDto: UpdateMovieRatingDto = {
      movieRatingId: this.selectedReviewId,
      rating: this.reviewForm.value.rating,
      comment: this.reviewForm.value.comment
    };

    this.loadingService.show();
    this.reviewService.updateReview(updateDto).subscribe(
      (response) => {
        this.alertService.showAlert(response.Message, 'success');
        this.loadReviews(this.movieId); // Rafraîchir la liste des avis
        this.resetForm();
      },
      (error) => {
        this.alertService.showAlert('Erreur lors de la mise à jour de l\'avis.', 'danger');
        this.loadingService.hide();
      }
    );
  }

  /**
   * Valide un avis.
   * @param reviewId L'ID de l'avis à valider.
   */
  validateReview(reviewId: number): void {
    if (confirm('Êtes-vous sûr de vouloir valider cet avis ?')) {
      this.loadingService.show();
      this.reviewService.validateReview(reviewId).subscribe(
        (response) => {
          this.alertService.showAlert(response.Message, 'success');
          this.loadReviews(this.movieId); // Rafraîchir la liste des avis
        },
        (error) => {
          this.alertService.showAlert('Erreur lors de la validation de l\'avis.', 'danger');
          this.loadingService.hide();
        }
      );
    }
  }

  /**
   * Supprime un avis.
   * @param reviewId L'ID de l'avis à supprimer.
   */
  deleteReview(reviewId: number): void {
    if (confirm('Êtes-vous sûr de vouloir supprimer cet avis ?')) {
      this.loadingService.show();
      this.reviewService.deleteReview(reviewId).subscribe(
        (response) => {
          this.alertService.showAlert(response.Message, 'success');
          this.loadReviews(this.movieId); // Rafraîchir la liste des avis
        },
        (error) => {
          this.alertService.showAlert('Erreur lors de la suppression de l\'avis.', 'danger');
          this.loadingService.hide();
        }
      );
    }
  }

  /**
   * Réinitialise le formulaire.
   */
  resetForm(): void {
    this.mode = 'create';
    this.selectedReviewId = null;
    this.initReviewForm();
  }

  /**
   * Ouvre le formulaire en mode édition pour un avis spécifique.
   * @param review L'avis à éditer.
   */
  openEditReview(review: MovieReviewDto): void {
    this.mode = 'edit';
    this.selectedReviewId = review.movieId;
    this.initReviewForm({
      rating: review.rating,
      comment: review.description
    });
  }
}