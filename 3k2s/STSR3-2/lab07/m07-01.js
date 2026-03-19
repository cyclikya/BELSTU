const fs = require('fs');
const path = require('path');

module.exports = (staticDir) => {

    const MIME_TYPES = {
        '.html': 'text/html',
        '.css': 'text/css',
        '.js': 'text/javascript',
        '.png': 'image/png',
        '.docx': 'application/msword',
        '.json': 'application/json',
        '.xml': 'application/xml',
        '.mp4': 'video/mp4'
    };

    return (req, res) => {

        if (req.method !== 'GET') {
            res.writeHead(405);
            res.end('Method Not Allowed');
            return;
        }

        let filePath = path.join(__dirname, staticDir, req.url);

        if (req.url === '/') {
            filePath = path.join(__dirname, staticDir, 'index.html');
        }

        const ext = path.extname(filePath);

        if (!MIME_TYPES[ext]) {
            res.writeHead(404);
            res.end('Not Found');
            return;
        }

        fs.readFile(filePath, (err, data) => {
            if (err) {
                res.writeHead(404);
                res.end('Not Found');
            } else {
                res.writeHead(200, {
                    'Content-Type': MIME_TYPES[ext] + '; charset=utf-8'
                });
                res.end(data);
            }
        });
    };
};