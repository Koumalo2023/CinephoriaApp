import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RedirectService {
  private currentStepSubject = new BehaviorSubject<number>(1); // Étape actuelle du processus de réservation
  currentStep$ = this.currentStepSubject.asObservable();

  // Définir l'étape actuelle
  setCurrentStep(step: number): void {
    this.currentStepSubject.next(step);
  }

  // Récupérer l'étape actuelle
  getCurrentStep(): number {
    return this.currentStepSubject.value;
  }
}

//Code d'exemple pour la redirection dans un composant
// export class MovieDetailsComponent {
//     constructor(private router: Router, private redirectService: RedirectService) {}
  
//     onReserveClick(): void {
//       // Stocker l'URL actuelle avant de rediriger vers la page de connexion
//       this.redirectService.setRedirectUrl(this.router.url);
  
//       // Rediriger vers la page de connexion
//       this.router.navigate(['/login']);
//     }
// }