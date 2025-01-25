import { TheaterDto } from "./theater.models";

export interface CinemaDto {
    cinemaId: number;
    name: string;
    address: string;
    phoneNumber: string;
    city: string;
    country: string;
    openingHours: string;
    theaters: TheaterDto[];
  }

  export interface CreateCinemaDto {
    name: string;
    address: string;
    phoneNumber: string;
    city: string;
    country: string;
    openingHours?: string;
  }

  export interface UpdateCinemaDto {
    cinemaId: number;
    name?: string;
    address?: string;
    phoneNumber?: string;
    city?: string;
    country?: string;
    openingHours?: string;
  }