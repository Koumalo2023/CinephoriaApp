import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertService } from '@app/core/services/alert.service';
import { AuthService } from '@app/core/services/auth.service';
import { LoadingService } from '@app/core/services/loading.service';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.scss'
})
export class ConfirmEmailComponent implements OnInit {
  isLoading: boolean = false; // Indique si la validation est en cours
  isConfirmed: boolean = false; // Indique si l'email a été confirmé avec succès
  errorMessage: string = ''; // Message d'erreur en cas d'échec

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private alertService: AlertService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    // Récupère les paramètres de l'URL (userId et token)
    const userId = this.route.snapshot.queryParams['userId'];
    const token = this.route.snapshot.queryParams['token'];

    // Valide le jeton de confirmation
    if (userId && token) {
      this.confirmEmail(userId, token);
    } else {
      this.errorMessage = 'Le lien de confirmation est invalide.';
      this.alertService.showAlert(this.errorMessage, 'danger');
      this.router.navigate(['/auth/login']); // Redirige vers la page de connexion
    }
  }

  // Méthode pour confirmer l'email
  confirmEmail(userId: string, token: string) {
    this.isLoading = true;
    this.loadingService.show();

    this.authService.confirmEmail(userId, token).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.loadingService.hide();
        this.isConfirmed = true;
        this.alertService.showAlert('Votre email a été confirmé avec succès.', 'success');
        setTimeout(() => {
          this.router.navigate(['/auth/login']); // Redirige vers la page de connexion après 3 secondes
        }, 3000);
      },
      error: (err) => {
        this.isLoading = false;
        this.loadingService.hide();
        this.errorMessage = 'Le lien de confirmation est invalide ou a expiré.';
        this.alertService.showAlert(this.errorMessage, 'danger');
        console.error(err);
      }
    });
  }
}
