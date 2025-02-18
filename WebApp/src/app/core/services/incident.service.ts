import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateIncidentDto, UpdateIncidentDto, IncidentStatusUpdateDto, IncidentDto } from '../models/incident.models';
import { ApiConfigService } from './apiConfigService.service';

@Injectable({
  providedIn: 'root'
})
export class IncidentService {
  private apiUrl: string;

  constructor(private http: HttpClient, private apiConfigService: ApiConfigService) {
    this.apiUrl = this.apiConfigService.getIncidentUrl(); 
  }

  // Les autres méthodes restent inchangées, mais utilisez `this.apiUrl` pour les URLs
  reportIncident(createIncidentDto: CreateIncidentDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/report`, createIncidentDto);
  }

  getIncidentDetails(incidentId: number): Observable<IncidentDto> {
    return this.http.get<IncidentDto>(`${this.apiUrl}/${incidentId}`);
  }

  getIncidentsByCinema(cinemaId: number): Observable<IncidentDto[]> {
    return this.http.get<IncidentDto[]>(`${this.apiUrl}/cinema/${cinemaId}`);
  }

  getAllIncidents(): Observable<IncidentDto[]> {
    return this.http.get<IncidentDto[]>(`${this.apiUrl}`);
  }

  uploadIncidentImage(incidentId: number, file: File): Observable<{ Url: string; Message: string }> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<{ Url: string; Message: string }>(
      `${this.apiUrl}/upload-incident-image/${incidentId}`,
      formData
    );
  }

  getTheaterIncidents(theaterId: number): Observable<IncidentDto[]> {
    return this.http.get<IncidentDto[]>(`${this.apiUrl}/theater/${theaterId}`);
  }

  updateIncidentStatus(incidentStatusUpdateDto: IncidentStatusUpdateDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}/status`, incidentStatusUpdateDto);
  }

  updateIncident(updateIncidentDto: UpdateIncidentDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}`, updateIncidentDto);
  }

  deleteIncident(incidentId: number): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/${incidentId}`);
  }
}