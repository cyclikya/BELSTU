const http = require('http');
const fs = require('fs');
const path = require('path');

const server = http.createServer((req, res) => {
    if (req.url === '/xmlhttprequest' && req.method === 'GET') {
        const filePath = path.join(__dirname, 'resourses', 'xmlhttprequest.html');
        
        fs.readFile(filePath, 'utf8', (err, data) => {
            if (err) {
                res.writeHead(500, { 'Content-Type': 'text/plain' });
                res.end('Internal Server Error');
                return;
            }
            
            res.writeHead(200, { 'Content-Type': 'text/html' });
            res.end(data);
        });
        
    } else if (req.url === '/api/name' && req.method === 'GET') {
        res.writeHead(200, { 'Content-Type': 'text/plain; charset=utf-8' });
        res.end('Угоренко Виолетта Романовна');
    }
});

server.listen(5000, () => {
    console.log('Server 02-04 running at http://localhost:5000/xmlhttprequest');
    console.log('Or at http://localhost:5000/api/name');
});