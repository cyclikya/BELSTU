const http = require('http');

const PORT = 3000;

// Объект с удалёнными процедурами JSON-RPC
const methods = {
    sum(params) {
        return params.reduce((sum, number) => sum + number, 0);
    },

    mul(params) {
        return params.reduce((mul, number) => mul * number, 1);
    },

    div(params) {
        const [x, y] = params;
        return x / y;
    },

    proc(params) {
        const [x, y] = params;
        return (x / y) * 100;
    }
};

// Функция обрабатывает один JSON-RPC запрос
function handleRequest(data) {
    const { method, params, id } = data;

    if (!methods[method]) {
        return {
            jsonrpc: '2.0',
            error: {
                code: -32601,
                message: 'Method not found'
            },
            id
        };
    }

    const result = methods[method](params);

    return {
        jsonrpc: '2.0',
        result,
        id
    };
}

// Создаём HTTP-сервер
const server = http.createServer((req, res) => {
    if (req.method !== 'POST') {
        res.writeHead(405, { 'Content-Type': 'application/json' });
        res.end(JSON.stringify({ error: 'Only POST requests are allowed' }));
        return;
    }

    let body = '';

    req.on('data', chunk => {
        body += chunk;
    });

    req.on('end', () => {
        try {
            const data = JSON.parse(body);

            const response = handleRequest(data);

            res.writeHead(200, { 'Content-Type': 'application/json' });
            res.end(JSON.stringify(response));
        } catch (error) {

            res.writeHead(400, { 'Content-Type': 'application/json' });
            res.end(JSON.stringify({
                jsonrpc: '2.0',
                error: {
                    code: -32700,
                    message: 'Parse error'
                },
                id: null
            }));
        }
    });
});

server.listen(3000, () => {
    console.log(`JSON-RPC server is running on http://localhost:3000`);
});