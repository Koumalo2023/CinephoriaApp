import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { AlertService } from '../../../core/services/alert.service';
import { CommonModule } from '@angular/common';
import { LoginUserDto,  } from '@app/core/models/user.models';

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
  
    const loginData: LoginUserDto = {
      email: this.loginForm.value.userName,
      password: this.loginForm.value.password,
    };
  
    // Appel de la méthode login du service AuthService
    this.authService.login(loginData).subscribe({
      next: (response) => {
        if (response.token) {
          this.alertService.showAlert('Connexion réussie !', 'success');
  
          this.router.navigate(['/admin/incident-list']);
        } else {
          this.errorMessage = 'Connexion échouée. Vérifiez vos informations.';
        }
      },
      error: (error) => {
        console.error('Erreur lors de la connexion :', error);
        this.errorMessage = 'Erreur lors de la connexion : ' + (error?.error?.message || 'Erreur inconnue');
      },
      complete: () => {
        this.loading = false;
      },
    });
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
