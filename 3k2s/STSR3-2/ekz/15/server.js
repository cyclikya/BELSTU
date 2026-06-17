// 2. Разработка Websockets-приложения: Node.js-сервер, браузер-клиент. Пример.

const WebSocket = require('ws');

const server = new WebSocket.Server({port: 3000});

server.on('connection', ws => {
    console.log('client connected');

    ws.on('message', message => {
        console.log(`client: ${message}`);

        ws.send(`server received: ${message}`);
    });
});

console.log('WebSocket server started:  ws://localhost:3000');

//npm install ws