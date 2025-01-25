import { IncidentStatus } from "./enum.model";
import { TheaterDto } from "./theater.models";
import { User } from "./user.models";

export interface Incident {
    incidentId: number;
    theaterId: number;
    description: string;
    reportedById: string;
    resolvedById?: string; // Optionnel car peut être null
    status: IncidentStatus;
    reportedAt: Date;
    resolvedAt?: Date; // Optionnel car peut être null
    imageUrls: string[];
    reportedBy?: User; // Optionnel car dépend de la relation
    resolvedBy?: User; // Optionnel car dépend de la relation
    theater?: TheaterDto; // Optionnel car dépend de la relation
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
    resolvedAt?: Date;
    imageUrls: string[];
}

export interface IncidentStatusUpdateDto {
    incidentId: number;
    status: IncidentStatus;
    resolvedAt?: Date;
}
