const express = require('express');

const nick = process.argv[2];
const port = Number(process.argv[3]);
const delay = Number(process.argv[4]);

const app = express();

function answer(k) {
    return function (req, res) {
        setTimeout(function () {
            res.json({ Nick: nick, Method: req.method });
        }, delay * k);
    };
}

app.get('/A', answer(1 / 3));
app.post('/A', answer(2 / 3));
app.put('/A', answer(1));
app.delete('/A', answer(1 / 4));

app.listen(port, function () {
    console.log('TDWA05-01 Nick=' + nick + ' Port=' + port + ' Delay=' + delay);
});
