import { Injectable } from "@angular/core";


@Injectable({
    providedIn: 'root'
})
export class EnumService {
    // Définition des enums
    public static readonly MovieGenre = {
        Action: 'Action',
        Aventure: 'Aventure',
        Comedie: 'Comedie',
        Animation: 'Animation',
        Crime: 'Crime',
        Documentaire: 'Documentaire',
        Fantastique: 'Fantastique',
        Guerre: 'Guerre',
        Horreur: 'Horreur',
        Western: 'Western',
        Romance: 'Romance',
        Familiale: 'Famille',
        Thriller: 'Thriller',
        Mystère: 'Mystère'
    } as const;

    public static readonly UserRole = {
        User: 'User',
        Employee: 'Employee',
        Admin: 'Admin'
    } as const;


    // Enum pour les statuts des réservations
    public static readonly ReservationStatus = {
        Pending: 'En attente',
        Confirmed: 'Confirmée',
        Cancelled: 'Annulée'
    } as const;

    // Enum pour les qualités de projection
    public static readonly ProjectionQuality = {
        FourDX: '4DX',
        ThreeD: '3D',
        IMAX: 'IMAX',
        FourK: '4K'
    } as const;

    // Enum pour les statuts des incidents
    public static readonly IncidentStatus = {
        Pending: 'En attente',
        Confirmed: 'Confirmée',
        Cancelled: 'Annulée'
    } as const;

    /**
     * Méthode générique pour obtenir la valeur d'un enum par sa clé.
     * @param enumObj L'objet enum
     * @param key La clé de l'enum
     * @returns La valeur correspondante ou "Inconnu" si non trouvée
     */
    public static getEnumValue<T>(enumObj: Record<string, T>, key: string): T | 'Inconnu' {
        return enumObj[key as keyof typeof enumObj] ?? 'Inconnu';
    }

    /**
     * Méthode générique pour obtenir toutes les valeurs d'un enum.
     * @param enumObj L'objet enum
     * @returns Un tableau de valeurs
     */
    public static getEnumValues<T>(enumObj: Record<string, T>): T[] {
        return Object.values(enumObj).filter(value => typeof value === 'string') as T[];
    }
}