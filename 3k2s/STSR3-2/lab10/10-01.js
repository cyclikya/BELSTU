const http = require('http');
const WebSocket = require('ws');

const HTTP_PORT = 3000;
const WS_PORT = 4000;

const html = `<!DOCTYPE html>
<html lang="ru">
<head>
  <meta charset="UTF-8">
  <title>10-01</title>
  <style>
    body { font-family: Arial, sans-serif; margin: 20px; }
    #log { border: 1px solid #999; padding: 10px; height: 300px; overflow-y: auto; white-space: pre-wrap; }
    button { padding: 10px 16px; margin-bottom: 12px; }
  </style>
</head>
<body>
  <button onclick="startWS()">startWS</button>
  <div id="log"></div>

  <script>
    let activeSockets = [];

    function write(text) {
      const log = document.getElementById('log');
      log.textContent += text + '\\n';
      log.scrollTop = log.scrollHeight;
    }

    function startWS() {
      const ws = new WebSocket('ws://localhost:${WS_PORT}');
      activeSockets.push(ws);

      let clientCounter = 0;
      let sendTimer = null;
      let stopTimer = null;

      ws.onopen = () => {
        write('[client] соединение открыто');

        sendTimer = setInterval(() => {
          clientCounter++;
          const msg = '10-01-client: ' + clientCounter;
          ws.send(msg);
          write('[client -> server] ' + msg);
        }, 3000);

        stopTimer = setTimeout(() => {
          clearInterval(sendTimer);
          write('[client] 25 сек прошло, закрываю соединение');
          ws.close();
        }, 25000);
      };

      ws.onmessage = (event) => {
        write('[server -> client] ' + event.data);
      };

      ws.onclose = () => {
        clearInterval(sendTimer);
        clearTimeout(stopTimer);
        write('[client] соединение закрыто');
      };

      ws.onerror = (error) => {
        write('[client] ошибка WebSocket');
        console.error(error);
      };
    }
  </script>
</body>
</html>`;

const httpServer = http.createServer((req, res) => {
  if (req.method === 'GET' && req.url === '/start') {
    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
    res.end(html);
    return;
  }

  res.writeHead(400, { 'Content-Type': 'text/plain; charset=utf-8' });
  res.end('400 Bad Request');
});

httpServer.listen(HTTP_PORT, () => {
  console.log(`HTTP server started on http://localhost:${HTTP_PORT}/start`);
});

const wss = new WebSocket.Server({ port: WS_PORT });

wss.on('connection', (ws) => {
  console.log('WS client connected');

  let lastClientNumber = 0;
  let serverCounter = 0;

  const timer = setInterval(() => {
    serverCounter++;
    const reply = `10-01-server: ${lastClientNumber}->${serverCounter}`;

    if (ws.readyState === WebSocket.OPEN) {
      ws.send(reply);
      console.log('[server -> client]', reply);
    }
  }, 5000);

  ws.on('message', (data) => {
    const text = data.toString();
    console.log('[client -> server]', text);

    const match = text.match(/10-01-client:\s*(\d+)/);
    if (match) {
      lastClientNumber = Number(match[1]);
    }
  });

  ws.on('close', () => {
    clearInterval(timer);
    console.log('WS client disconnected');
  });

  ws.on('error', (err) => {
    console.error('WS error:', err.message);
  });
});

console.log(`WS server started on ws://localhost:${WS_PORT}`);
