const http = require('http');

const body = JSON.stringify({name: 'Vi', age: 20});

const req = http.request('http://localhost:3000/json', {method: 'POST'}, res => {
    let data = '';

    res.on('data', chunk => {
        data += chunk;
    });

    res.on('end', () => {
        console.log(data);
    });
});

req.write(body);
req.end();