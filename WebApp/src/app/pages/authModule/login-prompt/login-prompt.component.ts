import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '@app/core/services/auth.service';
import { RedirectService } from '@app/core/services/redirect.service';

@Component({
  selector: 'app-login-prompt',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './login-prompt.component.html',
  styleUrl: './login-prompt.component.scss'
})
export class LoginPromptComponent {
  constructor(
    private authService: AuthService,
    private router: Router,
    private redirectService: RedirectService
  ) {}

  navigateToLogin() {
    this.redirectService.setCurrentStep(5); 
    this.router.navigate(['/auth/login']);
  }

  navigateToRegister() {
    this.redirectService.setCurrentStep(5);
    this.router.navigate(['/auth/register']);
  }
}