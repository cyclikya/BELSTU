// 2. Разаботка Websockets-приложения:  обработка json-сообщений, Node.js-сервер, Node.js-клиент. Пример.

const WebSocket = require('ws');

const server = new WebSocket.Server({port: 3000});

server.on('connection', ws => {
    console.log('client connected');

    ws.on('message', message => {
        const data = JSON.parse(message);

        console.log(`name = ${data.name}`);
        console.log(`age = ${data.age}`);

        ws.send(JSON.stringify({
            status: 'ok',
            text: 'json received'
        }));
    });
});

console.log('WebSocket server started: ws://localhost:3000');

//npm install ws