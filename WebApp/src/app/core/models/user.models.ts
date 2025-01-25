import { UserRole } from "./enum.model";
import { Incident, IncidentDto } from "./incident.models";
import { MovieRatingDto } from "./movie-rating.models";
import { ReservationDto } from "./reservation.models";

export interface User {
    id: string;
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
    id: string;
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
    userId: string;
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
    profilePictureUrl?: string;
    resolvedByIncidents: IncidentDto[];
    reportedIncidents: IncidentDto[];
}

export interface LoginUserDto {
    email: string;
    password: string;
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
    userId: string;
    rating: number;
    description?: string;
}

export interface UpdateAppUserDto {
    id: string;
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
    userId: string;
    firstName: string;
    lastName: string;
    email: string;
    createdAt: Date;
    updatedAt: Date;
    phoneNumber?: string;
    reservations: ReservationDto[];
    movieRatings: MovieRatingDto[];
}




