//Разработка HTTP-сервера в Node.js. Обработка запросов к статическим ресурсам:  html, css, js, png, msword.  Пример. Тестирование с помощью браузера.
const http = require('http');
const fs = require('fs');
const path = require('path');

const mimeTypes = {
    '.html': 'text/html; charset=utf-8',
    '.css': 'text/css; charset=utf-8',
    '.js': 'text/javascript; charset=utf-8',
    '.png': 'image/png',
    '.doc': 'application/msword'
};

const server = http.createServer((req, res) => {
    const url = new URL(req.url, 'http://localhost:3000');

    let fileName = url.pathname;

    const filePath = path.join(__dirname, 'public', fileName);
    const ext = path.extname(filePath);
    const contentType = mimeTypes[ext];

    if (!contentType) {
        res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
        res.end('404 Not Found');
        return;
    }

    fs.readFile(filePath, (err, data) => {
        if (err) {
            res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
            res.end('404 Not Found');
            return;
        }

        res.writeHead(200, { 'Content-Type': contentType });
        res.end(data);
    });
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000/index.html');
});