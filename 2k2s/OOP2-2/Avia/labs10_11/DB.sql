-- Создание таблицы Departments
CREATE TABLE IF NOT EXISTS "Departments" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(100) NOT NULL
);

-- Создание таблицы Persons
CREATE TABLE IF NOT EXISTS "Persons" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Age" INT NOT NULL
);

-- Заполнение таблицы Departments начальными данными
INSERT INTO "Departments" ("Title") VALUES
('Отдел разработки'),
('Бухгалтерия'),
('Отдел кадров');

-- Заполнение таблицы Persons начальными данными
INSERT INTO "Persons" ("Name", "Age") VALUES
('Иван Иванов', 28),
('Мария Петрова', 34),
('Алексей Смирнов', 41);
