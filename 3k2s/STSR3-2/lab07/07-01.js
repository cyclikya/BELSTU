const http = require('http');
const m0701 = require('./m07-01');

const PORT = 3000;

const handler = m0701('static');

http.createServer((req, res) => {
    handler(req, res);
}).listen(PORT, () => {
    console.log(`Server running at http://localhost:${PORT}`);
});