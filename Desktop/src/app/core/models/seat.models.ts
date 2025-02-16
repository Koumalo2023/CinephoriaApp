export interface SeatDto {
    seatId: number;
    theaterId: number;
    seatNumber: string;
    isAccessible: boolean;
    isAvailable: boolean;
  }
  
  export interface Seat {
    seatId: number;
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

  export interface UpdateSeatDto {
    seatId: number; 
    seatNumber: string; // Numéro du siège (ex: "A1", "B2")
    isAccessible: boolean;
    isAvailable: boolean;
  }
  

  export interface RemoveHandicapSeatDto {
    theaterId: number;
    seatNumber: string;
  }