/// <reference types="@angular/localize" />

import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { registerLocaleData } from '@angular/common';
import { enableProdMode } from '@angular/core';
import { environment } from './environments/environment';
import localeFr from '@angular/common/locales/fr';

// Enregistrez les données de localisation pour le français
registerLocaleData(localeFr);

// if (environment.production) {
//   enableProdMode();
// }

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
