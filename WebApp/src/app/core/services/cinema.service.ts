import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CinemaDto, CreateCinemaDto, UpdateCinemaDto } from '../models/cinema.models';

@Injectable({
  providedIn: 'root'
})
export class CinemaService {
  private apiUrl = 'api/cinemas';

  constructor(private http: HttpClient) {}

  /**
   * Crée un nouveau cinéma.
   * @param createCinemaDto Les données du cinéma à créer.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  createCinema(createCinemaDto: CreateCinemaDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/create`, createCinemaDto);
  }

  /**
   * Récupère la liste de tous les cinémas.
   * @returns Observable contenant une liste de cinémas.
   */
  getAllCinemas(): Observable<CinemaDto[]> {
    return this.http.get<CinemaDto[]>(`${this.apiUrl}/cinemas`);
  }

  /**
   * Récupère un cinéma par son identifiant.
   * @param cinemaId L'identifiant du cinéma.
   * @returns Observable contenant les informations du cinéma.
   */
  getCinemaById(cinemaId: number): Observable<CinemaDto> {
    return this.http.get<CinemaDto>(`${this.apiUrl}/cinema/${cinemaId}`);
  }

  /**
   * Met à jour les informations d'un cinéma existant.
   * @param updateCinemaDto Les données à mettre à jour pour le cinéma.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  updateCinema(updateCinemaDto: UpdateCinemaDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}`, updateCinemaDto);
  }

  /**
   * Supprime un cinéma par son identifiant.
   * @param cinemaId L'identifiant du cinéma à supprimer.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  deleteCinema(cinemaId: number): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/cinema/${cinemaId}`);
  }
}
