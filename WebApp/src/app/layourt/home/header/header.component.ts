import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@app/core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  isMenuOpen = false;
  isDropdownOpen = false;
  isDropdownMobileOpen = false;
  isLoggedIn = false;
  userRole: string | null = null; 
  dropdownTitle = 'Dropdown';
  username = '';

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    // Vérifie l'état de connexion à l'initialisation
    this.isLoggedIn = this.authService.isLoggedIn();
    this.setDropdownTitle();

    // Écoute les changements de connexion en temps réel
    this.authService.authStatus.subscribe(status => {
      this.isLoggedIn = status;
      this.setDropdownTitle();
    });
  }

  setDropdownTitle() {
    const user = this.authService.getCurrentUser();
    if (user) {
      this.username = user.firstName + ' ' + user.lastName;
      switch (user.role) {
        case 'User':
          this.dropdownTitle = 'Mon Espace';
          break;
        case 'Employee':
          this.dropdownTitle = 'Intranet';
          break;
        case 'Admin':
          this.dropdownTitle = 'Administration';
          break;
        default:
          this.dropdownTitle = 'Mon Espace';
      }
    } else {
      this.dropdownTitle = 'Mon Espace';
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/home/home']);
  }
  
  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  toggleDropdownMobile() {
    this.isDropdownMobileOpen = !this.isDropdownMobileOpen;
  }

}
