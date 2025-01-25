import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ShowtimeDto, CreateShowtimeDto, UpdateShowtimeDto } from '../models/showtime.models';

@Injectable({
  providedIn: 'root'
})
export class ShowtimeService {
  private apiUrl = 'api/showtime'; 

  constructor(private http: HttpClient) {}

  /**
   * Crée une nouvelle séance.
   * @param createShowtimeDto Les données de la séance à créer.
   * @returns Observable contenant un message de réussite.
   */
  createShowtime(createShowtimeDto: CreateShowtimeDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/create`, createShowtimeDto);
  }

  /**
   * Met à jour une séance existante.
   * @param updateShowtimeDto Les données mises à jour de la séance.
   * @returns Observable contenant un message de réussite.
   */
  updateShowtime(updateShowtimeDto: UpdateShowtimeDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}/update`, updateShowtimeDto);
  }

  /**
   * Supprime une séance existante.
   * @param showtimeId L'identifiant de la séance à supprimer.
   * @returns Observable contenant un message de réussite.
   */
  deleteShowtime(showtimeId: number): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/delete/${showtimeId}`);
  }

  /**
   * Récupère la liste de toutes les séances.
   * @returns Observable contenant une liste de séances.
   */
  getAllShowtimes(): Observable<ShowtimeDto[]> {
    return this.http.get<ShowtimeDto[]>(`${this.apiUrl}/all`);
  }

  /**
   * Récupère les détails d'une séance spécifique.
   * @param showtimeId L'identifiant de la séance.
   * @returns Observable contenant les détails de la séance.
   */
  getShowtimeDetails(showtimeId: number): Observable<ShowtimeDto> {
    return this.http.get<ShowtimeDto>(`${this.apiUrl}/${showtimeId}`);
  }
}