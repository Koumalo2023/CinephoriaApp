import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertService } from '../../core/services/alert.service';
import { ConfirmationDialogComponent } from '../sharedComponents/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule, FooterComponent, HeaderComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  
}
