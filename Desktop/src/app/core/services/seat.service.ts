import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SeatDto, AddHandicapSeatDto, RemoveHandicapSeatDto, UpdateSeatDto, Seat } from '../models/seat.models';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SeatsService {
  private apiUrl = `${environment.apiUrl}/Seats`;

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
   * Récupère tous les sièges d'une salle de cinéma par son identifiant.
   * @param theaterId L'identifiant de la salle.
   * @returns Observable contenant la liste des sièges.
   */
   getSeatsByTheaterId(theaterId: number): Observable<Seat[]> {
    return this.http.get<Seat[]>(`${this.apiUrl}/theater/${theaterId}`);
  }

  /**
   * Met à jour un siège existant.
   * @param updateSeatDto Les nouvelles informations du siège.
   * @returns Observable contenant un message de réussite.
   */
   updateSeat(updateSeatDto: UpdateSeatDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}/update`, updateSeatDto);
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