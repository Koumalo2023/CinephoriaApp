import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ResetPasswordDto } from '@app/core/models/user.models';
import { AlertService } from '@app/core/services/alert.service';
import { AuthService } from '@app/core/services/auth.service';
import { LoadingService } from '@app/core/services/loading.service';

@Component({
  selector: 'app-resset-password',
  standalone: true,
  imports: [RouterModule, CommonModule, ReactiveFormsModule],
  templateUrl: './resset-password.component.html',
  styleUrl: './resset-password.component.scss'
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm!: FormGroup;
  message: string = '';
  isSuccess: boolean = false;
  isLoading: boolean = false;
  token: string = '';
  email: string = '';
  tokenInvalid: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private alertService: AlertService,
    private loadingService: LoadingService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Récupère le token et l'email depuis l'URL
    this.token = this.route.snapshot.queryParams['token'];
    this.email = this.route.snapshot.queryParams['email'];

    // Valide le token avant d'afficher le formulaire
    this.validateResetToken();

    // Initialise le formulaire de réinitialisation
    this.resetPasswordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    });
  }

  // Valide le token de réinitialisation
  validateResetToken() {
    this.isLoading = true;
    this.authService.validateResetToken(this.email, this.token).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.message = response.Message;
        this.alertService.showAlert('Token valide. Vous pouvez réinitialiser votre mot de passe.', 'success');
      },
      error: (err) => {
        this.isLoading = false;
        this.tokenInvalid = true; 
        this.alertService.showAlert('Le token est invalide ou a expiré.', 'danger');
        console.error(err);
      }
    });
  }

  // Soumet le nouveau mot de passe
  onSubmit() {
    if (this.resetPasswordForm.invalid) {
      return;
    }

    const newPassword = this.resetPasswordForm.value.newPassword;
    const confirmPassword = this.resetPasswordForm.value.confirmPassword;

    if (newPassword !== confirmPassword) {
      this.alertService.showAlert('Les mots de passe ne correspondent pas.', 'danger');
      return;
    }

    this.isLoading = true;
    this.loadingService.show();

    const resetPasswordDto = {
      token: this.token,
      newPassword: newPassword
    };

    // Appel de la méthode du service pour réinitialiser le mot de passe
    this.authService.resetPassword(resetPasswordDto).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.loadingService.hide();
        this.message = response.Message;
        this.alertService.showAlert('Mot de passe réinitialisé avec succès.', 'success');
        this.router.navigate(['/auth/login']); 
      },
      error: (err) => {
        this.isLoading = false;
        this.loadingService.hide();
        this.alertService.showAlert('Une erreur est survenue lors de la réinitialisation.', 'danger');
        console.error(err);
      }
    });
  }
}
