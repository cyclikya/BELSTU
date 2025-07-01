-- Функция для хеширования пароля
CREATE OR REPLACE FUNCTION hash_password(password VARCHAR)
RETURNS VARCHAR AS $$
DECLARE
    hash_bytes BYTEA;
    hash_text VARCHAR;
BEGIN
    hash_bytes := digest(password::text, 'sha256');
    hash_text := encode(hash_bytes, 'base64');
    IF length(hash_text) > 60 THEN
        hash_text := substring(hash_text FROM 1 FOR 60);
    END IF;
    RETURN hash_text;
END;
$$ LANGUAGE plpgsql;
CREATE EXTENSION IF NOT EXISTS pgcrypto;


CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Login" VARCHAR(100) NOT NULL UNIQUE,
    "Password" VARCHAR(255) NOT NULL,
    "Role" VARCHAR(50) NOT NULL
);

CREATE TABLE "Flights" (
    "Id" SERIAL PRIMARY KEY,
    "Departure" VARCHAR(100) NOT NULL,
    "Destination" VARCHAR(100) NOT NULL,
    "Date" TIMESTAMPTZ NOT NULL,
    "Airline" VARCHAR(100) NOT NULL,
    "Price" DECIMAL(10,2) NOT NULL,
    "SeatsTotal" INT NOT NULL,
    "SeatsAvailable" INT NOT NULL,
    "BaggageInfo" VARCHAR(100) NOT NULL
);

CREATE TABLE "Bookings" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
	"FlightId" INT NOT NULL REFERENCES "Flights"("Id") ON DELETE CASCADE,
    "BookingDate" TIMESTAMPTZ NOT NULL,
    "Status" VARCHAR(50) NOT NULL,
    "SeatsReserved" INT NOT NULL
);

-- Добавление администратора и пользователей
INSERT INTO "Users" ("Id", "Login", "Password", "Role") VALUES
('1', 'admin', hash_password('1234'::text), 'admin'),
('2', 'user1', hash_password('password1'::text), 'client'),
('3', 'user2', hash_password('password2'::text), 'client'),
('4', 'user3', hash_password('password3'::text), 'client'),
('5', 'user4', hash_password('password4'::text), 'client'),
('6', 'user5', hash_password('password5'::text), 'client'),
('7', 'user6', hash_password('password6'::text), 'client'),
('8', 'user7', hash_password('password7'::text), 'client'),
('9', 'user8', hash_password('password8'::text), 'client'),
('10', 'user9', hash_password('password9'::text), 'client');

-- Добавление рейсов (все в мае 2025, цены в долларах)
INSERT INTO "Flights" ("Id", "Departure", "Destination", "Date", "Airline", "Price", "SeatsTotal", "SeatsAvailable", "BaggageInfo") VALUES
(1, 'Москва', 'Санкт-Петербург', '2025-05-01 08:00:00', 'Аэрофлот', 50.00, 100, 100, '10 кг'),
(2, 'Москва', 'Новосибирск', '2025-05-02 10:30:00', 'S7 Airlines', 100.00, 80, 80, '15 кг'),
(3, 'Сочи', 'Москва', '2025-05-03 14:00:00', 'Utair', 80.00, 90, 90, '20 кг'),
(4, 'Екатеринбург', 'Казань', '2025-05-04 16:45:00', 'Победа', 60.00, 120, 120, '5 кг'),
(5, 'Калининград', 'Москва', '2025-05-05 06:30:00', 'Россия', 70.00, 75, 75, '10 кг'),
(6, 'Москва', 'Краснодар', '2025-05-06 09:15:00', 'Аэрофлот', 80.00, 85, 85, '20 кг'),
(7, 'Санкт-Петербург', 'Сочи', '2025-05-07 12:40:00', 'S7 Airlines', 90.00, 95, 95, '15 кг'),
(8, 'Казань', 'Москва', '2025-05-08 18:20:00', 'Utair', 65.00, 88, 88, '10 кг'),
(9, 'Новосибирск', 'Красноярск', '2025-05-09 11:10:00', 'Победа', 50.00, 110, 110, '7 кг'),
(10, 'Владивосток', 'Москва', '2025-05-10 22:00:00', 'Аэрофлот', 170.00, 60, 60, '20 кг'),
(11, 'Москва', 'Париж', '2025-05-11 07:30:00', 'Air France', 350.00, 200, 200, '23 кг'),
(12, 'Москва', 'Нью-Йорк', '2025-05-12 12:15:00', 'Delta', 600.00, 180, 180, '2x23 кг'),
(13, 'Санкт-Петербург', 'Лондон', '2025-05-13 09:45:00', 'British Airways', 400.00, 150, 150, '23 кг'),
(14, 'Сочи', 'Дубай', '2025-05-14 16:20:00', 'Emirates', 450.00, 220, 220, '30 кг'),
(15, 'Казань', 'Стамбул', '2025-05-15 13:10:00', 'Turkish Airlines', 300.00, 170, 170, '20 кг');

-- Добавление бронирований (по 3-5 на каждого пользователя)
INSERT INTO "Bookings" ("Id", "UserId", "FlightId", "BookingDate", "Status", "SeatsReserved") VALUES
-- Бронирования для user1
('1', 2, 1, '2025-04-01 10:00:00', 'paid', 2),
('2', 2, 5, '2025-04-02 11:30:00', 'booked', 1),
('3', 2, 11, '2025-04-03 09:15:00', 'paid', 3),
-- Бронирования для user2
('4', 3, 2, '2025-04-01 14:20:00', 'paid', 1),
('5', 3, 7, '2025-04-02 16:45:00', 'booked', 2),
('6', 3, 12, '2025-04-04 10:30:00', 'cancelled', 4),
-- Бронирования для user3
('7', 4, 3, '2025-04-02 08:15:00', 'paid', 2),
('8', 4, 8, '2025-04-03 12:40:00', 'paid', 1),
('9', 4, 13, '2025-04-05 15:20:00', 'booked', 3),
-- Бронирования для user4
('10', 5, 4, '2025-04-01 09:30:00', 'paid', 1),
('11', 5, 9, '2025-04-03 11:50:00', 'booked', 2),
('12', 5, 14, '2025-04-04 14:10:00', 'paid', 1),
-- Бронирования для user5
('13', 6, 5, '2025-04-02 10:20:00', 'paid', 2),
('14', 6, 10, '2025-04-03 13:45:00', 'booked', 1),
('15', 6, 15, '2025-04-05 16:30:00', 'paid', 3),
-- Бронирования для user6
('16', 7, 1, '2025-04-01 11:15:00', 'paid', 1),
('17', 7, 6, '2025-04-02 14:30:00', 'paid', 2),
('18', 7, 11, '2025-04-04 10:20:00', 'booked', 1),
-- Бронирования для user7
('19', 8, 2, '2025-04-01 15:40:00', 'paid', 3),
('20', 8, 7, '2025-04-03 09:25:00', 'paid', 1),
('21', 8, 12, '2025-04-04 12:50:00', 'booked', 2),
-- Бронирования для user8
('22', 9, 3, '2025-04-02 10:10:00', 'paid', 1),
('23', 9, 8, '2025-04-03 13:35:00', 'paid', 1),
('24', 9, 13, '2025-04-05 16:20:00', 'booked', 2),
-- Бронирования для user9
('25', 10, 4, '2025-04-01 12:25:00', 'paid', 2),
('26', 10, 9, '2025-04-03 14:50:00', 'paid', 1),
('27', 10, 14, '2025-04-04 17:30:00', 'booked', 3);

Drop table "Flights";
drop FUNCTION hash_password;

-- -- Очистка таблиц
-- TRUNCATE TABLE "Bookings" CASCADE;
-- TRUNCATE TABLE "Users" CASCADE;
-- TRUNCATE TABLE "Flights" CASCADE;

-- Проверка данных
SELECT * FROM "Users";
SELECT * FROM "Flights";
SELECT * FROM "Bookings";

-------------------------------------------------------
--ТРИГГЕРЫ
-------------------------------------------------------
CREATE OR REPLACE FUNCTION update_seats_on_booking_cancel()
RETURNS TRIGGER AS $$
BEGIN
    -- Проверяем, что статус изменился и новый статус = 'cancelled'
    IF TG_OP = 'UPDATE' AND NEW."Status" = 'cancelled' AND OLD."Status" <> 'cancelled' THEN
        UPDATE "Flights"
        SET "SeatsAvailable" = "SeatsAvailable" + NEW."SeatsReserved"
        WHERE "Id" = NEW."FlightId";
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
CREATE OR REPLACE TRIGGER trg_update_seats_after_booking_cancel
AFTER UPDATE ON "Bookings"
FOR EACH ROW
EXECUTE FUNCTION update_seats_on_booking_cancel();


CREATE OR REPLACE FUNCTION delete_bookings_on_flight_delete()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM "Bookings" WHERE "FlightId" = OLD."Id";
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;
CREATE OR REPLACE TRIGGER trg_delete_bookings_on_flight_delete
AFTER DELETE ON "Flights"
FOR EACH ROW
EXECUTE FUNCTION delete_bookings_on_flight_delete();


CREATE OR REPLACE FUNCTION update_seats_on_booking_delete()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE "Flights"
    SET "SeatsAvailable" = "SeatsAvailable" + OLD."SeatsReserved"
    WHERE "Id" = OLD."FlightId";
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;
CREATE OR REPLACE TRIGGER trg_update_seats_after_booking_delete
AFTER DELETE ON "Bookings"
FOR EACH ROW
EXECUTE FUNCTION update_seats_on_booking_delete();


CREATE OR REPLACE FUNCTION delete_bookings_on_user_delete()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM "Bookings" WHERE "UserId" = OLD."Id";
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;
CREATE OR REPLACE TRIGGER trg_delete_bookings_on_user_delete
AFTER DELETE ON "Users"
FOR EACH ROW
EXECUTE FUNCTION delete_bookings_on_user_delete();


CREATE OR REPLACE FUNCTION update_seats_on_booking_insert()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE "Flights"
    SET "SeatsAvailable" = "SeatsAvailable" - NEW."SeatsReserved"
    WHERE "Id" = NEW."FlightId";
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
CREATE OR REPLACE TRIGGER trg_update_seats_after_booking_insert
AFTER INSERT ON "Bookings"
FOR EACH ROW
EXECUTE FUNCTION update_seats_on_booking_insert();


CREATE OR REPLACE FUNCTION update_seats_on_booking_update()
RETURNS TRIGGER AS $$
BEGIN
    IF OLD."SeatsReserved" != NEW."SeatsReserved" THEN
        UPDATE "Flights"
        SET "SeatsAvailable" = "SeatsAvailable" + OLD."SeatsReserved"
        WHERE "Id" = OLD."FlightId";
        
        UPDATE "Flights"
        SET "SeatsAvailable" = "SeatsAvailable" - NEW."SeatsReserved"
        WHERE "Id" = NEW."FlightId";
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
CREATE OR REPLACE TRIGGER trg_update_seats_after_booking_update
AFTER UPDATE ON "Bookings"
FOR EACH ROW
EXECUTE FUNCTION update_seats_on_booking_update();
