import http from 'http';
import sqlite3 from 'sqlite3';   // npm install sqlite3

const db = new sqlite3.Database('./base.db');

// создаём таблицу
db.run('CREATE TABLE IF NOT EXISTS faculty (id TEXT, name TEXT)');

http.createServer((req, res) => {
  const { method, url } = req;
  res.setHeader('Content-Type', 'application/json; charset=utf-8');

  // GET — выбрать все
  if (method === 'GET') {
    db.all('SELECT * FROM faculty', (err, rows) => {
      res.end(JSON.stringify(rows));
    });
    return;
  }

  // POST — добавить (данные из тела)
  if (method === 'POST') {
    let body = '';
    req.on('data', chunk => body += chunk);
    req.on('end', () => {
      const data = JSON.parse(body);
      db.run('INSERT INTO faculty VALUES (?, ?)', [data.id, data.name], () => {
        res.end(JSON.stringify({ ok: true }));
      });
    });
    return;
  }

  // DELETE — удалить по id (из тела)
  if (method === 'DELETE') {
    let body = '';
    req.on('data', chunk => body += chunk);
    req.on('end', () => {
      const data = JSON.parse(body);
      db.run('DELETE FROM faculty WHERE id = ?', [data.id], () => {
        res.end(JSON.stringify({ ok: true }));
      });
    });
  }
}).listen(3000);