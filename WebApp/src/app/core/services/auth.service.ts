import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { CreateEmployeeDto, LoginUserDto, RegisterUserDto, UpdateAppUserDto, UpdateEmployeeDto, User, UserDto } from '../models/user.models';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = `${environment.apiUrl}/Auth`;


  constructor(private http: HttpClient) { }



  //   //  Inscription d'un nouvel utilisateur
  //   registerUser(registerViewModel: RegisterViewModel): Observable<GeneralServiceResponse> {
  //     return this.http.post<GeneralServiceResponse>(
  //       `${this.apiUrl}/users/register`,
  //       registerViewModel
  //     );
  //   }


  //   //  Connexion de l'utilisateur
  //   login(loginViewModel: LoginViewModel): Observable<LoginResponseViewModel | null> {
  //     return this.http.post<LoginResponseViewModel | null>(`${this.apiUrl}/login`, loginViewModel).pipe(
  //         tap(response => {
  //             if (response && response.newToken) { 
  //                 this.storeUserData(response.userInfo, response.newToken);
  //             }
  //         })
  //     );
  // }

  


  /**
   * Crée un compte employé ou administrateur avec un mot de passe temporaire.
   * @param createEmployeeDto Les informations de création pour l'employé ou l'administrateur.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  registerEmployee(createEmployeeDto: CreateEmployeeDto): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/register-employee`, createEmployeeDto);
  }

  /**
   * Connecte un utilisateur en vérifiant ses informations d'identification.
   * @param loginUserDto Les informations de connexion de l'utilisateur.
   * @returns Observable contenant un jeton JWT et les informations de profil de l'utilisateur.
   */
  login(loginUserDto: LoginUserDto): Observable<{ Token: string; Profile: UserDto }> {
    return this.http.post<{ Token: string; Profile: UserDto }>(`${this.apiUrl}/login`, loginUserDto);
  }

  /**
   * Télécharge une image de profil pour un utilisateur spécifique.
   * @param userId L'identifiant de l'utilisateur.
   * @param file Le fichier image à télécharger.
   * @returns Observable contenant l'URL de l'image téléchargée.
   */
  uploadUserProfile(userId: string, file: File): Observable<{ Message: string; Url: string }> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<{ Message: string; Url: string }>(
      `${this.apiUrl}/upload-user-profile/${userId}`,
      formData
    );
  }


  /**
   * Supprime l'image de profil d'un utilisateur spécifique.
   * @param userId L'identifiant de l'utilisateur.
   * @param imageUrl L'URL de l'image à supprimer.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  deleteUserProfileImage(userId: string, imageUrl: string): Observable<{ Message: string }> {
    return this.http.delete<{ Message: string }>(`${this.apiUrl}/delete-user-profile-image/${userId}`, {
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
   * @param userId L'identifiant de l'utilisateur.
   * @returns Observable contenant les informations de l'utilisateur.
   */
  getUserById(userId: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/users/${userId}`);
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
   * @param userId L'identifiant de l'utilisateur.
   * @returns Observable contenant le profil de l'utilisateur.
   */
  getUserProfile(userId: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/user-profile/${userId}`);
  }

  /**
   * Met à jour le profil d'un utilisateur spécifique.
   * @param userId L'identifiant de l'utilisateur.
   * @param updateAppUserDto Les nouvelles données du profil utilisateur.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  updateUserProfile(userId: string, updateAppUserDto: UpdateAppUserDto): Observable<{ Message: string }> {
    return this.http.put<{ Message: string }>(`${this.apiUrl}/update-profile/${userId}`, updateAppUserDto);
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
   * @param userId L'identifiant de l'utilisateur.
   * @param token Le jeton de confirmation.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  confirmEmail(userId: string, token: string): Observable<{ Message: string }> {
    return this.http.get<{ Message: string }>(`${this.apiUrl}/confirm-email`, {
      params: { userId, token }
    });
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
   * @param userId L'identifiant de l'utilisateur.
   * @param token Le jeton de réinitialisation.
   * @returns Observable contenant le message de validation.
   */
  validateResetToken(userId: string, token: string): Observable<{ Message: string }> {
    return this.http.get<{ Message: string }>(`${this.apiUrl}/validate-reset-token`, {
      params: { userId, token }
    });
  }

  /**
   * Force la réinitialisation du mot de passe d'un utilisateur.
   * @param userId L'identifiant de l'utilisateur.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  forcePasswordReset(userId: string): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/force-password-reset`, null, {
      params: { userId }
    });
  }

  /**
   * Permet à un employé de changer son mot de passe après avoir utilisé un mot de passe temporaire.
   * @param changePasswordDto Les informations de changement de mot de passe.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  changeEmployeePassword(changePasswordDto: { oldPassword: string; newPassword: string }): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/change-password`, changePasswordDto);
  }

  /**
   * Force un employé à changer son mot de passe (ex. mot de passe temporaire expiré).
   * @param userId L'identifiant de l'employé.
   * @returns Observable contenant le message de réussite ou d'échec.
   */
  forceEmployeePasswordChange(userId: string): Observable<{ Message: string }> {
    return this.http.post<{ Message: string }>(`${this.apiUrl}/force-password-change`, null, {
      params: { userId }
    });
  }


  // Fonction pour obtenir l'utilisateur connecté
  getCurrentUser(): User | null {
    if (typeof window !== 'undefined' && localStorage) {
      const userJson = localStorage.getItem('user');
      return userJson ? JSON.parse(userJson) : null;
    }
    return null;
  }

  // Fonction pour sauvegarder l'utilisateur et le token dans le localStorage
  private storeUserData(user: User, token: string): void {
    if (typeof window !== 'undefined' && localStorage) {
      console.log("Utilisateur connecté :", token);
      localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(user));
    }
  }

  getToken(): string | null {
    if (typeof window !== 'undefined' && localStorage) {
      return localStorage.getItem('token');
    }
    return null;
  }

  // Fonction pour se déconnecter
  logout(): void {
    if (typeof window !== 'undefined' && localStorage) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      localStorage.removeItem('authToken');
    }
  }

  // Fonction pour vérifier si un utilisateur est connecté
  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  // Vérifier si l'utilisateur est un administrateur
  isAdmin(): boolean {
    const user = this.getCurrentUser();
    return user ? user.role.includes('Admin') : false;
  }

  // Vérifier si l'utilisateur est un employé
  isEmployee(): boolean {
    const user = this.getCurrentUser();
    return user ? user.role.includes('Employee') : false;
  }

  // Vérifier si l'utilisateur est un utilisateur
  isUser(): boolean {
    const user = this.getCurrentUser();
    return user ? user.role.includes('User') : false;
  }

}

