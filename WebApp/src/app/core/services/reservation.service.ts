import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import {
  CreateReservationDto,
  ReservationDto,
  UserReservationDto,
} from '../models/reservation.models'; 
import { ShowtimeDto } from '../models/showtime.models';
import { SeatDto } from '../models/seat.models';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  private apiUrl =`${environment.apiUrl}/Reservation`;

  
  private currentReservationSubject = new BehaviorSubject<any>(null);
  currentReservation$ = this.currentReservationSubject.asObservable();

  constructor(private http: HttpClient) {}

  /**
   * Récupère la liste des séances disponibles pour un film spécifique.
   * @param movieId L'identifiant du film.
   * @returns Observable contenant une liste de séances disponibles.
   */
  getMovieSessions(movieId: number): Observable<ShowtimeDto[]> {
    return this.http.get<ShowtimeDto[]>(`${this.apiUrl}/movie/${movieId}/sessions`);
  }

  /**
   * Récupère la liste des sièges disponibles pour une séance spécifique.
   * @param showtimeId L'identifiant de la séance.
   * @returns Observable contenant une liste de sièges disponibles.
   */
  getAvailableSeats(showtimeId: number): Observable<SeatDto[]> {
    return this.http.get<SeatDto[]>(`${this.apiUrl}/showtime/${showtimeId}/seats`);
  }

  /**
   * Récupère la liste des réservations d'un utilisateur.
   * @param userId L'identifiant de l'utilisateur.
   * @returns Observable contenant une liste de réservations.
   */
  getUserReservations(userId: string): Observable<UserReservationDto[]> {
    return this.http.get<UserReservationDto[]>(`${this.apiUrl}/user/${userId}`);
  }

  /**
   * Récupère la liste de toutes les réservations d'une séance spécifique.
   * @param showtimeId L'identifiant de la séance.
   * @returns Observable contenant une liste de réservations.
   */
  getReservationsByShowtime(showtimeId: number): Observable<ReservationDto[]> {
    return this.http.get<ReservationDto[]>(`${this.apiUrl}/showtime/${showtimeId}`);
  }

  /**
   * Valide un QRCode scanné pour une réservation.
   * @param qrCodeData Les données du QRCode scanné.
   * @returns Observable contenant le résultat de la validation.
   */
  validateSession(qrCodeData: string): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/validate`, { qrCodeData });
  }

  /**
   * Crée une nouvelle réservation.
   * @param createReservationDto Les données de la réservation à créer.
   * @returns Observable contenant un message de réussite.
   */
  createReservation(createReservationDto: CreateReservationDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/create`, createReservationDto);
  }

  /**
   * Annule une réservation existante.
   * @param reservationId L'identifiant de la réservation à annuler.
   * @returns Observable contenant un message de réussite.
   */
  cancelReservation(reservationId: number): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/cancel/${reservationId}`);
  }



  setCurrentReservation(reservationDetails: any): void {
    this.currentReservationSubject.next(reservationDetails);
  }

  getCurrentReservation(): any {
    return this.currentReservationSubject.value;
  }

  clearCurrentReservation(): void {
    this.currentReservationSubject.next(null);
  }
}
