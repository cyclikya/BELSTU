//2. Пакет Express. Основные принципы работы. Middleware-код. Пример.

const express = require('express');

const app = express();

app.use((req, res, next) => {
    console.log(`method = ${req.method}`);
    console.log(`url = ${req.url}`);

    next();
});

app.get('/', (req, res) => {
    res.send('Hello Express');
});

app.use((req, res) => {
    res.status(404).send('404 Not Found');
});

app.listen(3000, () => {
    console.log('Server started: http://localhost:3000');
});

//npm install express