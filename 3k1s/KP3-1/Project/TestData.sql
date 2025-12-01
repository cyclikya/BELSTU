SELECT * FROM avia.users
ORDER BY userid ASC 

-- =========================================
--   ТЕСТОВЫЕ ДАННЫЕ ДЛЯ БД: AviaTickets
--   Проект: "Авиабилеты"
-- =========================================

SET search_path TO avia;

-- =========================================
-- 1. ОЧИСТКА СУЩЕСТВУЮЩИХ ДАННЫХ (опционально)
-- =========================================
-- Раскомментируйте следующие строки, если нужно очистить данные перед вставкой:
-- DELETE FROM Tickets;
-- DELETE FROM Flights;
-- DELETE FROM Users;

-- =========================================
-- 2. ТЕСТОВЫЕ ПОЛЬЗОВАТЕЛИ
-- =========================================

-- Администратор
-- Пароль: admin123
-- Хеш вычислен через SHA256: admin123 -> Base64
INSERT INTO Users (PassportNumber, Pass, LastName, FirstName, MiddleName, AccessRole, BirthDate, CreatedAt)
VALUES 
('ADMIN001', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', 'Иванов', 'Иван', 'Иванович', 'admin'::role_type, '1980-01-15', CURRENT_TIMESTAMP);

-- Клиенты
-- Пароль: user123
-- Хеш вычислен через SHA256: user123 -> Base64
INSERT INTO Users (PassportNumber, Pass, LastName, FirstName, MiddleName, AccessRole, BirthDate, CreatedAt)
VALUES 
('USER001', '5gbjiw2MGbJM8O44CBgxYup81j/3kS27IrXoAyhrREY=', 'Петров', 'Петр', 'Петрович', 'client'::role_type, '1990-05-20', CURRENT_TIMESTAMP),
('USER002', '5gbjiw2MGbJM8O44CBgxYup81j/3kS27IrXoAyhrREY=', 'Сидорова', 'Анна', 'Сергеевна', 'client'::role_type, '1995-08-12', CURRENT_TIMESTAMP),
('USER003', '5gbjiw2MGbJM8O44CBgxYup81j/3kS27IrXoAyhrREY=', 'Козлов', 'Дмитрий', 'Александрович', 'client'::role_type, '1988-11-30', CURRENT_TIMESTAMP),
('USER004', '5gbjiw2MGbJM8O44CBgxYup81j/3kS27IrXoAyhrREY=', 'Морозова', 'Елена', 'Владимировна', 'client'::role_type, '1992-03-25', CURRENT_TIMESTAMP),
('USER005', '5gbjiw2MGbJM8O44CBgxYup81j/3kS27IrXoAyhrREY=', 'Волков', 'Алексей', 'Николаевич', 'client'::role_type, '1987-07-18', CURRENT_TIMESTAMP);

-- =========================================
-- 3. ТЕСТОВЫЕ РЕЙСЫ
-- =========================================

INSERT INTO Flights (DepartureCity, ArrivalCity, DepartureDate, DepartureTime, ArrivalDate, ArrivalTime, Airline, EconomyPrice, BusinessPrice, EconomySeats, BusinessSeats, BaggagePrice)
VALUES 
-- Рейсы из Минска
('Минск', 'Москва', '2025-12-15', '08:00:00', '2025-12-15', '10:30:00', 'Белавиа', 150.00, 350.00, 120, 30, 25.00),
('Минск', 'Москва', '2025-12-15', '14:30:00', '2025-12-15', '17:00:00', 'Аэрофлот', 180.00, 400.00, 150, 40, 30.00),
('Минск', 'Москва', '2025-12-16', '10:00:00', '2025-12-16', '12:30:00', 'Белавиа', 160.00, 360.00, 120, 30, 25.00),

('Минск', 'Санкт-Петербург', '2025-12-15', '09:30:00', '2025-12-15', '11:45:00', 'Белавиа', 200.00, 450.00, 100, 25, 30.00),
('Минск', 'Санкт-Петербург', '2025-12-16', '16:00:00', '2025-12-16', '18:15:00', 'Аэрофлот', 220.00, 480.00, 120, 30, 35.00),

('Минск', 'Киев', '2025-12-15', '11:00:00', '2025-12-15', '12:30:00', 'Белавиа', 120.00, 280.00, 140, 35, 20.00),
('Минск', 'Киев', '2025-12-16', '18:30:00', '2025-12-16', '20:00:00', 'МАУ', 130.00, 300.00, 130, 30, 25.00),

('Минск', 'Варшава', '2025-12-15', '13:00:00', '2025-12-15', '14:15:00', 'LOT', 180.00, 420.00, 110, 28, 30.00),
('Минск', 'Варшава', '2025-12-16', '19:00:00', '2025-12-16', '20:15:00', 'Белавиа', 190.00, 440.00, 100, 25, 30.00),

('Минск', 'Париж', '2025-12-15', '07:30:00', '2025-12-15', '10:00:00', 'Air France', 350.00, 850.00, 180, 45, 50.00),
('Минск', 'Париж', '2025-12-16', '15:00:00', '2025-12-16', '17:30:00', 'Белавиа', 380.00, 900.00, 170, 40, 55.00),

('Минск', 'Лондон', '2025-12-15', '06:00:00', '2025-12-15', '08:30:00', 'British Airways', 400.00, 950.00, 160, 40, 60.00),
('Минск', 'Лондон', '2025-12-16', '14:00:00', '2025-12-16', '16:30:00', 'Белавиа', 420.00, 980.00, 150, 35, 60.00),

-- Обратные рейсы
('Москва', 'Минск', '2025-12-15', '12:00:00', '2025-12-15', '13:20:00', 'Аэрофлот', 150.00, 350.00, 120, 30, 25.00),
('Москва', 'Минск', '2025-12-16', '18:00:00', '2025-12-16', '19:20:00', 'Белавиа', 160.00, 360.00, 130, 32, 25.00),

('Санкт-Петербург', 'Минск', '2025-12-15', '13:00:00', '2025-12-15', '14:15:00', 'Аэрофлот', 200.00, 450.00, 110, 28, 30.00),
('Санкт-Петербург', 'Минск', '2025-12-16', '19:30:00', '2025-12-16', '20:45:00', 'Белавиа', 210.00, 470.00, 120, 30, 30.00),

('Киев', 'Минск', '2025-12-15', '14:00:00', '2025-12-15', '15:20:00', 'МАУ', 120.00, 280.00, 140, 35, 20.00),
('Киев', 'Минск', '2025-12-16', '21:00:00', '2025-12-16', '22:20:00', 'Белавиа', 130.00, 300.00, 130, 30, 25.00);

-- =========================================
-- 4. ТЕСТОВЫЕ БИЛЕТЫ
-- =========================================

-- Билеты для USER001 (Петров)
INSERT INTO Tickets (FlightID, UserID, ClassType, Baggage, PurchaseDate, Status)
VALUES 
(1, 2, 'economy'::class_type, true, CURRENT_TIMESTAMP - INTERVAL '5 days', 'active'::ticket_status),
(4, 2, 'business'::class_type, true, CURRENT_TIMESTAMP - INTERVAL '3 days', 'active'::ticket_status),
(7, 2, 'economy'::class_type, false, CURRENT_TIMESTAMP - INTERVAL '10 days', 'cancelled'::ticket_status);

-- Билеты для USER002 (Сидорова)
INSERT INTO Tickets (FlightID, UserID, ClassType, Baggage, PurchaseDate, Status)
VALUES 
(2, 3, 'economy'::class_type, true, CURRENT_TIMESTAMP - INTERVAL '2 days', 'active'::ticket_status),
(10, 3, 'business'::class_type, true, CURRENT_TIMESTAMP - INTERVAL '1 day', 'active'::ticket_status);

-- Билеты для USER003 (Козлов)
INSERT INTO Tickets (FlightID, UserID, ClassType, Baggage, PurchaseDate, Status)
VALUES 
(3, 4, 'business'::class_type, true, CURRENT_TIMESTAMP - INTERVAL '7 days', 'active'::ticket_status),
(13, 4, 'economy'::class_type, false, CURRENT_TIMESTAMP - INTERVAL '4 days', 'active'::ticket_status);

-- Билеты для USER004 (Морозова)
INSERT INTO Tickets (FlightID, UserID, ClassType, Baggage, PurchaseDate, Status)
VALUES 
(5, 5, 'economy'::class_type, true, CURRENT_TIMESTAMP - INTERVAL '6 days', 'active'::ticket_status),
(8, 5, 'business'::class_type, true, CURRENT_TIMESTAMP - INTERVAL '8 days', 'cancelled'::ticket_status);

-- Билеты для USER005 (Волков)
INSERT INTO Tickets (FlightID, UserID, ClassType, Baggage, PurchaseDate, Status)
VALUES 
(11, 6, 'business'::class_type, true, CURRENT_TIMESTAMP - INTERVAL '9 days', 'active'::ticket_status),
(14, 6, 'economy'::class_type, true, CURRENT_TIMESTAMP - INTERVAL '11 days', 'active'::ticket_status);

-- =========================================
-- 5. ИНФОРМАЦИЯ О ТЕСТОВЫХ ДАННЫХ
-- =========================================
-- 
-- АДМИНИСТРАТОР:
--   Номер паспорта: ADMIN001
--   Пароль: admin123
--   Роль: admin
--
-- КЛИЕНТЫ (все с одинаковым паролем):
--   Номер паспорта: USER001, USER002, USER003, USER004, USER005
--   Пароль: user123
--   Роль: client
--
-- РЕЙСЫ:
--   - Из Минска в различные города (Москва, Санкт-Петербург, Киев, Варшава, Париж, Лондон)
--   - Обратные рейсы из этих городов в Минск
--   - Разные даты (15 и 16 декабря 2025)
--   - Разные авиакомпании (Белавиа, Аэрофлот, МАУ, LOT, Air France, British Airways)
--   - Разные цены на эконом и бизнес класс
--
-- БИЛЕТЫ:
--   - Разные клиенты имеют билеты на разные рейсы
--   - Есть билеты эконом и бизнес класса
--   - Есть билеты с багажом и без
--   - Есть активные и отмененные билеты
--   - Разные даты покупки
--
-- =========================================

