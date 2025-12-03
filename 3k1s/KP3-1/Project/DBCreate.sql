DROP SCHEMA IF EXISTS avia CASCADE;
CREATE SCHEMA avia;
SET search_path TO avia;

-- ENUM
CREATE TYPE role_type AS ENUM ('admin', 'client');
CREATE TYPE class_type AS ENUM ('economy', 'business');
CREATE TYPE ticket_status AS ENUM ('active', 'cancelled');

-- пользователи
CREATE TABLE Users (
    UserID SERIAL PRIMARY KEY,
	Pass VARCHAR(255) NOT NULL,
    PassportNumber VARCHAR(20) UNIQUE NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    MiddleName VARCHAR(50),
    AccessRole role_type DEFAULT 'client' NOT NULL,
    BirthDate DATE NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    LastLogin TIMESTAMP
);

-- рейсы
CREATE TABLE Flights (
    FlightID SERIAL PRIMARY KEY,
    DepartureCity VARCHAR(100) NOT NULL,
    ArrivalCity VARCHAR(100) NOT NULL,
    DepartureDate DATE NOT NULL,
    DepartureTime TIME NOT NULL,
    ArrivalDate DATE NOT NULL,
    ArrivalTime TIME NOT NULL,
    Airline VARCHAR(100) NOT NULL,
    EconomyPrice DECIMAL(10,2) NOT NULL CHECK (EconomyPrice > 0),
    BusinessPrice DECIMAL(10,2) NOT NULL CHECK (BusinessPrice > 0),
    EconomySeats INT NOT NULL CHECK (EconomySeats >= 0),
    BusinessSeats INT NOT NULL CHECK (BusinessSeats >= 0),
    BaggagePrice DECIMAL(10,2) DEFAULT 0 CHECK (BaggagePrice >= 0)
);

-- билеты
CREATE TABLE Tickets (
    TicketID SERIAL PRIMARY KEY,
    FlightID INT NOT NULL REFERENCES Flights(FlightID) ON DELETE CASCADE,
    UserID INT NOT NULL REFERENCES Users(UserID) ON DELETE CASCADE,
    ClassType class_type NOT NULL,
    Baggage BOOLEAN DEFAULT FALSE,
    PurchaseDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status ticket_status DEFAULT 'active' NOT NULL
);

-- функции
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


-- Возвращает количество доступных мест по рейсу и классу
CREATE OR REPLACE FUNCTION fn_GetAvailableSeats(p_FlightID INT, p_Class class_type)
RETURNS INT AS $$
DECLARE
    total_seats INT;
    sold INT;
BEGIN
    IF p_Class = 'economy' THEN
        SELECT EconomySeats INTO total_seats FROM Flights WHERE FlightID = p_FlightID;
        SELECT COUNT(*) INTO sold FROM Tickets WHERE FlightID = p_FlightID AND ClassType = 'economy' AND Status='active';
    ELSE
        SELECT BusinessSeats INTO total_seats FROM Flights WHERE FlightID = p_FlightID;
        SELECT COUNT(*) INTO sold FROM Tickets WHERE FlightID = p_FlightID AND ClassType = 'business' AND Status='active';
    END IF;
    RETURN total_seats - sold;
END;
$$ LANGUAGE plpgsql;

-- Рассчитать итоговую цену
CREATE OR REPLACE FUNCTION fn_GetFlightPrice(p_FlightID INT, p_Class class_type, p_Baggage BOOLEAN)
RETURNS DECIMAL AS $$
DECLARE
    base_price DECIMAL;
    bag_price DECIMAL;
BEGIN
    SELECT CASE WHEN p_Class = 'economy' THEN EconomyPrice ELSE BusinessPrice END,
           BaggagePrice
    INTO base_price, bag_price
    FROM Flights WHERE FlightID = p_FlightID;
    RETURN base_price + (CASE WHEN p_Baggage THEN bag_price ELSE 0 END);
END;
$$ LANGUAGE plpgsql;

-- Проверка возраста (например, 18+)
CREATE OR REPLACE FUNCTION fn_CheckAge(p_Birth DATE)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN (AGE(CURRENT_DATE, p_Birth)).year >= 18;
END;
$$ LANGUAGE plpgsql;

-- Продолжительность рейса
CREATE OR REPLACE FUNCTION fn_GetFlightDuration(p_FlightID INT)
RETURNS INTERVAL AS $$
DECLARE
    dep TIMESTAMP;
    arr TIMESTAMP;
BEGIN
    SELECT DepartureDate + DepartureTime, ArrivalDate + ArrivalTime
    INTO dep, arr
    FROM Flights WHERE FlightID = p_FlightID;
    RETURN arr - dep;
END;
$$ LANGUAGE plpgsql;

--процедуры

-- Создать пользователя
CREATE OR REPLACE PROCEDURE sp_CreateUser(
    p_Passport VARCHAR,
    p_Last VARCHAR,
    p_First VARCHAR,
    p_Middle VARCHAR,
    p_Role role_type,
    p_Birth DATE
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF EXISTS(SELECT 1 FROM Users WHERE PassportNumber = p_Passport) THEN
        RAISE EXCEPTION 'Пользователь с таким паспортом уже существует.';
    END IF;

    IF NOT fn_CheckAge(p_Birth) THEN
        RAISE EXCEPTION 'Пользователь должен быть старше 18 лет.';
    END IF;

    INSERT INTO Users(PassportNumber, LastName, FirstName, MiddleName, AccessRole, BirthDate)
    VALUES(p_Passport, p_Last, p_First, p_Middle, COALESCE(p_Role, 'client'), p_Birth);
END;
$$;

-- Создать рейс
CREATE OR REPLACE PROCEDURE sp_CreateFlight(
    p_DepCity VARCHAR,
    p_ArrCity VARCHAR,
    p_DepDate DATE,
    p_DepTime TIME,
    p_ArrDate DATE,
    p_ArrTime TIME,
    p_Airline VARCHAR,
    p_EcoPrice DECIMAL,
    p_BusPrice DECIMAL,
    p_EcoSeats INT,
    p_BusSeats INT,
    p_BagPrice DECIMAL
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF (p_ArrDate + p_ArrTime) <= (p_DepDate + p_DepTime) THEN
        RAISE EXCEPTION 'Дата прибытия должна быть позже даты вылета.';
    END IF;

    INSERT INTO Flights(DepartureCity, ArrivalCity, DepartureDate, DepartureTime, ArrivalDate, ArrivalTime, Airline,
                        EconomyPrice, BusinessPrice, EconomySeats, BusinessSeats, BaggagePrice)
    VALUES(p_DepCity, p_ArrCity, p_DepDate, p_DepTime, p_ArrDate, p_ArrTime, p_Airline,
           p_EcoPrice, p_BusPrice, p_EcoSeats, p_BusSeats, p_BagPrice);
END;
$$;

-- Купить билет
CREATE OR REPLACE PROCEDURE sp_BuyTicket(
    p_UserID INT,
    p_FlightID INT,
    p_Class class_type,
    p_Baggage BOOLEAN
)
LANGUAGE plpgsql
AS $$
DECLARE
    seats_left INT;
BEGIN
    SELECT fn_GetAvailableSeats(p_FlightID, p_Class) INTO seats_left;

    IF seats_left <= 0 THEN
        RAISE EXCEPTION 'Нет свободных мест на данном рейсе.';
    END IF;

    INSERT INTO Tickets(FlightID, UserID, ClassType, Baggage)
    VALUES(p_FlightID, p_UserID, p_Class, p_Baggage);
END;
$$;

-- Отмена билета
CREATE OR REPLACE PROCEDURE sp_CancelTicket(p_TicketID INT)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE Tickets SET Status='cancelled' WHERE TicketID = p_TicketID;
END;
$$;

--триггеры

-- Проверка дат рейса
CREATE OR REPLACE FUNCTION trg_ValidateFlightDates()
RETURNS TRIGGER AS $$
BEGIN
    IF (NEW.ArrivalDate + NEW.ArrivalTime) <= (NEW.DepartureDate + NEW.DepartureTime) THEN
        RAISE EXCEPTION 'Дата прибытия должна быть позже даты вылета.';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER check_flight_dates
BEFORE INSERT OR UPDATE ON Flights
FOR EACH ROW EXECUTE FUNCTION trg_ValidateFlightDates();

-- Обновление LastLogin при изменении
CREATE OR REPLACE FUNCTION trg_UpdateLastLogin()
RETURNS TRIGGER AS $$
BEGIN
    NEW.LastLogin := CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_login_time
BEFORE UPDATE ON Users
FOR EACH ROW
WHEN (OLD.LastLogin IS DISTINCT FROM NEW.LastLogin)
EXECUTE FUNCTION trg_UpdateLastLogin();

-- Запрет удаления админов
CREATE OR REPLACE FUNCTION trg_PreventDeleteAdmin()
RETURNS TRIGGER AS $$
BEGIN
    IF OLD.AccessRole = 'admin' THEN
        RAISE EXCEPTION 'Нельзя удалить администратора.';
    END IF;
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER prevent_delete_admin
BEFORE DELETE ON Users
FOR EACH ROW EXECUTE FUNCTION trg_PreventDeleteAdmin();

-- =========================================
-- 8. ИНДЕКСЫ
-- =========================================
CREATE INDEX idx_flights_cities ON Flights(DepartureCity, ArrivalCity);
CREATE INDEX idx_tickets_user ON Tickets(UserID);
CREATE INDEX idx_tickets_flight ON Tickets(FlightID);

-- =========================================
-- 9. ГОТОВО
-- =========================================
-- Подключение EF Core: можно использовать имя схемы "avia" и таблицы как DbSet
-- Пример строки подключения:
-- Host=localhost;Port=5432;Database=AviaTickets;Username=postgres;Password=vivi5567;
