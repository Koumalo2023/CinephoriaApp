import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ContactRequest, CreateEmployeeDto, EmployeeProfileDto, LoginResponseDto, LoginUserDto, RegisterUserDto, UpdateAppUserDto, UpdateEmployeeDto, User, UserDto, UserProfileDto } from '../models/user.models';
import { BehaviorSubject, catchError, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { ApiConfigService } from './apiConfigService.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authStatusSubject = new BehaviorSubject<boolean>(this.isLoggedIn());
  public authStatus = this.authStatusSubject.asObservable();

  private currentUserSubject = new BehaviorSubject<User | null>(this.getCurrentUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  private apiUrl: string;

  constructor(private http: HttpClient, private router: Router, private apiConfigService: ApiConfigService) {
    this.apiUrl = this.apiConfigService.getAuthUrl();
  }

  
  registerEmployee(createEmployeeDto: CreateEmployeeDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/register-employee`, createEmployeeDto);
  }

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
   * Enregistre un nouvel utilisateur.
   * @param registerUserDto Les données de l'utilisateur à enregistrer.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  registerUser(registerUserDto: RegisterUserDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/register`, registerUserDto);
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
   * Récupère le profil d'un employé spécifique.
   * @param employeeId L'identifiant de l'employé.
   * @returns Observable contenant le profil de l'employé.
   */
  getEmployeeProfile(employeeId: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/employee-profile/${employeeId}`);
  }

  /**
   * Récupère le profil d'un utilisateur spécifique.
   * @param appUserId L'identifiant de l'utilisateur.
   * @returns Observable contenant le profil de l'utilisateur.
   */
  getUserProfile(appUserId: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/user-profile/${appUserId}`);
  }

  /**
   * Met à jour le profil d'un utilisateur spécifique.
   * @param appUserId L'identifiant de l'utilisateur.
   * @param updateAppUserDto Les nouvelles données du profil utilisateur.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  updateUserProfile(appUserId: string, updateAppUserDto: UpdateAppUserDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}/update-profile/${appUserId}`, updateAppUserDto);
  }

  /**
   * Met à jour le profil d'un employé spécifique.
   * @param employeeId L'identifiant de l'employé.
   * @param updateEmployeeDto Les nouvelles données du profil employé.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  updateEmployeeProfile(employeeId: string, updateEmployeeDto: UpdateEmployeeDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(
      `${this.apiUrl}/update-employee-profile/${employeeId}`,
      updateEmployeeDto
    );
  }

  /**
   * Confirme l'adresse email d'un utilisateur.
   * @param appUserId L'identifiant de l'utilisateur.
   * @param token Le jeton de confirmation.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  confirmEmail(appUserId: string, token: string): Observable<{ Message: string }> {
    return this.http.get<{ Message: string }>(`${this.apiUrl}/confirm-email`, {
      params: { appUserId, token }
    });
  }

  /**
   * Envoie un email de contact.
   * @param request Les données de la demande de contact.
   * @returns Observable contenant la réponse du serveur.
   */
  sendContactEmail(request: ContactRequest): Observable<any> {
    console.log('Requête envoyée :', request);
    return this.http.post(`${this.apiUrl}/send-contact`, request);
  }

  /**
   * Demande de réinitialisation de mot de passe pour un utilisateur normal.
   * @param email L'adresse email de l'utilisateur.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  forgotPassword(email: string): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/forgot-password`, { email });
  }

  /**
   * Réinitialise le mot de passe d'un utilisateur normal.
   * @param resetPasswordDto Les informations pour réinitialiser le mot de passe.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  resetPassword(resetPasswordDto: { token: string; newPassword: string }): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/reset-password`, resetPasswordDto);
  }

  /**
   * Valide un jeton de réinitialisation de mot de passe.
   * @param appUserId L'identifiant de l'utilisateur.
   * @param token Le jeton de réinitialisation.
   * @returns Observable contenant le message de validation.
   */
  validateResetToken(appUserId: string, token: string): Observable<{ Message: string }> {
    return this.http.get<{ Message: string }>(`${this.apiUrl}/validate-reset-token`, {
      params: { appUserId, token }
    });
  }

  /**
   * Permet à un utilisateur connecté de changer son mot de passe.
   * @param appUserId L'identifiant de l'utilisateur.
   * @param changePasswordDto Les informations de changement de mot de passe.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  updateUserPassword(appUserId: string, changePasswordDto: { oldPassword: string; newPassword: string; confirmNewPassword: string }): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/change-password/${appUserId}`, changePasswordDto);
  }

  /**
   * Permet à un employé de changer son mot de passe après avoir utilisé un mot de passe temporaire.
   * @param changePasswordDto Les informations de changement de mot de passe.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  changeEmployeePassword(changePasswordDto: { oldPassword: string; newPassword: string; confirmNewPassword: string; appUserId: string }): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/change-password`, changePasswordDto);
  }

  /**
   * Force un employé à changer son mot de passe (ex. mot de passe temporaire expiré).
   * @param appUserId L'identifiant de l'employé.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  forceEmployeePasswordChange(appUserId: string): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/force-password-change`, null, {
      params: { appUserId }
    });
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
      userName: `${profile.firstName}.${profile.lastName}`, // Générer un nom d'utilisateur
      emailConfirmed: true, // Supposons que l'email est confirmé
      phoneNumber: profile.phoneNumber || '', // Utiliser la valeur fournie ou une chaîne vide
      createdAt: profile.createdAt,
      updatedAt: profile.updatedAt,
      hasApprovedTermsOfUse: true, // Supposons que les termes sont approuvés
      hiredDate: undefined, // Non fourni dans UserProfileDto
      position: undefined, // Non fourni dans UserProfileDto
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

