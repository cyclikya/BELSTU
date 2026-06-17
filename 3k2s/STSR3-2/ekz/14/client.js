const http = require('http');
const fs = require('fs');

const file = fs.createWriteStream('downloaded.txt');

const req = http.request('http://localhost:3000/download', {method: 'GET'}, res => {
    res.pipe(file);

    res.on('end', () => {
        console.log('file downloaded');
    });
});

req.end();