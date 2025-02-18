import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiConfigService } from './apiConfigService.service';

@Injectable({
  providedIn: 'root'
})
export class ImageService {
 private apiUrl: string;
 
   constructor(private http: HttpClient, private apiConfigService: ApiConfigService) {
     this.apiUrl = this.apiConfigService.getImageUrl();
   }

  uploadPoster(movieId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post(`${this.apiUrl}/upload-movie-poster/${movieId}`, formData);
  }
}
