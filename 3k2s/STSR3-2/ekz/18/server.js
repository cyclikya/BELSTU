//2. Применение функции pipe для обработки данных (файла) файловой системы  и записи в http-ответ. Пример.

const http = require('http');
const fs = require('fs');

const server = http.createServer((req, res) => {

    if (req.method === 'GET' && req.url === '/file') {
        res.writeHead(200, {'Content-Type': 'text/plain; charset=utf-8'});

        fs.createReadStream('file.txt').pipe(res);
    }

    else {
        res.writeHead(404, {'Content-Type': 'text/plain; charset=utf-8'});
        res.end('404 Not Found');
    }
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000/file');
});