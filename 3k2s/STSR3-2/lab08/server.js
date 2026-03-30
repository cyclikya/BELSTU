const http = require('http');
const url  = require('url');
const handlers = require('./handlers');


const PORT = 5000;

const server = http.createServer((req, res) => {
    const parsedUrl = url.parse(req.url, true);  // true — парсить query
    const pathname  = parsedUrl.pathname;        // путь без query
    const query     = parsedUrl.query;           // объект с query-параметрами
    const method    = req.method;                // GET, POST, ...

    console.log(`${method} ${req.url}`);

    if (method === 'GET' && pathname === '/connection') {
        handlers.handleConnection(req, res, query, server);
    }
    else if (method === 'GET' && pathname === '/headers') {
        handlers.handleHeaders(req, res);
    }
    else if (method === 'GET' && pathname === '/parameter' && query.x !== undefined) {
        handlers.handleParameterQuery(req, res, query);
    }
    else if (method === 'GET' && pathname.startsWith('/parameter/')) {
        handlers.handleParameterRoute(req, res, pathname);
    }
    else if (method === 'GET' && pathname === '/close') {
        handlers.handleClose(req, res, server);
    }
    else if (method === 'GET' && pathname === '/socket') {
        handlers.handleSocket(req, res);
    }
    else if (method === 'GET' && pathname === '/req-data') {
        handleReqData(req, res);
    }
    else if (method === 'POST' && pathname === '/req-data') {
        handlers.handleReqData(req, res);
    }
    else if (method === 'GET' && pathname === '/resp-status') {
        handlers.handleRespStatus(req, res, query);
    }
    else if (method === 'GET' && pathname === '/formparameter') {
        handlers.handleFormParameterGet(req, res);
    }
    else if (method === 'POST' && pathname === '/formparameter') {
        handlers.handleFormParameterPost(req, res);
    }
    else if (method === 'POST' && pathname === '/json') {
        handlers.handleJson(req, res);
    }
    else if (method === 'POST' && pathname === '/xml') {
        handlers.handleXml(req, res);
    }
    else if (method === 'GET' && pathname === '/files') {
        handlers.handleFiles(req, res);
    }
    else if (method === 'GET' && pathname.startsWith('/files/')) {
        handlers.handleFileByName(req, res, pathname);
    }
    else if (method === 'GET' && pathname === '/upload') {
        handlers.handleUploadGet(req, res);
    }
    else if (method === 'POST' && pathname === '/upload') {
        handlers.handleUploadPost(req, res);
    }
    else {
        res.writeHead(404, { 'Content-Type': 'text/html; charset=utf-8' });
        res.end('<h1>404 — Маршрут не найден</h1>');
    }
});

server.listen(PORT, () => {
    console.log(`Сервер запущен`)
})