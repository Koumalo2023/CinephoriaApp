import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { EmployeeProfileDto, LoginResponseDto, LoginUserDto,  User, UserDto, UserProfileDto,  } from '../models/user.models';
import { BehaviorSubject,  Observable, tap } from 'rxjs';
import { Router } from '@angular/router';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authStatusSubject = new BehaviorSubject<boolean>(this.isLoggedIn());
  public authStatus = this.authStatusSubject.asObservable();

  private currentUserSubject = new BehaviorSubject<User | null>(this.getCurrentUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  private apiUrl = `${environment.apiUrl}/Auth`;


  constructor(private http: HttpClient, private router: Router) { }

  

  /**
   * Connecte un utilisateur en vérifiant ses informations d'identification.
   * @param loginUserDto Les informations de connexion de l'utilisateur.
   * @returns Observable contenant un jeton JWT et les informations de profil de l'utilisateur.
   */
  login(loginUserDto: LoginUserDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(`${this.apiUrl}/login`, loginUserDto).pipe(
      tap(response => {
        if (response && response.token) {
          try {
            const user = this.mapProfileToUser(response.profile);
            this.storeUserData(user, response.token);
            this.updateAuthStatus(true);
          } catch (error) {
            console.error('Erreur lors de la conversion du profil en User :', error);
          }
        }
      })
    );
  }
  

  /**
   * Télécharge une image de profil pour un utilisateur spécifique.
   * @param appUserId L'identifiant de l'utilisateur.
   * @param file Le fichier image à télécharger.
   * @returns Observable contenant l'URL de l'image téléchargée.
   */
  uploadUserProfile(appUserId: string, file: File): Observable<{ Message: string; Url: string }> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<{ Message: string; Url: string }>(
      `${this.apiUrl}/upload-user-profile/${appUserId}`,
      formData
    );
  }


  /**
   * Supprime l'image de profil d'un utilisateur spécifique.
   * @param appUserId L'identifiant de l'utilisateur.
   * @param imageUrl L'URL de l'image à supprimer.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  deleteUserProfileImage(appUserId: string, imageUrl: string): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/delete-user-profile-image/${appUserId}`, {
      params: { imageUrl }
    });
  }


  /**
   * Récupère la liste de tous les utilisateurs enregistrés.
   * @returns Observable contenant une liste des utilisateurs.
   */
  getAllUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.apiUrl}/users`);
  }

  /**
   * Récupère un utilisateur spécifique par son identifiant.
   * @param appUserId L'identifiant de l'utilisateur.
   * @returns Observable contenant les informations de l'utilisateur.
   */
  getUserById(appUserId: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/users/${appUserId}`);
  }


  /**
   * Obtient l'utilisateur connecté depuis le localStorage.
   */
  getCurrentUser(): User | null {
    if (typeof window !== 'undefined' && localStorage) {
      const userJson = localStorage.getItem('user');
      return userJson ? JSON.parse(userJson) : null;
    }
    return null;
  }

  /**
   * Sauvegarde l'utilisateur et le token dans le localStorage.
   */
  private storeUserData(user: User, token: string): void {
    if (typeof window !== 'undefined' && localStorage) {
      localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(user));
      this.currentUserSubject.next(user); // Notifier les changements
    }
  }

  /**
   * Récupère le token JWT depuis le localStorage.
   */
  getToken(): string | null {
    if (typeof window !== 'undefined' && localStorage) {
      return localStorage.getItem('token');
    }
    return null;
  }

  /**
   * Déconnecte l'utilisateur en supprimant les données du localStorage.
   */
  logout(): void {
    if (typeof window !== 'undefined' && localStorage) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      this.updateAuthStatus(false); // Mettre à jour l'état de connexion
      this.currentUserSubject.next(null); // Réinitialiser l'utilisateur
      this.router.navigate(['/login']);
    }
  }

  /**
   * Vérifie si un utilisateur est connecté.
   */
  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  /**
   * Vérifie si l'utilisateur est un administrateur.
   */
  isAdmin(): boolean {
    const user = this.getCurrentUser();
    return user ? user.role.includes('Admin') : false;
  }

  /**
   * Vérifie si l'utilisateur est un employé.
   */
  isEmployee(): boolean {
    const user = this.getCurrentUser();
    return user ? user.role.includes('Employee') : false;
  }

  /**
   * Vérifie si l'utilisateur est un utilisateur standard.
   */
  isUser(): boolean {
    const user = this.getCurrentUser();
    return user ? user.role.includes('User') : false;
  }

  /**
   * Met à jour l'état de connexion via le sujet authStatusSubject.
   */
  private updateAuthStatus(status: boolean): void {
    this.authStatusSubject.next(status);
  }

  
  private mapEmployeeProfileDtoToUser(profile: EmployeeProfileDto): User {
    return {
      appUserId: profile.employeeId,
      firstName: profile.firstName,
      lastName: profile.lastName,
      email: profile.email,
      userName: `${profile.firstName}.${profile.lastName}`, 
      emailConfirmed: true, 
      phoneNumber: '', 
      createdAt: profile.createdAt,
      updatedAt: profile.updatedAt,
      hasApprovedTermsOfUse: true, // Supposons que les termes sont approuvés
      hiredDate: profile.hiredDate,
      position: profile.position,
      profilePictureUrl: profile.profilePictureUrl,
      reportedIncidents: profile.reportedIncidents,
      resolvedByIncidents: profile.resolvedByIncidents,
      role: profile.role,
      reservations: [],
      movieRatings: [], 
    };
  }

  private mapUserProfileDtoToUser(profile: UserProfileDto): User {
  return {
    appUserId: profile.appUserId,
    firstName: profile.firstName,
    lastName: profile.lastName,
    email: profile.email,
    userName: `${profile.firstName}.${profile.lastName}`, 
    emailConfirmed: true, 
    phoneNumber: profile.phoneNumber || '', 
    createdAt: profile.createdAt,
    updatedAt: profile.updatedAt,
    hasApprovedTermsOfUse: true, 
    hiredDate: undefined, 
    position: undefined, 
    profilePictureUrl: undefined, // Non fourni dans UserProfileDto
    reportedIncidents: [], // Non fourni dans UserProfileDto
    resolvedByIncidents: [], // Non fourni dans UserProfileDto
    role: profile.role,
    reservations: profile.reservations,
    movieRatings: profile.movieRatings,
  };
}

/**
   * Mappe les données de profil reçues depuis le serveur vers l'interface User.
   */
  
private mapProfileToUser(profile: UserProfileDto | EmployeeProfileDto | string): User {
  if (typeof profile === 'string') {
    try {
      return JSON.parse(profile) as User;
    } catch (error) {
      throw new Error('Le profil ne peut pas être converti en User.');
    }
  }

  if ('reservations' in profile && 'movieRatings' in profile) {
    return this.mapUserProfileDtoToUser(profile as UserProfileDto);
  }

  if ('employeeId' in profile) {
    return this.mapEmployeeProfileDtoToUser(profile);
  }

  throw new Error('Le type de profil n\'est pas reconnu.');
}
}

