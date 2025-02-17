import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CinemaDto} from '../models/cinema.models';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CinemaService {
  private apiUrl = `${environment.apiUrl}/Cinemas`;

  constructor(private http: HttpClient) {}

  
  /**
   * Récupère la liste de tous les cinémas.
   * @returns Observable contenant une liste de cinémas.
   */
  getAllCinemas(): Observable<CinemaDto[]> {
    return this.http.get<CinemaDto[]>(`${this.apiUrl}/cinemas`);
  }
}
