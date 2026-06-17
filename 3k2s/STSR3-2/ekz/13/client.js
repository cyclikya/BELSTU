const http = require('http');
const fs = require('fs');

const req = http.request('http://localhost:3000/upload', {method: 'POST'}, res => {
    let body = '';

    res.on('data', chunk => {
        body += chunk;
    });

    res.on('end', () => {
        console.log(body);
    });
});

fs.createReadStream('test.txt').pipe(req);