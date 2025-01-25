import { ProjectionQuality } from "./enum.model";
import { SeatDto } from "./seat.models";

export interface CreateTheaterDto {
    name: string;
    seatCount: number;
    cinemaId: number;
    projectionQuality: ProjectionQuality;
  }

  export interface TheaterDto {
    theaterId: number;
    name: string;
    seatCount: number;
    cinemaId: number;
    isOperational: boolean;
    projectionQuality: ProjectionQuality;
    seats: SeatDto[];
  }

  export interface UpdateTheaterDto {
    theaterId: number;
    name: string;
    seatCount: number;
    cinemaId: number;
    projectionQuality: ProjectionQuality;
  }

  