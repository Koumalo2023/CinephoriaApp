import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateIncidentDto,
  UpdateIncidentDto,
  IncidentStatusUpdateDto,
  IncidentDto
} from '../models/incident.models';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class IncidentService {
  private apiUrl = `${environment.apiUrl}/Incident`;

  constructor(private http: HttpClient) { }

  /**
   * Signale un nouvel incident dans une salle de cinéma.
   * @param createIncidentDto Les données de l'incident à signaler.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  reportIncident(createIncidentDto: CreateIncidentDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/report`, createIncidentDto);
  }

  /**
   * Récupère les détails d'un incident en fonction de son identifiant.
   * @param incidentId L'identifiant de l'incident.
   * @returns Observable contenant les détails de l'incident.
   */
  getIncidentDetails(incidentId: number): Observable<IncidentDto> {
    return this.http.get<IncidentDto>(`${this.apiUrl}/${incidentId}`);
  }

  /**
   * Récupère la liste des incidents associés à un cinéma spécifique.
   * @param cinemaId L'identifiant du cinéma.
   * @returns Observable contenant une liste d'incidents.
   */
  getIncidentsByCinema(cinemaId: number): Observable<IncidentDto[]> {
    return this.http.get<IncidentDto[]>(`${this.apiUrl}/cinema/${cinemaId}`);
  }

  /**
   * Récupère tous les incidents dans tous les cinémas.
   * @returns Observable contenant une liste de tous les incidents.
   */
  getAllIncidents(): Observable<IncidentDto[]> {
    return this.http.get<IncidentDto[]>(`${this.apiUrl}`);
  }

  /**
 * Télécharge une image pour un incident.
 * @param incidentId L'identifiant de l'incident.
 * @param file Le fichier image à télécharger.
 * @returns Observable contenant l'URL de l'image et un message de réussite.
 */
  uploadIncidentImage(incidentId: number, file: File): Observable<{ Url: string; Message: string }> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<{ Url: string; Message: string }>(
      `${this.apiUrl}/upload-incident-image/${incidentId}`,
      formData
    );
  }

  /**
   * Récupère les URLs des images associées à un incident.
   * @param incidentId L'identifiant de l'incident.
   * @returns Observable contenant la liste des URLs des images.
   */
  getIncidentImages(incidentId: number): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/${incidentId}/images`);
  }

  /**
   * Récupère les incidents associés à une salle de cinéma spécifique.
   * @param theaterId L'identifiant de la salle de cinéma.
   * @returns Observable contenant une liste d'incidents.
   */
  getTheaterIncidents(theaterId: number): Observable<IncidentDto[]> {
    return this.http.get<IncidentDto[]>(`${this.apiUrl}/theater/${theaterId}`);
  }

  /**
   * Met à jour le statut d'un incident.
   * @param incidentStatusUpdateDto Les données de mise à jour du statut de l'incident.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  updateIncidentStatus(incidentStatusUpdateDto: IncidentStatusUpdateDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}/status`, incidentStatusUpdateDto);
  }

  /**
   * Met à jour les informations d'un incident existant.
   * @param updateIncidentDto Les données à mettre à jour pour l'incident.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  updateIncident(updateIncidentDto: UpdateIncidentDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}`, updateIncidentDto);
  }

  /**
   * Supprime un incident par son identifiant.
   * @param incidentId L'identifiant de l'incident à supprimer.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  deleteIncident(incidentId: number): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/${incidentId}`);
  }
}
