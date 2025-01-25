import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SeatDto, AddHandicapSeatDto, RemoveHandicapSeatDto } from '../models/seat.models';

@Injectable({
  providedIn: 'root'
})
export class SeatsService {
  private apiUrl = 'api/seats';

  constructor(private http: HttpClient) {}

  /**
   * Récupère la liste des sièges disponibles pour une séance spécifique.
   * @param sessionId L'identifiant de la séance.
   * @returns Observable contenant une liste de sièges disponibles.
   */
  getAvailableSeats(sessionId: number): Observable<SeatDto[]> {
    return this.http.get<SeatDto[]>(`${this.apiUrl}/available/${sessionId}`);
  }

  /**
   * Ajoute un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
   * @param addHandicapSeatDto Les données du siège à ajouter (AddHandicapSeatDto).
   * @returns Observable contenant un message de réussite.
   */
  addHandicapSeat(addHandicapSeatDto: AddHandicapSeatDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/handicap-add-seat`, addHandicapSeatDto);
  }

  /**
   * Supprime un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
   * @param dto Les données du siège à supprimer (RemoveHandicapSeatDto).
   * @returns Observable contenant un message de réussite.
   */
  removeHandicapSeat(removeHandicapSeatDto: RemoveHandicapSeatDto): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/handicap-delete-seat`, { body: removeHandicapSeatDto });
  }
}