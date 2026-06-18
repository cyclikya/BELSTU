import express from 'express';
import { Sequelize, DataTypes } from 'sequelize';   // npm install express sequelize sqlite3

const app = express();
app.use(express.json());

// 1. Подключение к базе (SQLite — файл создастся сам)
const sequelize = new Sequelize({
  dialect: 'sqlite',
  storage: './database.sqlite'
});

// 2. Модель — описание таблицы "User"
const User = sequelize.define('User', {
  name: { type: DataTypes.STRING, allowNull: false },
  age:  { type: DataTypes.INTEGER }
});

// 3. Создаём таблицы в базе по моделям
await sequelize.sync();

// CREATE — добавить запись
app.post('/users', async (req, res) => {
  const user = await User.create({ name: req.body.name, age: req.body.age });
  res.json(user);
});

// READ — получить все записи
app.get('/users', async (req, res) => {
  const users = await User.findAll();
  res.json(users);
});

// UPDATE — изменить запись по id
app.put('/users/:id', async (req, res) => {
  await User.update({ name: req.body.name }, { where: { id: req.params.id } });
  res.json({ ok: true });
});

// DELETE — удалить запись по id
app.delete('/users/:id', async (req, res) => {
  await User.destroy({ where: { id: req.params.id } });
  res.json({ ok: true });
});

app.listen(3000, () => console.log('http://localhost:3000'));











// 1. CREATE — добавить пользователя
//      Метод: POST
//      URL:   http://localhost:3000/users
//      Body → raw → JSON:
//        { "name": "Иван", "age": 20 }
//      Ответ: созданный объект с id

//   ----------------------------------------
//   2. READ — получить всех
//      Метод: GET
//      URL:   http://localhost:3000/users
//      Ответ: массив всех записей

//   ----------------------------------------
//   3. UPDATE — изменить по id
//      Метод: PUT
//      URL:   http://localhost:3000/users/1
//      Body → raw → JSON:
//        { "name": "Иван Петров" }
//      Ответ: { "ok": true }

//   ----------------------------------------
//   4. DELETE — удалить по id
//      Метод: DELETE
//      URL:   http://localhost:3000/users/1
//      Ответ: { "ok": true }