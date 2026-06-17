// 2. Разработка HTTP-клиента в Node.js.  Оправка POST-запроса с параметрами в теле.  Пример. Тестирование с помощью с Node.js-сервера.

const http = require('http');

const server = http.createServer((req, res) => {

    if (req.method === 'POST' && req.url === '/form') {
        let body = '';

        req.on('data', chunk => {
            body += chunk;
        });

        req.on('end', () => {
            const params = new URLSearchParams(body);

            res.writeHead(200, {'Content-Type': 'text/plain; charset=utf-8'});
            res.end(`name = ${params.get('name')}\nage = ${params.get('age')}`);
        });
    }

    else {
        res.writeHead(404, {'Content-Type': 'text/plain; charset=utf-8'});
        res.end('404 Not Found');
    }
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000');
});