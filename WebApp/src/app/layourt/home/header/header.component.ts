import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@app/core/services/auth.service';
import { AlertService } from '@app/core/services/alert.service';
import { User } from '@app/core/models/user.models';

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
  myUser!:User | null;
  userRole: string | null = null; 
  

  constructor(private authService: AuthService, private router: Router, private alertService: AlertService) {}

  ngOnInit() {
    this.myUser = this.authService.getCurrentUser();
    // Vérifie l'état de connexion à l'initialisation
    this.isLoggedIn = this.authService.isLoggedIn();
    this.userRole = this.myUser?.role || null;
    
    // Écoute les changements de connexion en temps réel
    this.authService.authStatus.subscribe(status => {
      this.isLoggedIn = status;
      this.userRole = this.authService.getCurrentUser()?.role || null;
    });
    // Écoute les changements de connexion en temps réel
    this.authService.authStatus.subscribe(status => {
      this.isLoggedIn = status;
     
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/home/home']);
    this.alertService.showAlert('Vous avez été déconnecté avec succès.', 'success');
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
