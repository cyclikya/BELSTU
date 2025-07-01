-- Таблица отделов
CREATE TABLE departments (
    department_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    location VARCHAR(100)
);

-- Таблица сотрудников
CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    department_id INT NOT NULL REFERENCES departments(department_id) ON DELETE CASCADE,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    photo BYTEA, -- поле для хранения фото
    hire_date DATE NOT NULL
);


INSERT INTO departments (name, location) VALUES
('Отдел продаж', 'Москва'),
('Отдел разработки', 'Санкт-Петербург'),
('Отдел маркетинга', 'Новосибирск');

-- Вставка сотрудников с пустым фото (NULL)
INSERT INTO employees (department_id, first_name, last_name, photo, hire_date) VALUES
(1, 'Иван', 'Иванов', NULL, '2020-01-15'),
(1, 'Мария', 'Петрова', NULL, '2019-03-10'),
(2, 'Алексей', 'Сидоров', NULL, '2021-06-01'),
(2, 'Елена', 'Кузнецова', NULL, '2018-11-20'),
(3, 'Дмитрий', 'Васильев', NULL, '2022-02-28');



CREATE OR REPLACE FUNCTION create_two_departments()
RETURNS void AS $$
BEGIN
    -- Проверяем, существуют ли отделы с такими именами, чтобы не дублировать
    IF NOT EXISTS (SELECT 1 FROM departments WHERE name = 'first') THEN
        INSERT INTO departments (name, location) VALUES ('first', 'Москва');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM departments WHERE name = 'second') THEN
        INSERT INTO departments (name, location) VALUES ('second', 'Санкт-Петербург');
    END IF;
END;
$$ LANGUAGE plpgsql;

--ТРИГГЕР
CREATE OR REPLACE FUNCTION log_employee_deletion()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO employee_deletions_log(employee_id) VALUES (OLD.employee_id);
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_log_employee_deletion
BEFORE DELETE ON employees
FOR EACH ROW EXECUTE FUNCTION log_employee_deletion();
