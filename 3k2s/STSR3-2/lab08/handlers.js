const http = require('http');
const url  = require('url');
const fs  = require('fs');


//Задание 1
function handleConnection(req, res, query, server) {
    if (query.set !== undefined) {
        const newValue = parseInt(query.set);
        server.keepAliveTimeout = newValue;
        res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
        res.end(`<h1>Установлено новое значение KeepAliveTimeout = ${newValue} мс</h1>`);
    } else {
        res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
        res.end(`<h1>Текущее значение KeepAliveTimeout = ${server.keepAliveTimeout} мс</h1>`);
    }

}

//Задание 2
function handleHeaders(req, res) {
    res.setHeader('X-My-Custom-Header', 'HelloFromServer-2024');
    res.setHeader('X-Request-Time', new Date().toISOString());

    let html = '<h1>Заголовки запроса (Request Headers)</h1><table border="1">';
    html += '<tr><th>Заголовок</th><th>Значение</th></tr>';
    
    for (const [key, value] of Object.entries(req.headers)) {
        html += `<tr><td>${key}</td><td>${value}</td></tr>`;
    }
    html += '</table>';

    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });

    const responseHeaders = res.getHeaders();
    html += '<h1>Заголовки ответа (Response Headers)</h1><table border="1">';
    html += '<tr><th>Заголовок</th><th>Значение</th></tr>';
    for (const [key, value] of Object.entries(responseHeaders)) {
        html += `<tr><td>${key}</td><td>${value}</td></tr>`;
    }
    html += '</table>';

    res.end(html);
}

//Задание 3
function handleParameterQuery(req, res, query) {
    const x = query.x;
    const y = query.y;
    
    const numX = parseFloat(x);
    const numY = parseFloat(y);

    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });

    if (isNaN(numX) || isNaN(numY)) {
        res.end(`<h1>Ошибка!</h1><p>Параметры x="${x}" и y="${y}" должны быть числами.</p>`);
    } else {
        let html = `<h1>Результаты вычислений (Query Parameters)</h1>`;
        html += `<p>x = ${numX}, y = ${numY}</p>`;
        html += `<p>x + y = ${numX + numY}</p>`;
        html += `<p>x - y = ${numX - numY}</p>`;
        html += `<p>x * y = ${numX * numY}</p>`;
        html += `<p>x / y = ${numY !== 0 ? (numX / numY) : 'деление на 0!'}</p>`;
        res.end(html);
    }
}

//Задание 4
function handleParameterRoute(req, res, pathname) {
    const parts = pathname.split('/');
    const x = parts[2];
    const y = parts[3];

    const numX = parseFloat(x);
    const numY = parseFloat(y);

    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });

    if (isNaN(numX) || isNaN(numY)) {
        res.end(`<h1>URI: ${pathname}</h1><p>Параметры не являются числами.</p>`);
    } else {
        let html = `<h1>Результаты вычислений (Route Parameters)</h1>`;
        html += `<p>x = ${numX}, y = ${numY}</p>`;
        html += `<p>x + y = ${numX + numY}</p>`;
        html += `<p>x - y = ${numX - numY}</p>`;
        html += `<p>x * y = ${numX * numY}</p>`;
        html += `<p>x / y = ${numY !== 0 ? (numX / numY) : 'деление на 0!'}</p>`;
        res.end(html);
    }
}

//Задание 5
function handleClose(req, res, server) {
    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
    res.end('<h1>Сервер будет остановлен через 10 секунд...</h1>');

    setTimeout(() => {
        console.log('Сервер остановлен.');
        server.close(() => {
            process.exit(0);
        });
    }, 5000);
}    

//Задание 6
function handleSocket(req, res) {
    const clientIP   = req.socket.remoteAddress;
    const clientPort = req.socket.remotePort;
    const serverIP   = req.socket.localAddress;
    const serverPort = req.socket.localPort;

    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
    let html = '<h1>Информация о соединении</h1>';
    html += `<p><b>Клиент:</b> IP = ${clientIP}, Port = ${clientPort}</p>`;
    html += `<p><b>Сервер:</b> IP = ${serverIP}, Port = ${serverPort}</p>`;
    res.end(html);
}

//Задание 7
function handleReqData(req, res) {
    let body = '';
    let chunkCount = 0;

    console.log('--- Начало приёма данных ---');

    req.on('data', (chunk) => {
        chunkCount++;
        console.log(`Chunk #${chunkCount}, размер: ${chunk.length} байт`);
        body += chunk.toString();
    });

    req.on('end', () => {
        console.log(`--- Всего получено ${chunkCount} chunk(ов), ${body.length} байт ---`);
        
        res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
        let html = '<h1>Порционная обработка запроса</h1>';
        html += `<p>Количество чанков (порций): ${chunkCount}</p>`;
        html += `<p>Общий размер тела запроса: ${body.length} байт</p>`;
        html += `<p>Первые 500 символов тела:</p>`;
        html += `<pre>${body.substring(0, 500)}</pre>`;
        res.end(html);
    });
}

//Задание 8
function handleRespStatus(req, res, query) {
    let code = query.code;
    let mess = query.mess;

    if (!mess) {
        const rawQuery = url.parse(req.url).query;
        const parts = rawQuery.split('?');
        for (const part of parts) {
            const [key, value] = part.split('=');
            if (key === 'code') code = value;
            if (key === 'mess') mess = value;
        }
    }

    const statusCode = parseInt(code) || 200;
    const statusMessage = mess || 'OK';

    res.writeHead(statusCode, statusMessage, { 'Content-Type': 'text/html; charset=utf-8' });
    res.end(`<h1>Ответ со статусом ${statusCode} ${statusMessage}</h1>`);
}

//Задание 9
function handleFormParameterGet(req, res) {
    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
    const html = `
    <!DOCTYPE html>
    <html>
    <head><title>Форма</title></head>
    <body>
        <h1>Задание 09: HTML-форма</h1>
        <form method="POST" action="/formparameter">
            <p>
                <label>Имя (text): <input type="text" name="username" value="Vi"></label>
            </p>
            <p>
                <label>Возраст (number): <input type="number" name="age" value="20"></label>
            </p>
            <p>
                <label>Дата рождения (date): <input type="date" name="birthdate"></label>
            </p>
            <p>Хобби (checkbox):<br>
                <label><input type="checkbox" name="hobby" value="sport"> Спорт</label><br>
                <label><input type="checkbox" name="hobby" value="music"> Музыка</label><br>
                <label><input type="checkbox" name="hobby" value="reading"> Чтение</label>
            </p>
            <p>Пол (radiobutton):<br>
                <label><input type="radio" name="gender" value="male"> Мужской</label><br>
                <label><input type="radio" name="gender" value="female"> Женский</label>
            </p>
            <p>
                <label>Комментарий (textarea):<br>
                <textarea name="comment" rows="4" cols="40">Комментарий</textarea></label>
            </p>
            <p>
                <input type="submit" name="action" value="Сохранить">
                <input type="submit" name="action" value="Отменить">
            </p>
        </form>
    </body>
    </html>`;
    res.end(html);
}

function handleFormParameterPost(req, res) {
    let body = '';
    req.on('data', chunk => { body += chunk.toString(); });
    req.on('end', () => {
        const params = new URLSearchParams(body);
        
        res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
        let html = '<h1>Полученные параметры формы</h1><table border="1">';
        html += '<tr><th>Параметр</th><th>Значение</th></tr>';
        
        // getAll нужен для checkbox (может быть несколько значений)
        for (const key of new Set(params.keys())) {
            const values = params.getAll(key);
            html += `<tr><td>${key}</td><td>${values.join(', ')}</td></tr>`;
        }
        
        html += '</table>';
        html += '<br><a href="/formparameter">Назад к форме</a>';
        res.end(html);
    });
}

//Задание 10
function handleJson(req, res) {
    let body = '';
    req.on('data', chunk => { body += chunk.toString(); });
    req.on('end', () => {
        try {
            const data = JSON.parse(body);
            
            const x = data.x;                    // число
            const y = data.y;                    // число
            const s = data.s;                    // строка
            const m = data.m;                    // массив
            const o = data.o;                    // объект { surname, name }

            // Формируем ответ
            const response = {
                "__comment": "Ответ.Лабораторная работа 8/10",
                "x_plus_y": x + y,
                "Concatination_s_o": `${s}: ${o.surname}, ${o.name}`,
                "Length_m": m.length
            };

            res.writeHead(200, { 'Content-Type': 'application/json; charset=utf-8' });
            res.end(JSON.stringify(response, null, 2));
        } catch (e) {
            res.writeHead(400, { 'Content-Type': 'application/json; charset=utf-8' });
            res.end(JSON.stringify({ error: 'Некорректный JSON', details: e.message }));
        }
    });
}

//Задание 11
const xml2js = require('xml2js');

function handleXml(req, res) {
    let body = '';
    req.on('data', chunk => { body += chunk.toString(); });
    req.on('end', () => {
        xml2js.parseString(body, { explicitArray: true }, (err, result) => {
            if (err) {
                res.writeHead(400, { 'Content-Type': 'text/plain; charset=utf-8' });
                res.end('Ошибка парсинга XML: ' + err.message);
                return;
            }

            // result.request — корневой элемент
            const requestId = result.request.$.id; // атрибут id корневого элемента
            
            // Собираем все элементы x и m
            let sumX = 0;
            let concatM = '';
            
            // xml2js при explicitArray: true оборачивает всё в массивы
            // Элементы <x value="1"/> доступны как result.request.x
            if (result.request.x) {
                for (const xElem of result.request.x) {
                    sumX += parseFloat(xElem.$.value);
                }
            }
            
            if (result.request.m) {
                for (const mElem of result.request.m) {
                    concatM += mElem.$.value;
                }
            }

            // Генерируем ID ответа (случайное число)
            const responseId = Math.floor(Math.random() * 100);

            // Формируем XML-ответ
            const responseXml = `<?xml version="1.0" encoding="UTF-8"?>
                                 <response id="${responseId}" request="${requestId}">
                                     <sum element="x" result="${sumX}" />
                                     <concat element="m" result="${concatM}" />
                                 </response>`;

            res.writeHead(200, { 'Content-Type': 'application/xml; charset=utf-8' });
            res.end(responseXml);
        });
    });
}

//Задание 12
function handleFiles(req, res) {  // ← удалите path из параметров
    const staticDir = require('path').join(__dirname, 'static');
    
    try {
        const files = fs.readdirSync(staticDir);
        const fileCount = files.length;
        
        res.writeHead(200, { 
            'Content-Type': 'text/html; charset=utf-8',
            'X-static-files-count': fileCount.toString()
        });
        
        let html = `<h1>Файлы в директории static (всего: ${fileCount})</h1><ul>`;
        for (const file of files) {
            html += `<li><a href="/files/${file}">${file}</a></li>`;
        }
        html += '</ul>';
        res.end(html);
    } catch (e) {
        console.error('Ошибка:', e);
        res.writeHead(500, { 'Content-Type': 'text/html; charset=utf-8' });
        res.end(`<h1>Ошибка чтения директории static</h1><p>${e.message}</p>`);
    }
}

//Задание 13
function handleFileByName(req, res, pathname) {
    // pathname = "/files/test.txt"
    const filename = pathname.replace('/files/', '');
    const filePath = require('path').join(__dirname, 'static', filename);
    
    if (!fs.existsSync(filePath)) {
        res.writeHead(404, { 'Content-Type': 'text/html; charset=utf-8' });
        res.end(`<h1>404 — Файл "${filename}" не найден</h1>`);
        return;
    }

    // Определяем Content-Type по расширению
    const ext = require('path').extname(filename).toLowerCase();
    const mimeTypes = {
        '.html': 'text/html; charset=utf-8',
        '.txt':  'text/plain; charset=utf-8',
        '.png':  'image/png; charset=utf-8',
    };
    const contentType = mimeTypes[ext] || 'application/octet-stream';

    res.writeHead(200, { 'Content-Type': contentType });
    const stream = fs.createReadStream(filePath);
    stream.pipe(res);
}

//Задание 14
const Busboy = require('busboy');

function handleUploadGet(req, res) {
    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
    const html = `
    <!DOCTYPE html>
    <html>
    <head><title>Upload</title></head>
    <body>
        <h1>Задание 14: Загрузка файла</h1>
        <form method="POST" action="/upload" enctype="multipart/form-data">
            <p>
                <label>Выберите файл: <input type="file" name="uploadedFile"></label>
            </p>
            <p>
                <input type="submit" value="Загрузить">
            </p>
        </form>
    </body>
    </html>`;
    res.end(html);
}

function handleUploadPost(req, res) {
    const busboy = Busboy({ headers: req.headers });
    let savedFileName = '';

    busboy.on('file', (fieldname, file, info) => {
        const { filename, encoding, mimeType } = info;
        savedFileName = filename;
        const saveTo = require('path').join(__dirname, 'static', filename);
        console.log(`Загрузка файла: ${filename} (${mimeType})`);
        file.pipe(fs.createWriteStream(saveTo));
    });

    busboy.on('finish', () => {
        res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
        res.end(`
            <h1>Файл "${savedFileName}" успешно загружен!</h1>
            <p>Файл сохранён в директории static.</p>
            <a href="/files">Посмотреть файлы</a> | 
            <a href="/upload">Загрузить ещё</a>
        `);
    });

    req.pipe(busboy);
}


module.exports = {
    handleConnection,
    handleHeaders,
    handleParameterQuery,
    handleParameterRoute,
    handleClose,
    handleSocket,
    handleReqData,
    handleRespStatus,
    handleFormParameterGet,
    handleFormParameterPost,
    handleJson,
    handleXml,
    handleFiles,
    handleFileByName,
    handleUploadGet,
    handleUploadPost
};