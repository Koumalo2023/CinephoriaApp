import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ResetPasswordDto } from '@app/core/models/user.models';
import { AuthService } from '@app/core/services/auth.service';

@Component({
  selector: 'app-resset-password',
  standalone: true,
  imports: [RouterModule, CommonModule, ReactiveFormsModule],
  templateUrl: './resset-password.component.html',
  styleUrl: './resset-password.component.scss'
})
export class RessetPasswordComponent implements OnInit {
  resetPasswordForm!: FormGroup;
  message: string = '';
  isSuccess: boolean = false;
  isLoading: boolean = false;
  token: string = ''; // Token récupéré depuis l'URL
  email: string = ''; // Email récupéré depuis l'URL

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private route: ActivatedRoute 
  ) {}

  ngOnInit(): void {
    // Récupérer le token et l'email depuis l'URL
    this.token = this.route.snapshot.queryParams['token'];
    this.email = this.route.snapshot.queryParams['email'];

    // Initialiser le formulaire
    this.resetPasswordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validator: this.passwordMatchValidator });
  }

  // Validateur personnalisé pour vérifier que les mots de passe correspondent
  passwordMatchValidator(formGroup: FormGroup) {
    const newPassword = formGroup.get('newPassword')?.value;
    const confirmPassword = formGroup.get('confirmPassword')?.value;
    return newPassword === confirmPassword ? null : { mismatch: true };
  }

  onSubmit() {
    if (this.resetPasswordForm.invalid) {
      return;
    }

    this.isLoading = true;

    // Créer l'objet ResetPasswordDto
    const resetPasswordDto: ResetPasswordDto = {
      token: this.token,
      email: this.email,
      newPassword: this.resetPasswordForm.value.newPassword
    };

    // Appel de la méthode du service
    this.authService.resetPassword(resetPasswordDto).subscribe({
      next: (response) => {
        this.message = response.Message; // Affichez le message de réussite
        this.isSuccess = true;
        this.isLoading = false;
      },
      error: (err) => {
        this.message = 'Une erreur est survenue. Veuillez réessayer plus tard.'; // Message d'erreur générique
        this.isSuccess = false;
        this.isLoading = false;
        console.error(err);
      }
    });
  }
}
