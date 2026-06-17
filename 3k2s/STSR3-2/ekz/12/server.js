// 2. Разработка HTTP-клиента в Node.js. Обработка json-ответа. Пример. Тестирование с помощью с Node.js-сервера.   

const http = require('http');

const server = http.createServer((req, res) => {

    if (req.method === 'GET' && req.url === '/json') {
        const data = {
            name: 'Vi',
            age: 20
        };

        res.writeHead(200, {'Content-Type': 'application/json; charset=utf-8'});
        res.end(JSON.stringify(data));
    }

    else {
        res.writeHead(404, {'Content-Type': 'text/plain; charset=utf-8'});
        res.end('404 Not Found');
    }
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000');
});