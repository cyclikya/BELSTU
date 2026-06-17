// 2. Разработка HTTP-клиента в Node.js.  Оправка GET запроса с query-параметрами. Пример. Тестирование с помощью с Node.js-сервера.   

const http = require('http');

const server = http.createServer((req, res) => {
    const url = new URL(req.url, 'http://localhost:3000');

    if (req.method === 'GET' && url.pathname === '/api') {
        const name = url.searchParams.get('name');
        const age = url.searchParams.get('age');

        res.writeHead(200, {'Content-Type': 'text/plain; charset=utf-8'});
        res.end(`name = ${name}\nage = ${age}`);
    }
    else {
        res.writeHead(404, {'Content-Type': 'text/plain; charset=utf-8'});
        res.end('404 Not Found');
    }
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000');
});