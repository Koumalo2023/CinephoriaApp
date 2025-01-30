import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateMovieRatingDto,
  UpdateMovieRatingDto,
  MovieReviewDto
} from '../models/movie-rating.models';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MovieRatingService {
  private apiUrl = `${environment.apiUrl}/MovieRating`; // Base URL de l'API

  constructor(private http: HttpClient) {}

  /**
   * Soumet un nouvel avis sur un film.
   * @param createMovieRatingDto Les données de l'avis à soumettre.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  submitMovieReview(createMovieRatingDto: CreateMovieRatingDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/submit`, createMovieRatingDto);
  }

  /**
   * Valide un avis sur un film.
   * @param reviewId L'identifiant de l'avis à valider.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  validateReview(reviewId: number): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}/validate/${reviewId}`, {});
  }

  /**
   * Supprime un avis sur un film.
   * @param reviewId L'identifiant de l'avis à supprimer.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  deleteReview(reviewId: number): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/delete/${reviewId}`);
  }

  /**
   * Récupère la liste des avis associés à un film.
   * @param movieId L'identifiant du film.
   * @returns Observable contenant une liste d'avis.
   */
  getMovieReviews(movieId: number): Observable<MovieReviewDto[]> {
    return this.http.get<MovieReviewDto[]>(`${this.apiUrl}/movie/${movieId}`);
  }

  /**
   * Récupère les détails d'un avis spécifique.
   * @param reviewId L'identifiant de l'avis.
   * @returns Observable contenant les détails de l'avis.
   */
  getReviewDetails(reviewId: number): Observable<MovieReviewDto> {
    return this.http.get<MovieReviewDto>(`${this.apiUrl}/${reviewId}`);
  }

  /**
   * Met à jour un avis existant.
   * @param updateMovieRatingDto Les données mises à jour de l'avis.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  updateReview(updateMovieRatingDto: UpdateMovieRatingDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}/update`, updateMovieRatingDto);
  }
}
