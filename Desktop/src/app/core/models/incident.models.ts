
import { TheaterDto } from "./theater.models";
import { User } from "./user.models";

export interface Incident {
    incidentId: number;
    theaterId: number;
    description: string;
    reportedById: string;
    resolvedById?: string; 
    status: IncidentStatus;
    reportedAt: Date;
    resolvedAt?: Date; 
    imageUrls: string[];
    reportedBy?: User; 
    resolvedBy?: User; 
    theater?: TheaterDto;
  }
  

export interface IncidentDto {
    incidentId: number;
    description: string;
    status: IncidentStatus;
    reportedAt: Date;
    resolvedAt?: Date;
    theaterId: number;
    theaterName: string;
    reportedBy?: string;
    resolvedBy?: string;
    imageUrls: string[];
}

export interface CreateIncidentDto {
    theaterId: number;
    description: string;
    reportedBy?: string;
    imageUrls: string[];
}

export interface UpdateIncidentDto {
    incidentId: number;
    status: IncidentStatus;
    description: string;
    resolvedAt?: Date;
    imageUrls: string[];
}

export interface IncidentStatusUpdateDto {
    incidentId: number;
    status: IncidentStatus;
    resolvedAt?: Date;
}



// Enum pour les statuts des incidents
export enum IncidentStatus {
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2
}
