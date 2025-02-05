import { ReservationStatus } from "./enum.model";
import { SeatDto } from "./seat.models";

export interface ReservationDto {
    reservationId: number;
    appUserId: string;
    showtimeId: number;
    seats: SeatDto[];
    totalPrice: number;
    qrCode: string;
    isValidated: boolean;
    status: ReservationStatus;
    numberOfSeats: number;
}

export interface CancelReservationDto {
    reservationId: number;
}

export interface CreateReservationDto {
    appUserId: string;
    showtimeId: number;
    seatNumbers: string[];
}

export interface UpdateReservationDto {
    reservationId: number;
    status: ReservationStatus;
    isValidated: boolean;
}

export interface UserReservationDto {
    reservationId: number;
    appUserId: string;
    showtimeId: number;
    movieId: number;
    totalPrice: number;
    qrCode: string;
    isValidated: boolean;
    status: ReservationStatus;
    numberOfSeats: number;
    seats: SeatDto[];
    movieName: string;
    cinemaName: string;
    startTime: Date;
    endTime: Date;
}