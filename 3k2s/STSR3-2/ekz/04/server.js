// 2. Разработка HTTP-сервера в Node.js. Обработка uri-параметров GET-запроса. Пример. Тестирование с помощью браузера.const http = require('http');

const http = require('http');

const server = http.createServer((req, res) => {
    const url = new URL(req.url, 'http://localhost:3000');

    if (req.method === 'GET' && url.pathname.startsWith('/user/')) {
        const parts = url.pathname.split('/');
        const id = parts[2];

        res.writeHead(200, {'Content-Type': 'text/plain; charset=utf-8'});

        res.end(`id пользователя = ${id}`);
    }
    else {
        res.writeHead(404, {'Content-Type': 'text/plain; charset=utf-8'});

        res.end('404 Not Found');
    }
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000');
    console.log('Open: http://localhost:3000/user/5');
});