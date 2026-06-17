//2. Разработка HTTP-сервера в Node.js. Обработка параметров POST-запроса. Пример. Тестирование с помощью браузера (<form>) и POSTMAN.

const http = require('http');

const server = http.createServer((req, res) => {

    if (req.method === 'GET' && req.url === '/') {
        res.writeHead(200, {'Content-Type': 'text/html; charset=utf-8'});

        res.end(`
            <form method="POST" action="/form">
                <input name="name" placeholder="Введите имя">
                <input name="age" placeholder="Введите возраст">
                <button type="submit">Отправить</button>
            </form>
        `);
    }

    else if (req.method === 'POST' && req.url === '/form') {
        let body = '';

        req.on('data', chunk => {
            body += chunk;
        });

        req.on('end', () => {
            const params = new URLSearchParams(body);

            const name = params.get('name');
            const age = params.get('age');

            res.writeHead(200, {'Content-Type': 'text/plain; charset=utf-8'});
            res.end(`name = ${name}\nage = ${age}`);
        });
    }

    else {
        res.writeHead(404, {'Content-Type': 'text/plain; charset=utf-8'});
        res.end('404 Not Found');
    }
});

server.listen(3000, () => {
    console.log('Server started: http://localhost:3000');
    console.log('Server started: http://localhost:3000/form');
});

// в постмане тело не json а  x-www-form-urlencoded 