import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { config } from './app/app.config.server';
import * as dotenv from 'dotenv';

// Charger les variables d'environnement
dotenv.config();

// Vérifiez si API_URL est définie
if (!process.env.API_URL) {
  throw new Error('API_URL is not set in the environment variables');
}


const bootstrap = () => bootstrapApplication(AppComponent, config);
export default bootstrap;