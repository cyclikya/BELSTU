const http = require('http');
const fs = require('fs')
const express = require('express');

const app = express();

const options = {
    key: fs.readFileSync('resource-key.pem'),
    cert: fs.readFileSync('resource-csr.pem')
};

app.get('/', (req, res) => {
    res.send('https server is working');
})
.get('/resource', (req, res) => {
    res.send('resource - secure resource');
});

http.createServer(options, app).listen(3000, () => {
    console.log('HTTPS сервер запущен на порту 3000');
    console.log('http://localhost:3000');
    console.log('http://LAB22-DUS:3000');
    console.log('http://DUS:3000');
})