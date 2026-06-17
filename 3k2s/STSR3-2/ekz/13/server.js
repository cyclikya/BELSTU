// 2. Разработка HTTP-клиента в Node.js.  Пересылка файла на сервер в POST-запросе (upload).   Пример. Тестирование с помощью с Node.js-сервера.

const http = require('http');
const fs = require('fs');

const server = http.createServer((req, res) => {

    if (req.method === 'POST' && req.url === '/upload') {
        const file = fs.createWriteStream('uploaded.txt');

        req.pipe(file);

        req.on('end', () => {
            res.writeHead(200, {'Content-Type': 'text/plain; charset=utf-8'});
            res.end('file uploaded');
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