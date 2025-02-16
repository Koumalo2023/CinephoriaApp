import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { NavigationComponent } from './navigation/navigation.component';
import { NgbCollapseModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '@app/core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterOutlet, NavBarComponent, NavigationComponent, NgbCollapseModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  isSidebarHidden = false;
  constructor(private authService: AuthService) {}

  onToggleSidebar() {
    this.isSidebarHidden = !this.isSidebarHidden;
  }

  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  get isEmployee(): boolean {
    return this.authService.isEmployee();
  }
}
