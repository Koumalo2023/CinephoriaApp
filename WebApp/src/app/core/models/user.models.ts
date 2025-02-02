import { UserRole } from "./enum.model";
import { Incident, IncidentDto } from "./incident.models";
import { MovieRatingDto } from "./movie-rating.models";
import { ReservationDto } from "./reservation.models";

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

export interface ChangeEmployeePasswordDto {
    oldPassword: string;
    appUserId: string;
    newPassword: string;
}


export interface CreateEmployeeDto {
    email: string;
    password: string;
    confirmPassword: string;
    role: UserRole;
    firstName: string;
    lastName: string;
    position?: string;
    hiredDate: Date;
    phoneNumber: string;
    profilePictureUrl?: string;
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
    profile: UserProfileDto | EmployeeProfileDto | string;
}

export interface RegisterUserDto {
    email: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
}

export interface RequestPasswordResetDto {
    email: string;
}

export interface ResetPasswordDto {
    token: string;
    email: string;
    newPassword: string;
}

export interface SubmitMovieReviewDto {
    movieId: number;
    appUserId: string;
    rating: number;
    description?: string;
}

export interface UpdateAppUserDto {
    appUserId: string;
    firstName: string;
    lastName: string;
    email: string;
    userName: string;
    profilePictureUrl?: string;
    phoneNumber?: string;
}

export interface UpdateEmployeeDto {
    employeeId: string;
    email: string;
    firstName: string;
    lastName: string;
    position?: string;
    phoneNumber?: string;
    profilePictureUrl?: string;
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






