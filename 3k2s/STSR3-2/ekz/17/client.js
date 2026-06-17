const WebSocket = require('ws');

const socket = new WebSocket('ws://localhost:3000');

socket.on('open', () => {
    const request = {
        method: 'sum',
        params: {a: 5, b: 3}
    };

    socket.send(JSON.stringify(request));
});

socket.on('message', message => {
    const response = JSON.parse(message);

    console.log(`result = ${response.result}`);
});