const WebSocket = require('ws');

const socket = new WebSocket('ws://localhost:3000');

socket.on('open', () => {
    const data = {
        name: 'Vi',
        age: 20
    };

    socket.send(JSON.stringify(data));
});

socket.on('message', message => {
    const data = JSON.parse(message);

    console.log(data.status);
    console.log(data.text);
});