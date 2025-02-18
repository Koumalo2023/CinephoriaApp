import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';
import {
  MovieDto,
  MovieReviewDto,
  CreateMovieDto,
  UpdateMovieDto,
  FilterMoviesRequestDto,
  MovieDetailsDto
} from '../models/movie.models'; 
import { ShowtimeDto } from '../models/showtime.models';
import { ApiConfigService } from './apiConfigService.service';

@Injectable({
  providedIn: 'root'
})
export class MovieService {
  private apiUrl: string;

  constructor(private http: HttpClient, private apiConfigService: ApiConfigService) {
    this.apiUrl = this.apiConfigService.getMoviesUrl(); // Configurez l'URL de base
  }

  /**
   * Récupère la liste des derniers films ajoutés.
   * @returns Observable contenant une liste de films.
   */
  getRecentMovies(): Observable<MovieDto[]> {
    return this.http.get<MovieDto[]>(`${this.apiUrl}/recent`).pipe(
      map((movies) =>
        movies.map((movie) => ({
          ...movie,
          posterUrls: movie.posterUrls.startsWith('http') ? movie.posterUrls : `${this.apiUrl.replace(/\/$/, '')}/${movie.posterUrls.replace(/^\//, '')}`
        }))
      )
    );
  }

  /**
   * Récupère la liste de tous les films.
   * @returns Observable contenant une liste de films.
   */
  getAllMovies(): Observable<MovieDto[]> {
    return this.http.get<MovieDto[]>(`${this.apiUrl}/all`).pipe(
      map((movies) =>
        movies.map((movie) => ({
          ...movie,
          posterUrls: movie.posterUrls.startsWith('http') ? movie.posterUrls : `${this.apiUrl.replace(/\/$/, '')}/${movie.posterUrls.replace(/^\//, '')}`
        }))
      )
    );
  }

  /**
   * Récupère les détails d'un film en fonction de son identifiant.
   * @param movieId L'identifiant du film.
   * @returns Observable contenant les détails du film.
   */
  getMovieDetails(movieId: number): Observable<MovieDetailsDto> {
    return this.http.get<MovieDetailsDto>(`${this.apiUrl}/movie/${movieId}`).pipe(
      map((movie) => ({
        ...movie,
        posterUrls: movie.posterUrls.startsWith('http') ? movie.posterUrls : `${this.apiUrl.replace(/\/$/, '')}/${movie.posterUrls.replace(/^\//, '')}`
      }))
    );
  }

  /**
   * Récupère la liste des films qui ont des séances dans un cinéma spécifique.
   * @param cinemaId L'identifiant du cinema.
   * @returns Observable contenant une liste de films.
   */
  getMoviesByCinemaId(cinemaId: number): Observable<MovieDto[]> {
    return this.http.get<MovieDto[]>(`${this.apiUrl}/cinema/${cinemaId}/movies`).pipe(
      map((movies) =>
        movies.map((movie) => ({
          ...movie,
          posterUrls: movie.posterUrls.startsWith('http') ? movie.posterUrls : `${this.apiUrl.replace(/\/$/, '')}/${movie.posterUrls.replace(/^\//, '')}`
        }))
      )
    );
  }

  /**
   * Récupère la liste des séances disponibles pour un film spécifique.
   * @param movieId L'identifiant du film.
   * @returns Observable contenant une liste de séances.
   */
  getMovieSessions(movieId: number): Observable<ShowtimeDto[]> {
    return this.http.get<ShowtimeDto[]>(`${this.apiUrl}/${movieId}/sessions`);
  }

  /**
   * Soumet un avis sur un film.
   * @param reviewDto Les données de l'avis.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  submitMovieReview(reviewDto: MovieReviewDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/review`, reviewDto);
  }

  /**
   * Filtre les films en fonction des critères donnés.
   * @param filterDto Les critères de filtrage.
   * @returns Observable contenant une liste de films correspondant aux critères.
   */
  filterMovies(filterDto: FilterMoviesRequestDto): Observable<MovieDto[]> {
    return this.http.post<MovieDto[]>(`${this.apiUrl}/filter`, filterDto).pipe(
      map((movies) =>
        movies.map((movie) => ({
          ...movie,
          posterUrls: movie.posterUrls.startsWith('http') ? movie.posterUrls : `${this.apiUrl.replace(/\/$/, '')}/${movie.posterUrls.replace(/^\//, '')}`
        }))
      )
    );
  }

  /**
   * Crée un nouveau film.
   * @param createMovieDto Les données du film à créer.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  createMovie(createMovieDto: CreateMovieDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/create`, createMovieDto);
  }

  /**
   * Télécharge une affiche pour un film.
   * @param movieId L'identifiant du film.
   * @param file Le fichier image à télécharger.
   * @returns Observable contenant l'URL de l'image et un message de réussite.
   */
  uploadMoviePoster(movieId: number, file: File): Observable<{ Url: string; Message: string }> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<{ Url: string; Message: string }>(
      `${this.apiUrl}/upload-movie-poster/${movieId}`,
      formData
    );
  }

  /**
   * Supprime l'affiche d'un film.
   * @param movieId L'identifiant du film.
   * @param posterUrl L'URL de l'affiche à supprimer.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  deleteMoviePoster(movieId: number, posterUrl: string): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/delete-movie-poster/${movieId}`, {
      params: { posterUrl }
    });
  }

  /**
   * Met à jour les informations d'un film.
   * @param updateMovieDto Les nouvelles données du film.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  updateMovie(updateMovieDto: UpdateMovieDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}`, updateMovieDto);
  }

  /**
   * Supprime un film en fonction de son identifiant.
   * @param movieId L'identifiant du film à supprimer.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  deleteMovie(movieId: number): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/movie/${movieId}`);
  }
}