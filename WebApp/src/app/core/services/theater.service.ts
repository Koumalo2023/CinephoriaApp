import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TheaterDto, CreateTheaterDto, UpdateTheaterDto,} from '../models/theater.models';
import { IncidentDto } from '../models/incident.models';
import { ApiConfigService } from './apiConfigService.service';

@Injectable({
  providedIn: 'root'
})
export class TheaterService {
   private apiUrl: string;
  
    constructor(private http: HttpClient, private apiConfigService: ApiConfigService) {
      this.apiUrl = this.apiConfigService.getTheaterUrl();
    }

  /**
   * Récupère la liste des salles de cinéma associées à un cinéma spécifique.
   * @param cinemaId L'identifiant du cinéma.
   * @returns Observable contenant une liste de salles de cinéma.
   */
  getTheatersByCinema(cinemaId: number): Observable<TheaterDto[]> {
    return this.http.get<TheaterDto[]>(`${this.apiUrl}/by-cinema/${cinemaId}`);
  }

  /**
   * Récupère une salle de cinéma par son identifiant.
   * @param theaterId L'identifiant de la salle.
   * @returns Observable contenant les détails de la salle de cinéma.
   */
  getTheaterById(theaterId: number): Observable<TheaterDto> {
    return this.http.get<TheaterDto>(`${this.apiUrl}/${theaterId}`);
  }

  /**
   * Crée une nouvelle salle de cinéma.
   * @param createTheaterDto Les données de la salle à créer.
   * @returns Observable contenant un message de réussite.
   */
  createTheater(createTheaterDto: CreateTheaterDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/create`, createTheaterDto);
  }

  /**
   * Met à jour les informations d'une salle de cinéma existante.
   * @param updateTheaterDto Les données de la salle à mettre à jour.
   * @returns Observable contenant un message de réussite.
   */
  updateTheater(updateTheaterDto: UpdateTheaterDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}/update`, updateTheaterDto);
  }

  /**
   * Supprime une salle de cinéma en fonction de son identifiant.
   * @param theaterId L'identifiant de la salle à supprimer.
   * @returns Observable contenant un message de réussite.
   */
  deleteTheater(theaterId: number): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/delete/${theaterId}`);
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