// 2. Разработка  RPC-Websockets-сервера. Пример. Тестирование:  Node.js-клиент. 

const WebSocket = require('ws');

const server = new WebSocket.Server({port: 3000});

server.on('connection', ws => {

    ws.on('message', message => {
        const request = JSON.parse(message);

        let result;

        if (request.method === 'sum') {
            result = request.params.a + request.params.b;
        }

        ws.send(JSON.stringify({
            result: result
        }));
    });
});

console.log('RPC WebSocket server started');


//npm install ws