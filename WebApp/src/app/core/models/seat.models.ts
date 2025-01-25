export interface SeatDto {
    seatId: number;
    theaterId: number;
    seatNumber: string;
    isAccessible: boolean;
    isAvailable: boolean;
  }

  export interface AddHandicapSeatDto {
    theaterId: number;
    seatNumber: string;
  }

  export interface CreateSeatDto {
    theaterId: number;
    seatNumber: string;
    isAccessible: boolean;
  }

  export interface RemoveHandicapSeatDto {
    theaterId: number;
    seatNumber: string;
  }