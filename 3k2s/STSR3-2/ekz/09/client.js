const http = require('http');

const req = http.request('http://localhost:3000/api?name=Vi&age=20', {method: 'GET'}, res => {
    let body = '';

    res.on('data', chunk => {
        body += chunk;
    });

    res.on('end', () => {
        console.log(body);
    });
});

req.end();