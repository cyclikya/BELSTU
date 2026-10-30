const express = require('express');

const nick = process.argv[2];
const port = Number(process.argv[3]);

const app = express();

function answer(req, res) {
    res.json({ Nick: nick, Method: req.method });
}

app.get('/A', answer);
app.post('/A', answer);
app.put('/A', answer);
app.delete('/A', answer);

app.listen(port, function () {
    console.log('TDWA04-01 Nick=' + nick + ' Port=' + port);
});
