import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class RedirectService {
  private redirectUrl: string | null = null;

  // Stocker l'URL de redirection
  setRedirectUrl(url: string): void {
    this.redirectUrl = url;
  }

  // Récupérer l'URL de redirection
  getRedirectUrl(): string | null {
    return this.redirectUrl;
  }

  // Effacer l'URL de redirection après utilisation
  clearRedirectUrl(): void {
    this.redirectUrl = null;
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