// 2. Разработка HTTP-клиента в Node.js. Обработка ответа с файлом (download). Пример. Тестирование с помощью с Node.js-сервера.   

const http = require('http');
const fs = require('fs');

const server = http.createServer((req, res) => {

    if (req.method === 'GET' && req.url === '/download') {
        res.writeHead(200, {'Content-Type': 'text/plain; charset=utf-8'});

        fs.createReadStream('test.txt').pipe(res);
    }

    else {
        res.writeHead(404, {'Content-Type': 'text/plain; charset=utf-8'});
        res.end('404 Not Found');
    }
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000');
});