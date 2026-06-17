//Разработка HTTP-сервера в Node.js. Обработка GET, POST, PUT и DELETE-запросов.  Генерация ответа с кодом 404. Пример. Тестирование с помощью POSTMAN. 
const http = require('http');

const server = http.createServer((req, res) => {
    res.setHeader('Content-Type', 'text/plain; charset=utf-8');

    if (req.url === '/api' && req.method === 'GET') {
        res.statusCode = 200;
        res.end('Обработан GET-запрос');
    }
    else if (req.url === '/api' && req.method === 'POST') {
        res.statusCode = 200;
        res.end('Обработан POST-запрос');
    }
    else if (req.url === '/api' && req.method === 'PUT') {
        res.statusCode = 200;
        res.end('Обработан PUT-запрос');
    }
    else if (req.url === '/api' && req.method === 'DELETE') {
        res.statusCode = 200;
        res.end('Обработан DELETE-запрос');
    }
    else {
        res.statusCode = 404;
        res.end('404 Not Found');
    }
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000');
});