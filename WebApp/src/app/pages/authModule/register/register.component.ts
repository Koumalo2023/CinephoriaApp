import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';;
import { CommonModule } from '@angular/common';
import { AlertService } from '../../../core/services/alert.service';
import { tap } from 'rxjs';
import { LoadingService } from '../../../core/services/loading.service';
import { RegisterUserDto } from '@app/core/models/user.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterModule, CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  showPassword: boolean = false;
  showPasswordCriteria: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private alertService: AlertService,
    private loadingService: LoadingService,
    private router: Router
  ) {
    
  }

  ngOnInit(): void {
    // Écoute les changements de valeur du formulaire
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]]
    }, { validator: this.passwordMatchValidator });
  }

  // Vérifie si un champ a une erreur

  passwordMatchValidator(formGroup: FormGroup) {
    const passwordControl = formGroup.get('password');
    const confirmPasswordControl = formGroup.get('confirmPassword');
  
    if (!passwordControl || !confirmPasswordControl) {
      return null; // Retourne null si l'un des contrôles n'existe pas
    }
  
    const password = passwordControl.value;
    const confirmPassword = confirmPasswordControl.value;
  
    return password === confirmPassword ? null : { mismatch: true };
  }


  // Soumission du formulaire
  
  onSubmit() {
    if (this.registerForm.valid) {
      const registerData = this.registerForm.value;
      this.authService.registerUser(registerData).subscribe({
        next: () => {
          this.loadingService.hide();
          this.alertService.showAlert('Inscription réussie !', 'success');
          this.router.navigate(['/auth/login']);
        },
        error: (error) => {
          this.loadingService.hide();
          const errorMessage = error?.error?.message || 'Une erreur est survenue lors de l\'inscription.';
          this.alertService.showAlert(errorMessage, 'danger');
        }
      });
    } else {
      console.log('Form is not valid');
    }
  }


  // Affiche ou masque le mot de passe
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  // Vérifie si un champ a une erreur spécifique
  hasError(controlName: string, errorName: string): boolean {
    const control = this.registerForm.get(controlName);
    return !!control && control.hasError(errorName) && (control.dirty || control.touched);
  }
}