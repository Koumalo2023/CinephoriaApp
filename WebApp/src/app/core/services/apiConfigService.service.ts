// api-config.service.ts
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
@Injectable({
    providedIn: 'root',
})
export class ApiConfigService {
    private readonly apiUrl = environment.apiUrl;

    getMoviesUrl(): string {
        return `${this.apiUrl}/Movie`;
    }

    getCinemasUrl(): string {
        return `${this.apiUrl}/Cinema`;
    }

    getShowtimesUrl(): string {
        return `${this.apiUrl}/Showtime`;
    }

    getAuthUrl(): string {
        return `${this.apiUrl}/Auth`;
    }

    getIncidentUrl(): string {
        return `${this.apiUrl}/Incident`;
    }
    getMovieRatingUrl(): string {
        return `${this.apiUrl}/MovieRating`;
    }

    getSeatsUrl(): string {
        return `${this.apiUrl}/Seats`;
    }

    getReservationUrl(): string {
        return `${this.apiUrl}/Reservation`;
    }

    getTheaterUrl(): string {
        return `${this.apiUrl}/Theater`;
    }

    getImageUrl(): string {
        return `${this.apiUrl}/Image`;
    }


}