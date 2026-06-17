//2. Пакет Express. Основные принципы работы. Обработка query-параметров GET-запроса. Пример (POSTMAN).

const express = require('express');

const app = express();

app.get('/api', (req, res) => {
    const name = req.query.name;
    const age = req.query.age;

    res.send(`name = ${name}\nage = ${age}`);
});

app.use((req, res) => {
    res.status(404).send('404 Not Found');
});

app.listen(3000, () => {
    console.log('Server started: http://localhost:3000/api?name=Vi&age=20');
});

//npm install express
