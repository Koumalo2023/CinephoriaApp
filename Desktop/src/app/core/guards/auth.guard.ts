import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
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

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const user = this.authService.getCurrentUser();
    const requiredRole = route.data['role'] || null;

    if (!user) {
      this.router.navigate(['/home/home']);
      return false;
    }

    if (requiredRole && user.role !== requiredRole) {
      this.router.navigate(['/home/home']);
      return false;
    }

    return true;
  }
}