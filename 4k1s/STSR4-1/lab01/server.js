const express = require('express');

const app = express();
const port = 40000;

app.use(express.json());

let savedRequest = null;

function calculate(operation, x, y) {
  let result;

  switch (operation) {
    case 'add':
      result = x + y;
      break;
    case 'sub':
      result = x - y;
      break;
    case 'mul':
      result = x * y;
      break;
    case 'div':
      result = Math.trunc(x / y);
      break;
  }

  return result;
}

function makeAnswer(request) {
  const answer = {
    op: request.op,
    x: request.x,
    y: request.y,
    result: calculate(request.op, request.x, request.y)
  };

  return answer;
}

app.get('/NGINX-test', function (req, res) {
  if (savedRequest === null) {
    res.status(404);
    res.json({ error: 'JSON-запрос не найден' });
  } else {
    const answer = makeAnswer(savedRequest);
    res.status(200);
    res.json(answer);
  }
});

app.post('/NGINX-test', function (req, res) {
  if (savedRequest !== null) {
    res.status(409);
    res.json({ error: 'JSON-запрос уже существует' });
  } else {
    savedRequest = req.body;
    const answer = makeAnswer(savedRequest);
    res.status(200);
    res.json(answer);
  }
});

app.put('/NGINX-test', function (req, res) {
  if (savedRequest === null) {
    res.status(404);
    res.json({ error: 'JSON-запрос не найден' });
  } else {
    savedRequest = req.body;
    const answer = makeAnswer(savedRequest);
    res.status(200);
    res.json(answer);
  }
});

app.delete('/NGINX-test', function (req, res) {
  if (savedRequest === null) {
    res.status(404);
    res.json({ error: 'JSON-запрос не найден' });
  } else {
    savedRequest = null;
    res.status(200);
    res.json({ message: 'JSON-запрос удалён' });
  }
});

app.listen(port, function () {
  console.log('Сервер TDWA01-01 запущен на порту ' + port);
});


// Start-Process .\nginx.exe

// .\nginx.exe -s reload

// .\nginx.exe -s quit