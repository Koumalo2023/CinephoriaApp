import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { RequestPasswordResetDto } from '@app/core/models/user.models';
import { AlertService } from '@app/core/services/alert.service';
import { AuthService } from '@app/core/services/auth.service';
import { LoadingService } from '@app/core/services/loading.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [RouterModule, CommonModule, ReactiveFormsModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss'
})
export class ForgotPasswordComponent implements OnInit {
  requestPasswordResetForm!: FormGroup;
  message: string = '';
  isSuccess: boolean = false;
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private alertService: AlertService,
    private loadingService: LoadingService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.requestPasswordResetForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit() {
    if (this.requestPasswordResetForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.loadingService.show();
    const email = this.requestPasswordResetForm.value.email;

    // Appel de la méthode du service pour demander la réinitialisation
    this.authService.forgotPassword(email).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.loadingService.hide();
        this.message = response.Message;
        this.alertService.showAlert('Un email de réinitialisation a été envoyé.', 'success');
        this.router.navigate(['/auth/reset-password'], { queryParams: { email } }); // Redirige vers la page de réinitialisation
      },
      error: (err) => {
        this.isLoading = false;
        this.loadingService.hide();
        this.alertService.showAlert('Une erreur est survenue lors de l\'envoi de l\'email.', 'danger');
        console.error(err);
      }
    });
  }
}
