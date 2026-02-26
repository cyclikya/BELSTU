// Подключаем модуль http для создания сервера
const http = require('http');

// Создаем сервер
const server = http.createServer((req, res) => {
    // Устанавливаем заголовок ответа - тип контента HTML
    res.writeHead(200, {'Content-Type': 'text/html'});
    
    // Отправляем HTML разметку
    res.write('<h1>Hello World</h1>');
    
    // Завершаем ответ
    res.end();
});

// Запускаем сервер на порту 3000
server.listen(3000, () => {
    console.log('Сервер запущен на http://localhost:3000');
});