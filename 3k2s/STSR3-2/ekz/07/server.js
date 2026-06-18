//2. Разработка HTTP-сервера в Node.js. Пересылка файла в POST-запросе (upload). Пример. Тестирование с помощью браузера.

const http = require('http');
const fs = require('fs');
const multiparty = require('multiparty');   // npm install multiparty

const server = http.createServer((req, res) => {

  // GET — показать форму
  if (req.method === 'GET') {
    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
    res.end(`
      <form method="POST" enctype="multipart/form-data">            // обязательно multipart/form-data для загрузки файлов
        <input type="file" name="myfile">
        <button>Загрузить</button>
      </form>
    `);
    return;
  }

  // POST — сохранить файл
  if (req.method === 'POST') {
    const form = new multiparty.Form();
    form.parse(req, (err, fields, files) => {
      const file = files.myfile[0];
      fs.rename(file.path, './static/' + file.originalFilename, () => {
        res.end('file saved');
      });
    });
  }
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000');
});