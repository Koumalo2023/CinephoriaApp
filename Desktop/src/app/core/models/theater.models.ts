import { SeatDto } from "./seat.models";

  export interface TheaterDto {
    theaterId: number;
    name: string;
    seatCount: number;
    cinemaId: number;
    isOperational: boolean;
    projectionQuality: ProjectionQuality;
    seats: SeatDto[];
  }


  export enum ProjectionQuality {
    FourDX = '4DX',
    ThreeD = '3D',
    IMAX = 'IMAX',
    FourK = '4K',
    Standard2D='2D',
    DolbyCinema='DolbyC'
  }