/* =========================================================
   2. ОБЪЕКТНЫЙ ТИП "ЛИЦЕНЗИЯ"
   - дополнительный конструктор
   - MAP метод сравнения
   - метод-функция
   - метод-процедура
========================================================= */

CREATE OR REPLACE TYPE license_obj_t AS OBJECT (
    license_id       NUMBER,
    software_id      NUMBER,
    license_key      VARCHAR2(255),
    license_type     VARCHAR2(100),
    purchase_date    DATE,
    expiration_date  DATE,
    total_seats      NUMBER,
    used_seats       NUMBER,
    cost             NUMBER(12,2),

    CONSTRUCTOR FUNCTION license_obj_t(
        p_license_id   NUMBER,
        p_software_id  NUMBER,
        p_license_key  VARCHAR2,
        p_total_seats  NUMBER,
        p_used_seats   NUMBER,
        p_cost         NUMBER
    ) RETURN SELF AS RESULT,

    MAP MEMBER FUNCTION free_seats RETURN NUMBER DETERMINISTIC,

    MEMBER FUNCTION get_status RETURN VARCHAR2,

    MEMBER PROCEDURE increase_cost(p_percent NUMBER)
);
/
SHOW ERRORS;

CREATE OR REPLACE TYPE BODY license_obj_t AS

    CONSTRUCTOR FUNCTION license_obj_t(
        p_license_id   NUMBER,
        p_software_id  NUMBER,
        p_license_key  VARCHAR2,
        p_total_seats  NUMBER,
        p_used_seats   NUMBER,
        p_cost         NUMBER
    ) RETURN SELF AS RESULT
    IS
    BEGIN
        SELF.license_id      := p_license_id;
        SELF.software_id     := p_software_id;
        SELF.license_key     := p_license_key;
        SELF.license_type    := 'UNKNOWN';
        SELF.purchase_date   := SYSDATE;
        SELF.expiration_date := ADD_MONTHS(SYSDATE, 12);
        SELF.total_seats     := p_total_seats;
        SELF.used_seats      := p_used_seats;
        SELF.cost            := p_cost;
        RETURN;
    END;

    MAP MEMBER FUNCTION free_seats RETURN NUMBER DETERMINISTIC
    IS
    BEGIN
        RETURN NVL(SELF.total_seats, 0) - NVL(SELF.used_seats, 0);
    END;

    MEMBER FUNCTION get_status RETURN VARCHAR2
    IS
    BEGIN
        IF SELF.expiration_date < TRUNC(SYSDATE) THEN
            RETURN 'EXPIRED';
        ELSIF SELF.expiration_date <= TRUNC(SYSDATE) + 30 THEN
            RETURN 'EXPIRING_SOON';
        ELSE
            RETURN 'ACTIVE';
        END IF;
    END;

    MEMBER PROCEDURE increase_cost(p_percent NUMBER)
    IS
    BEGIN
        SELF.cost := ROUND(NVL(SELF.cost, 0) * (1 + p_percent / 100), 2);
    END;

END;
/
SHOW ERRORS;

/* =========================================================
   3. ОБЪЕКТНЫЙ ТИП "МЕСТО ИСПОЛЬЗОВАНИЯ"
   - дополнительный конструктор
   - ORDER метод сравнения
   - метод-функция
   - метод-процедура
========================================================= */

CREATE OR REPLACE TYPE usage_place_obj_t AS OBJECT (
    assignment_id   NUMBER,
    license_id      NUMBER,
    employee_id     NUMBER,
    assigned_date   DATE,
    status          VARCHAR2(50),
    device_type     VARCHAR2(100),
    device_name     VARCHAR2(100),

    CONSTRUCTOR FUNCTION usage_place_obj_t(
        p_assignment_id NUMBER,
        p_license_id    NUMBER,
        p_employee_id   NUMBER,
        p_device_type   VARCHAR2,
        p_device_name   VARCHAR2
    ) RETURN SELF AS RESULT,

    ORDER MEMBER FUNCTION compare_to(p_other usage_place_obj_t) RETURN INTEGER,

    MEMBER FUNCTION get_place_info RETURN VARCHAR2,

    MEMBER PROCEDURE close_usage
);
/
SHOW ERRORS;

CREATE OR REPLACE TYPE BODY usage_place_obj_t AS

    CONSTRUCTOR FUNCTION usage_place_obj_t(
        p_assignment_id NUMBER,
        p_license_id    NUMBER,
        p_employee_id   NUMBER,
        p_device_type   VARCHAR2,
        p_device_name   VARCHAR2
    ) RETURN SELF AS RESULT
    IS
    BEGIN
        SELF.assignment_id := p_assignment_id;
        SELF.license_id    := p_license_id;
        SELF.employee_id   := p_employee_id;
        SELF.assigned_date := SYSDATE;
        SELF.status        := 'Active';
        SELF.device_type   := p_device_type;
        SELF.device_name   := p_device_name;
        RETURN;
    END;

    ORDER MEMBER FUNCTION compare_to(p_other usage_place_obj_t) RETURN INTEGER
    IS
    BEGIN
        IF SELF.assigned_date < p_other.assigned_date THEN
            RETURN -1;
        ELSIF SELF.assigned_date > p_other.assigned_date THEN
            RETURN 1;
        ELSE
            IF SELF.assignment_id < p_other.assignment_id THEN
                RETURN -1;
            ELSIF SELF.assignment_id > p_other.assignment_id THEN
                RETURN 1;
            ELSE
                RETURN 0;
            END IF;
        END IF;
    END;

    MEMBER FUNCTION get_place_info RETURN VARCHAR2
    IS
    BEGIN
        RETURN 'Employee=' || SELF.employee_id ||
               ', Device=' || SELF.device_type ||
               ' ' || SELF.device_name ||
               ', Status=' || SELF.status;
    END;

    MEMBER PROCEDURE close_usage
    IS
    BEGIN
        SELF.status := 'Inactive';
    END;

END;
/
SHOW ERRORS;

/* =========================================================
   4. ОБЪЕКТНЫЕ ТАБЛИЦЫ
========================================================= */

CREATE TABLE license_obj_tab OF license_obj_t (
    CONSTRAINT pk_license_obj_tab PRIMARY KEY (license_id)
);

SELECT * FROM license_obj_tab;


CREATE TABLE usage_place_obj_tab OF usage_place_obj_t (
    CONSTRAINT pk_usage_place_obj_tab PRIMARY KEY (assignment_id)
);

SELECT * FROM usage_place_obj_tab;

/* =========================================================
   5. КОПИРОВАНИЕ ДАННЫХ ИЗ РЕЛЯЦИОННЫХ ТАБЛИЦ В ОБЪЕКТНЫЕ
========================================================= */

INSERT INTO license_obj_tab
SELECT license_obj_t(
    l.license_id,
    l.software_id,
    l.license_key,
    l.license_type,
    l.purchase_date,
    l.expiration_date,
    l.total_seats,
    l.used_seats,
    l.cost
)
FROM Licenses l;

INSERT INTO usage_place_obj_tab
SELECT usage_place_obj_t(
    a.assignment_id,
    a.license_id,
    a.employee_id,
    a.assigned_date,
    a.status,
    a.device_type,
    a.device_name
)
FROM LicenseAssignments a;

COMMIT;

/* =========================================================
   6. ОБЪЕКТНЫЕ ПРЕДСТАВЛЕНИЯ
========================================================= */

CREATE OR REPLACE VIEW license_obj_view OF license_obj_t
WITH OBJECT IDENTIFIER (license_id)
AS
SELECT license_obj_t(
    l.license_id,
    l.software_id,
    l.license_key,
    l.license_type,
    l.purchase_date,
    l.expiration_date,
    l.total_seats,
    l.used_seats,
    l.cost
)
FROM Licenses l;

CREATE OR REPLACE VIEW usage_place_obj_view OF usage_place_obj_t
WITH OBJECT IDENTIFIER (assignment_id)
AS
SELECT usage_place_obj_t(
    a.assignment_id,
    a.license_id,
    a.employee_id,
    a.assigned_date,
    a.status,
    a.device_type,
    a.device_name
)
FROM LicenseAssignments a;

/* =========================================================
   7. ИНДЕКСЫ
   - по атрибуту
   - по методу
========================================================= */

CREATE INDEX idx_license_obj_cost
ON license_obj_tab x (x.cost);

CREATE INDEX idx_license_obj_free_seats
ON license_obj_tab x (x.free_seats());

/* =========================================================
   8. ДЕМОНСТРАЦИЯ РАБОТЫ
========================================================= */

-- 8.1. Просмотр объектной таблицы
SELECT x.license_id,
       x.license_key,
       x.total_seats,
       x.used_seats,
       x.free_seats() AS free_seats,
       x.get_status() AS status_text
FROM license_obj_tab x
ORDER BY VALUE(x);

-- 8.2. Просмотр мест использования
SELECT x.assignment_id,
       x.license_id,
       x.employee_id,
       x.assigned_date,
       x.get_place_info() AS info
FROM usage_place_obj_tab x
ORDER BY VALUE(x);

-- 8.3. Использование объектного представления
SELECT x.license_id,
       x.license_key,
       x.get_status() AS status_text
FROM license_obj_view x
WHERE x.free_seats() > 10
ORDER BY VALUE(x);

-- 8.4. Демонстрация дополнительного конструктора
SELECT license_obj_t(999, 2, 'TEST-KEY', 50, 5, 1234.50) AS sample_license
FROM dual;

SELECT usage_place_obj_t(999, 1, 4, 'Laptop', 'Test Device') AS sample_place
FROM dual;

-- 8.5. Демонстрация метода-процедуры для лицензии
DECLARE
    v_obj license_obj_t;
BEGIN
    SELECT VALUE(x)
    INTO v_obj
    FROM license_obj_tab x
    WHERE x.license_id = 1;

    v_obj.increase_cost(10);

    UPDATE license_obj_tab x
    SET VALUE(x) = v_obj
    WHERE x.license_id = 1;

    COMMIT;
END;
/

SELECT x.license_id, x.license_key, x.cost
FROM license_obj_tab x
WHERE x.license_id = 1;

-- 8.6. Демонстрация метода-процедуры для места использования
DECLARE
    v_place usage_place_obj_t;
BEGIN
    SELECT VALUE(x)
    INTO v_place
    FROM usage_place_obj_tab x
    WHERE x.assignment_id = 1;

    v_place.close_usage;

    UPDATE usage_place_obj_tab x
    SET VALUE(x) = v_place
    WHERE x.assignment_id = 1;

    COMMIT;
END;
/

SELECT x.assignment_id, x.status, x.get_place_info()
FROM usage_place_obj_tab x
WHERE x.assignment_id = 1;