import { TheaterDto } from "./theater.models";

export interface CinemaDto {
    cinemaId: number;
    name: string;
    address: string;
    phoneNumber: string;
    city: string;
    country: string;
    openingHours: string;
    theaterCount?: number;
    theaters: TheaterDto[];
  }
