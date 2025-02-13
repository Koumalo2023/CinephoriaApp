import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ContactRequest } from '@app/core/models/user.models';
import { AlertService } from '@app/core/services/alert.service';
import { AuthService } from '@app/core/services/auth.service';
import { ContainerComponent } from '@app/layourt/sharedComponents/container/container.component';

@Component({
  selector: 'app-contacts',
  standalone: true,
  imports: [CommonModule, FormsModule, ContainerComponent],
  templateUrl: './contacts.component.html',
  styleUrl: './contacts.component.scss'
})
export class ContactsComponent {
  isLoading: boolean = false;
  contactRequest: ContactRequest = {
    username: '',
    email: '',
    title: '',
    description: ''
  };

  constructor(private authService: AuthService, private alertService: AlertService) {}

  onSubmit() {
    this.isLoading = true;
    this.authService.sendContactEmail(this.contactRequest).subscribe(
      () => {
        this.isLoading = false;
        this.alertService.showAlert('Votre message a été envoyé avec succès.', 'success');
        this.contactRequest = {
          username: '',
          email: '',
          title: '',
          description: ''
        };
      },
      (error) => {
        this.isLoading = false;
        let errorMessage = 'Une erreur s\'est produite lors de l\'envoi du message.';
        if (error.error && error.error.message) {
          errorMessage = error.error.message; 
        }
        this.alertService.showAlert(errorMessage, 'danger', error);
      }
    );
  }
}
