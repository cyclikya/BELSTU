//2. Разработка HTTP-сервера в Node.js. Пересылка файла в ответе (download). Пример. Тестирование с помощью браузера.

const http = require('http');
const fs = require('fs');

const server = http.createServer((req, res) => {

  // GET / — страница со ссылкой на скачивание
  if (req.method === 'GET' && req.url === '/') {
    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
    res.end('<a href="/download">Скачать файл</a>');
    return;
  }

  // GET /download — отдать файл на скачивание
  if (req.method === 'GET' && req.url === '/download') {
    fs.readFile('./sample.txt', (err, data) => {
      res.writeHead(200, { 'Content-Type': 'application/octet-stream' });
      res.end(data);
    });
  }
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000');
});