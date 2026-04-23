const WebSocket = require('ws');

const PORT = 5000;
const wss = new WebSocket.Server({ port: PORT });

let messageCounter = 0;

wss.on('connection', (ws) => {
  console.log('Client connected. Total clients:', wss.clients.size);

  ws.on('message', (data) => {
    const text = data.toString();
    messageCounter++;
    const broadcastMessage = `broadcast ${messageCounter}: ${text}`;

    console.log('[received]', text);
    console.log('[broadcast]', broadcastMessage);

    wss.clients.forEach((client) => {
      if (client.readyState === WebSocket.OPEN) {
        client.send(broadcastMessage);
      }
    });
  });

  ws.on('close', () => {
    console.log('Client disconnected. Total clients:', wss.clients.size);
  });

  ws.on('error', (err) => {
    console.error('Client error:', err.message);
  });
});

console.log(`Broadcast WS server started on ws://localhost:${PORT}`);
