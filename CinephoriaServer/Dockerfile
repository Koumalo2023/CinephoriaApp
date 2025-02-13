# Utiliser l'image officielle de .NET 8 SDK pour construire l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier les fichiers projet et restaurer les dépendances
COPY *.csproj .
RUN dotnet restore

# Copier tout le code source et construire l'application
COPY . .
RUN dotnet publish -c Release -o /app

# Utiliser l'image officielle de .NET 8 runtime pour exécuter l'application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Exposer le port sur lequel l'application écoute
EXPOSE 80

# Définir la commande pour démarrer l'application
ENTRYPOINT ["dotnet", "CinephoriaServer.dll"]