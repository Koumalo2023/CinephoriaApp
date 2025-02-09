import { ProjectionQuality } from "./enum.model";
import { ReservationDto } from "./reservation.models";

export interface ShowtimeDto {
    showtimeId: number;
    movieId: number;
    theaterId: number;
    cinemaId: number;
    startTime: Date;
    quality: ProjectionQuality;
    endTime: Date;
    price: number;
    priceAdjustment: number;
    isPromotion: boolean;
    reservations: ReservationDto[];
  }

  export interface CreateShowtimeDto {
    showtimeId?:number;
    movieId: number;
    theaterId: number;
    cinemaId: number;
    startTime: Date;
    quality: ProjectionQuality;
    endTime: Date;
    priceAdjustment: number;
    isPromotion: boolean;
  }
  
  export interface UpdateShowtimeDto {
    showtimeId: number;
    movieId: number;
    theaterId: number;
    cinemaId: number;
    startTime: Date;
    quality: ProjectionQuality;
    endTime: Date;
    priceAdjustment: number;
    isPromotion: boolean;
  }