const http = require('http');

const server = http.createServer((req, res) => {
    const method = req.method;
    const url = req.url;
    const headers = JSON.stringify(req.headers);
    let body = '';

    // Собираем данные из тела запроса (для POST)
    req.on('data', chunk => {
        body += chunk.toString();
    });

    // Когда все данные получены
    req.on('end', () => {
        const html = `
        <!DOCTYPE html>
        <html>
        <head>
            <title>Информация о запросе</title>
            <meta charset="utf-8">
            <style>
                body { font-family: Arial; margin: 20px; }
                pre { background: #f4f4f4; padding: 10px; border-radius: 5px; }
            </style>
        </head>
        <body>
            <h1>Информация о вашем запросе</h1>
            <h2>Метод:</h2>
            <pre>${method}</pre>
            
            <h2>URI:</h2>
            <pre>${url}</pre>
            
            <h2>Заголовки:</h2>
            <pre>${headers}</pre>
            
            <h2>Тело запроса:</h2>
            <pre>${body || '(пусто)'}</pre>
        </body>
        </html>
        `;

        // Отправляем ответ
        res.writeHead(200, {'Content-Type': 'text/html'});
        res.write(html);
        res.end();
    });
});

server.listen(3000, () => {
    console.log('Сервер: http://localhost:3000');
});