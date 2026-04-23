const WebSocket = require('ws');

const ws = new WebSocket('ws://localhost:4000');

let clientCounter = 0;
let sendTimer = null;
let stopTimer = null;

ws.on('open', () => {
  console.log('[10-02] connection opened');

  sendTimer = setInterval(() => {
    clientCounter++;
    const msg = `10-01-client: ${clientCounter}`;
    ws.send(msg);
    console.log('[10-02 -> server]', msg);
  }, 3000);

  stopTimer = setTimeout(() => {
    clearInterval(sendTimer);
    console.log('[10-02] 25 seconds passed, closing connection');
    ws.close();
  }, 25000);
});

ws.on('message', (data) => {
  console.log('[server -> 10-02]', data.toString());
});

ws.on('close', () => {
  clearInterval(sendTimer);
  clearTimeout(stopTimer);
  console.log('[10-02] connection closed');
});

ws.on('error', (err) => {
  console.error('[10-02] error:', err.message);
});
