--Задание 4

PRAGMA foreign_keys = ON;
-- 4.1 УСПЕШНОЕ ДОБАВЛЕНИЕ
INSERT INTO LicenseAssignments
(license_id, employee_id, assigned_date, status, device_type, device_name)
VALUES (1, 1, '2025-06-01', 'Active', 'Laptop', 'ThinkPad T14');

SELECT * FROM LicenseAssignments WHERE employee_id = 1;


PRAGMA foreign_keys = ON;
-- 4.2 ПОПЫТКА ДОБАВИТЬ С НЕСУЩЕСТВУЮЩЕЙ ЛИЦЕНЗИЕЙ
INSERT INTO LicenseAssignments
(license_id, employee_id, assigned_date, status, device_type, device_name)
VALUES (999, 1, '2025-06-01', 'Active', 'Laptop', 'Test Device');


-- 4.4 УСПЕШНОЕ ОБНОВЛЕНИЕ
SELECT * FROM LicenseAssignments WHERE assignment_id = 1;

PRAGMA foreign_keys = ON;
UPDATE LicenseAssignments
SET status = 'Revoked', device_name = 'RETURNED'
WHERE assignment_id = 1;

SELECT * FROM LicenseAssignments WHERE assignment_id = 1;


PRAGMA foreign_keys = ON;
-- 4.5 ПОПЫТКА ОБНОВИТЬ license_id НА НЕСУЩЕСТВУЮЩИЙ
UPDATE LicenseAssignments SET license_id = 999 WHERE assignment_id = 2;


-- 4.6 УДАЛЕНИЕ ИЗ ПОДЧИНЁННОЙ ТАБЛИЦЫ
SELECT COUNT(*) AS before_count FROM LicenseAssignments;

PRAGMA foreign_keys = ON;
DELETE FROM LicenseAssignments
WHERE employee_id = 1 AND license_id = 1;

SELECT COUNT(*) AS after_count FROM LicenseAssignments;

-- Задание 5
-- Представление Использование лицензий
CREATE VIEW IF NOT EXISTS vw_license_usage AS
SELECT
    l.license_id,
    s.name        AS software_name,
    l.license_key,
    l.total_seats,
    l.used_seats,
    (l.total_seats - l.used_seats) AS free_seats
FROM Licenses l
JOIN Software s ON l.software_id = s.software_id;

SELECT * FROM vw_license_usage;


-- Задание 6
-- Обычный индекс: ускоряет JOIN по license_id
CREATE INDEX IF NOT EXISTS idx_assign_license
ON LicenseAssignments(license_id);

-- Частичный индекс: индексирует ТОЛЬКО активные назначения
CREATE INDEX IF NOT EXISTS idx_assign_active
ON LicenseAssignments(status)
WHERE status = 'Active';

-- Составной индекс: vendor_id + category
CREATE INDEX IF NOT EXISTS idx_soft_vendor
ON Software(vendor_id, category);

EXPLAIN QUERY PLAN
SELECT * FROM LicenseAssignments WHERE license_id = 2;

EXPLAIN QUERY PLAN
SELECT * FROM LicenseAssignments WHERE status = 'Active';


-- Задание 7
-- Триггер: автоматически увеличивает used_seats в Licenses
CREATE TRIGGER IF NOT EXISTS trg_after_insert_assignment
AFTER INSERT ON LicenseAssignments
BEGIN
    UPDATE Licenses
    SET used_seats = used_seats + 1
    WHERE license_id = NEW.license_id;
END;

SELECT license_id, used_seats, total_seats FROM Licenses WHERE license_id = 4;

PRAGMA foreign_keys = ON;
INSERT INTO LicenseAssignments (license_id, employee_id, assigned_date, status, device_type, device_name)
	VALUES (4, 3, '2025-06-20', 'Active', 'Laptop', 'Trigger Test Laptop');

-- Смотрим used_seats ПОСЛЕ — должен увеличиться на 1
SELECT license_id, used_seats, total_seats
FROM Licenses WHERE license_id = 4;