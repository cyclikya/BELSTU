const WebSocket = require('ws');

const clientName = process.argv[2];
const ws = new WebSocket('ws://localhost:5000');

const SEND_INTERVAL_MS = 3000;
const MAX_MESSAGES = 3;
let sentCount = 0;
let sendTimer = null;

ws.on('open', () => {
  console.log(`[${clientName}] connected to server`);

  sendTimer = setInterval(() => {
    sentCount++;
    const message = `${clientName}: auto-${sentCount}`;
    ws.send(message);
    console.log(`[${clientName}] sent: ${message}`);

    if (sentCount >= MAX_MESSAGES) {
      clearInterval(sendTimer);
      console.log(`[${clientName}] done, closing`);
      ws.close();
    }
  }, SEND_INTERVAL_MS);
});

ws.on('message', (data) => {
  console.log(`[${clientName}] received: ${data.toString()}`);
});

ws.on('close', () => {
  clearInterval(sendTimer);
  console.log(`[${clientName}] disconnected`);
});

ws.on('error', (err) => {
  console.error(`[${clientName}] error: ${err.message}`);
});