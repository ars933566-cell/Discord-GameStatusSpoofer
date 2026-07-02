const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');
const DiscordRPC = require('discord-rpc');

let mainWindow;
let rpc;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 450,
    height: 600,
    webPreferences: { nodeIntegration: true, contextIsolation: false }
  });
  mainWindow.loadFile('index.html');
}

app.whenReady().then(createWindow);

// Listen for the user clicking "Update Presence" in the app interface
ipcMain.on('update-status', (event, data) => {
  if (rpc) rpc.destroy();

  rpc = new DiscordRPC.Client({ transport: 'ipc' });
  
  rpc.on('ready', () => {
    rpc.setActivity({
      details: data.details,
      state: data.state,
      largeImageKey: data.imageLink,
      largeImageText: data.gameName,
      instance: false,
    });
  });

  // Connect using either a custom App ID or a default dummy ID
  rpc.login({ clientId: data.appId || '123456789012345678' }).catch(console.error);
});
