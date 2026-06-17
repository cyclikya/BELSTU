const http = require('http');

const req = http.request('http://localhost:3000/json', {method: 'GET'}, res => {
    let body = '';

    res.on('data', chunk => {
        body += chunk;
    });

    res.on('end', () => {
        const data = JSON.parse(body);

        console.log(`name = ${data.name}`);
        console.log(`age = ${data.age}`);
    });
});

req.end();