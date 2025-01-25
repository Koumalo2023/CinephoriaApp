import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRoles: string[] = route.data['expectedRole']?.split(',') || [];
    const currentUser = this.authService.getCurrentUser();

    // Vérifie si l'utilisateur est connecté
    if (!currentUser) {
      this.router.navigate(['/login']); // Redirige vers la page de connexion
      return false;
    }

    // Vérifie si le rôle de l'utilisateur correspond au rôle attendu
    const userRole: string = currentUser.role; 

    if (!expectedRoles.includes(userRole)) {
      this.router.navigate(['/unauthorized']); 
      return false;
    }

    return true; // Autorise l'accès si tout est valide
  }
}
