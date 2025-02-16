
import { Incident, IncidentDto } from "./incident.models";


export interface User {
    appUserId: string;
    firstName: string;
    lastName: string;
    email: string;
    userName: string;
    emailConfirmed: boolean;
    phoneNumber: string;
    createdAt: Date;
    updatedAt: Date;
    hasApprovedTermsOfUse: boolean;
    hiredDate?: Date;
    position?: string;
    profilePictureUrl?: string;
    reportedIncidents: IncidentDto[];
    resolvedByIncidents: Incident[];
    role: UserRole;
    reservations: ReservationDto[];
    movieRatings: MovieRatingDto[];
}

export interface UserDto {
    appUserId: string;
    firstName: string;
    lastName: string;
    email: string;
    userName: string;
    emailConfirmed: boolean;
    phoneNumber: string;
    createdAt: Date;
    updatedAt: Date;
    hasApprovedTermsOfUse: boolean;
    hiredDate?: Date;
    position?: string;
    profilePictureUrl?: string;
    reportedIncidents: IncidentDto[];
    resolvedByIncidents: Incident[];
    role: UserRole;
    reservations: ReservationDto[];
    movieRatings: MovieRatingDto[];
}

export interface EmployeeProfileDto {
    employeeId: string;
    firstName: string;
    lastName: string;
    email: string;
    position?: string;
    hiredDate?: Date;
    createdAt: Date;
    updatedAt: Date;
    role: UserRole;
    profilePictureUrl?: string;
    resolvedByIncidents: Incident[];
    reportedIncidents: IncidentDto[];
}

export interface LoginUserDto {
    email: string;
    password: string;
}

export interface LoginResponseDto {
    token: string | null; 
    profile:  EmployeeProfileDto | string;
}

export interface ReservationDto {
    reservationId: number;
    appUserId: string;
    showtimeId: number;
   
    totalPrice: number;
    qrCode: string;
    isValidated: boolean;
    
    numberOfSeats: number;
}
export interface MovieRatingDto {
    movieRatingId: number;
    movieId: number;
    appUserId: string;
    rating: number;
    comment?: string;
    isValidated: boolean;
  }

  export enum UserRole {
    User = 'User',
    Employee = 'Employee',
    Admin = 'Admin'
  }

  export interface UserProfileDto {
    appUserId: string;
    firstName: string;
    lastName: string;
    email: string;
    createdAt: Date;
    updatedAt: Date;
    role: UserRole;
    phoneNumber?: string;
    reservations: ReservationDto[];
    movieRatings: MovieRatingDto[];
}




