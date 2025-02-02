import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { AlertService } from '../services/alert.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    private alertService: AlertService
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRoles: string[] = route.data['expectedRole']?.split(',') || [];
    const currentUser = this.authService.getCurrentUser();

    // Vérifie si l'utilisateur est connecté
    if (!currentUser) {
      this.alertService.showAlert('Vous devez vous connecter pour accéder à cette page.', 'warning');
      this.router.navigate(['/auth/login']);
      return false;
    }

    // Vérifie si le rôle de l'utilisateur correspond au rôle attendu
    const userRole: string = currentUser.role;

    if (!expectedRoles.includes(userRole)) {
      this.alertService.showAlert('Vous n\'avez pas les autorisations nécessaires pour accéder à cette page.', 'danger');
      this.router.navigate(['/unauthorized']);
      return false;
    }

    return true;
  }
}