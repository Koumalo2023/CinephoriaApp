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
