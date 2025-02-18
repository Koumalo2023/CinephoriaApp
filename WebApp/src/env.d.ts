// src/env.d.ts
declare global {
    namespace NodeJS {
      interface ProcessEnv {
        API_URL: string; // Définissez le type de votre variable d'environnement ici
      }
    }
  }s
  
  export {};