const { app, BrowserWindow } = require('electron');
const path = require('path');

// Configuration de electron-reloader
try {
  require('electron-reloader')(module, {
    debug: true, // Active les logs de débogage
    watchRenderer: true, // Surveille les fichiers du renderer (Angular)
  });
} catch (err) {
  console.error('Erreur lors de l\'initialisation d\'electron-reloader', err);
}

// Créer la fenêtre principale
function createWindow() {
  const mainWindow = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      nodeIntegration: false, // Désactive Node.js dans le renderer pour des raisons de sécurité
      contextIsolation: true, // Active l'isolation de contexte pour la sécurité
      sandbox: true, // Active le mode sandbox pour la sécurité
    },
  });

  // Charger l'application Angular
  if (process.env.NODE_ENV === 'development') {
    mainWindow.loadURL('http://localhost:4200'); // URL de développement Angular
  } else {
    mainWindow.loadFile(path.join(__dirname, 'dist/your-angular-project/index.html')); // Chemin de production
  }

  // Ouvrir les DevTools uniquement en mode développement
  if (process.env.NODE_ENV === 'development') {
    mainWindow.webContents.openDevTools();
  }
}

// Démarrer l'application Electron
app.whenReady().then(() => {
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow();
    }
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});