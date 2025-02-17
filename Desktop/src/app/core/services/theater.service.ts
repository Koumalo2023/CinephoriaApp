import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TheaterDto} from '../models/theater.models';
import { IncidentDto } from '../models/incident.models';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TheaterService {
  private apiUrl = `${environment.apiUrl}/Theater`;

  constructor(private http: HttpClient) {}

  /**
   * Récupère la liste des salles de cinéma associées à un cinéma spécifique.
   * @param cinemaId L'identifiant du cinéma.
   * @returns Observable contenant une liste de salles de cinéma.
   */
  getTheatersByCinema(cinemaId: number): Observable<TheaterDto[]> {
    return this.http.get<TheaterDto[]>(`${this.apiUrl}/by-cinema/${cinemaId}`);
  }

  /**
   * Récupère la liste des incidents associés à une salle de cinéma.
   * @param theaterId L'identifiant de la salle de cinéma.
   * @returns Observable contenant une liste d'incidents.
   */
  getTheaterIncidents(theaterId: number): Observable<IncidentDto[]> {
    return this.http.get<IncidentDto[]>(`${this.apiUrl}/${theaterId}/incidents`);
  }
}