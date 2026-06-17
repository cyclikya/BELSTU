// Разработка HTTP-сервера в Node.js. Обработка query-параметров GET-запроса. Пример. Тестирование с помощью браузера.
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
    console.log('Server started: http://localhost:3000/api?name=Vi&age=20');

});