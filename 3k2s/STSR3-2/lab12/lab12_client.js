// ===== WebSocket Client для получения уведомлений об обновлениях =====
const WebSocket = require('ws');

const socket = new WebSocket('ws://localhost:3000');

// Обработка события подключения к серверу
socket.on('open', () => {
    console.log('Поключился к  серверу');
});

// Получение и вывод уведомлений от сервера
socket.on('message', (data) => {
    console.log('Уведомление:', JSON.parse(data).message);
});

// Обработка события отключения от сервера
socket.on('close', () => {
    console.log('Отлючился от сервера');
});