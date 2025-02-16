import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, OnInit, Output } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { User } from '@app/core/models/user.models';
import { AlertService } from '@app/core/services/alert.service';
import { AuthService } from '@app/core/services/auth.service';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [CommonModule, NgbDropdownModule, RouterModule],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss'
})
export class NavBarComponent  {
  @Output() toggleSidebar = new EventEmitter<void>();
  isMenuOpen: boolean = false;
  isLoggedIn = false;
  myUser!:User | null;
  userRole: string | null = null; 
  
  constructor(private authService: AuthService, private router: Router, private alertService: AlertService) {}
  
  ngOnInit() {
    this.myUser = this.authService.getCurrentUser();
    this.isLoggedIn = this.authService.isLoggedIn();
    this.userRole = this.myUser?.role || null;
    
    
    this.authService.authStatus.subscribe(status => {
      this.isLoggedIn = status;
      this.userRole = this.authService.getCurrentUser()?.role || null;
    });
   
    this.authService.authStatus.subscribe(status => {
      this.isLoggedIn = status;
     
    });
  }
  toggleSidebarVisibility() {
    this.toggleSidebar.emit();
  }

 
  toggleUserMenu(event: MouseEvent) {
    event.stopPropagation(); 
    this.isMenuOpen = !this.isMenuOpen;
  }

  @HostListener('document:click', ['$event'])
  closeMenu() {
    this.isMenuOpen = false;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/home/home']);
    this.alertService.showAlert('Vous avez été déconnecté avec succès.', 'success');
  }
}
