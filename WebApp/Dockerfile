# Utiliser l'image officielle de Node.js pour construire l'application Angular
FROM node:18 AS build
WORKDIR /app

# Copier les fichiers package.json et package-lock.json
COPY package*.json ./

# Installer les dépendances
RUN npm install

# Copier tout le code source
COPY . .

# Construire l'application Angular
RUN npm run build -- --configuration production

# Utiliser l'image officielle de Nginx pour servir l'application
FROM nginx:alpine
COPY --from=build /app/dist/cinephoria /usr/share/nginx/html

# Exposer le port 80
EXPOSE 80

# Définir la commande pour démarrer Nginx
CMD ["nginx", "-g", "daemon off;"]