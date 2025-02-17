import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { NAVIGATION_ITEMS, NavigationItem } from './navigation';
import { AlertService } from '@app/core/services/alert.service';
import { AuthService } from '@app/core/services/auth.service';

@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.scss'
})
export class NavigationComponent {
  @Input() isSidebarHidden = false;
  userRole: 'admin' | 'employee' = 'employee';
  constructor(
    private router: Router,
    private authService: AuthService, 
    private alertService: AlertService
  ) {}


  get navigationItems(): NavigationItem[] {
    return NAVIGATION_ITEMS.filter(item => item.roles.includes(this.userRole));
  }

  toggleSubnav(item: NavigationItem): void {
    if (item.children) {
      item.isOpen = !item.isOpen;
    }
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/home/home']);
    this.alertService.showAlert('Vous avez été déconnecté avec succès.', 'success');
  }

}
