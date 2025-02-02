import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { AlertService } from '../../../core/services/alert.service';
import { CommonModule } from '@angular/common';
import { EmployeeProfileDto, UserProfileDto } from '@app/core/models/user.models';
import { RedirectService } from '@app/core/services/redirect.service';
import { app } from 'server';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterModule, CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  loading = false;
  errorMessage = '';
  showPassword: boolean = false;
  showPasswordCriteria: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private alertService: AlertService,
    private redirectService: RedirectService
  ) {
    this.loginForm = this.fb.group({
      userName: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }
  
    this.loading = true;
    const loginData = {
      email: this.loginForm.value.userName,
      password: this.loginForm.value.password,
    };
  
    this.authService.login(loginData).subscribe({
      next: (response) => {
        if (response.token) {
          this.alertService.showAlert('Connexion réussie !', 'success');
  
          // Vérifier le type de `response.profile`
          if (typeof response.profile === 'string') {
            try {
              // Parser la chaîne en objet
              const parsedProfile = JSON.parse(response.profile);
              this.handleProfile(parsedProfile);
            } catch (error) {
              console.error('Erreur lors de l\'analyse du profil :', error);
              this.errorMessage = 'Erreur lors de la connexion.';
            }
          } else {
            // Si `response.profile` est déjà un objet
            this.handleProfile(response.profile);
          }
  
          // Rediriger en fonction du rôle ou de l'URL de redirection
          // const redirectUrl = this.redirectService.getRedirectUrl();
          // if (redirectUrl) {
          //   this.router.navigateByUrl(redirectUrl);
          //   this.redirectService.clearRedirectUrl();
          // }
        } else {
          this.errorMessage = 'Connexion échouée. Vérifiez vos informations.';
        }
      },  
      error: (error) => {
        console.error("Erreur lors de la connexion :", error);
        this.errorMessage = 'Erreur lors de la connexion : ' + (error?.error?.message || 'Erreur inconnue');
      },
      complete: () => {
        this.loading = false;
      },
    });
  }

  private handleProfile(profile: UserProfileDto | EmployeeProfileDto): void {
    const userRole = profile.role;
    const employeeId = ('userId' in profile ? profile.userId : profile) as string;
    this.redirectBasedOnRole(userRole, employeeId);
  }

  private redirectBasedOnRole(role: string, userId: string): void {
    switch (role) {
      case 'Admin':
        this.router.navigate(['/home/home']);
        break;
      case 'Employee':
      case 'User':
        this.router.navigate(['/admin/dashboard/', userId]);
        break;
      default:
        this.errorMessage = 'Rôle utilisateur non reconnu.';
        console.error('Rôle non reconnu :', role);
        break;
    }
  }
  
  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
  // Méthode pour simplifier l'accès aux erreurs de validation dans le HTML
  getErrorMessage(controlName: string, errorName: string): boolean {
    const control = this.loginForm.get(controlName);
    return control?.hasError(errorName) && (control.dirty || control.touched) || false;
  }  
}
