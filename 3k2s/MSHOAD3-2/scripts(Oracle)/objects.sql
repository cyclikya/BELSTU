/* =========================================
   ПОСЛЕДОВАТЕЛЬНОСТИ (только для Oracle)
========================================= */

-- Генерирует уникальные идентификаторы для назначений лицензий
CREATE SEQUENCE seq_license_assignments START WITH 1 INCREMENT BY 1;

-- Генерирует уникальные идентификаторы для заявок на ПО
CREATE SEQUENCE seq_requests START WITH 1 INCREMENT BY 1;

-- Генерирует уникальные идентификаторы для отчетов
CREATE SEQUENCE seq_reports START WITH 1 INCREMENT BY 1;

/* =========================================
   ПРЕДСТАВЛЕНИЯ
========================================= */

-- Отчет по использованию лицензий: показывает занятые и свободные места по каждому ПО
CREATE OR REPLACE VIEW vw_license_usage AS
SELECT 
    l.license_id,
    s.name AS software_name,
    l.total_seats,
    l.used_seats,
    (l.total_seats - l.used_seats) AS free_seats
FROM Licenses l
JOIN Software s ON l.software_id = s.software_id;

-- Список лицензий сотрудников: показывает какое ПО назначено каждому сотруднику
CREATE OR REPLACE VIEW vw_employee_licenses AS
SELECT 
    e.employee_id,
    e.full_name,
    s.name AS software_name,
    la.assigned_date,
    la.status
FROM LicenseAssignments la
JOIN Employees e ON la.employee_id = e.employee_id
JOIN Licenses l ON la.license_id = l.license_id
JOIN Software s ON l.software_id = s.software_id;

-- Список активных заявок: показывает все заявки на ПО со статусом NEW
CREATE OR REPLACE VIEW vw_active_requests AS
SELECT 
    r.request_id,
    e.full_name,
    s.name AS software_name,
    r.request_date,
    r.status
FROM Requests r
JOIN Employees e ON r.employee_id = e.employee_id
JOIN Software s ON r.software_id = s.software_id
WHERE r.status = 'NEW';

/* =========================================
   ФУНКЦИЯ
========================================= */

-- Возвращает количество свободных мест по указанной лицензии
CREATE OR REPLACE FUNCTION fn_free_seats (p_license_id NUMBER)
RETURN NUMBER
IS
    v_free NUMBER;
BEGIN
    SELECT total_seats - used_seats
    INTO v_free
    FROM Licenses
    WHERE license_id = p_license_id;

    RETURN v_free;
END;
/

/* =========================================
   ПРОЦЕДУРЫ
========================================= */

-- Добавление поставщика: создает запись о новом поставщике ПО
CREATE OR REPLACE PROCEDURE sp_add_vendor (
    p_vendor_name VARCHAR2,
    p_contact_email VARCHAR2,
    p_phone VARCHAR2
)
IS
BEGIN
    INSERT INTO Vendors (vendor_name, contact_email, phone)
    VALUES (p_vendor_name, p_contact_email, p_phone);
END;
/

-- Добавление ПО: создает запись о новом программном обеспечении
CREATE OR REPLACE PROCEDURE sp_add_software (
    p_name VARCHAR2,
    p_version VARCHAR2,
    p_category VARCHAR2,
    p_vendor_id NUMBER
)
IS
BEGIN
    INSERT INTO Software (name, version, category, vendor_id)
    VALUES (p_name, p_version, p_category, p_vendor_id);
END;
/

-- Добавление лицензии: создает запись о новой лицензии на ПО
CREATE OR REPLACE PROCEDURE sp_add_license (
    p_software_id NUMBER,
    p_license_key VARCHAR2,
    p_license_type VARCHAR2,
    p_purchase_date DATE,
    p_expiration_date DATE,
    p_total_seats NUMBER,
    p_cost NUMBER
)
IS
BEGIN
    INSERT INTO Licenses
    (software_id, license_key, license_type, purchase_date, expiration_date, total_seats, used_seats, cost)
    VALUES
    (p_software_id, p_license_key, p_license_type, p_purchase_date, p_expiration_date, p_total_seats, 0, p_cost);
END;
/

-- Добавление сотрудника: создает запись о новом сотруднике
CREATE OR REPLACE PROCEDURE sp_add_employee (
    p_full_name VARCHAR2,
    p_email VARCHAR2,
    p_department_id NUMBER,
    p_position VARCHAR2
)
IS
BEGIN
    INSERT INTO Employees (full_name, email, department_id, position)
    VALUES (p_full_name, p_email, p_department_id, p_position);
END;
/

-- Назначение лицензии: создает запись о назначении лицензии сотруднику
CREATE OR REPLACE PROCEDURE sp_assign_license (
    p_license_id NUMBER,
    p_employee_id NUMBER
)
IS
BEGIN
    INSERT INTO LicenseAssignments
    (assignment_id, license_id, employee_id, assigned_date, status)
    VALUES
    (seq_license_assignments.NEXTVAL, p_license_id, p_employee_id, SYSDATE, 'ACTIVE');
END;
/

-- Отзыв лицензии: удаляет запись о назначении лицензии
CREATE OR REPLACE PROCEDURE sp_revoke_license (
    p_assignment_id NUMBER
)
IS
BEGIN
    DELETE FROM LicenseAssignments
    WHERE assignment_id = p_assignment_id;
END;
/

-- Создание заявки: создает новую заявку сотрудника на получение ПО
CREATE OR REPLACE PROCEDURE sp_create_request (
    p_employee_id NUMBER,
    p_software_id NUMBER
)
IS
BEGIN
    INSERT INTO Requests
    (request_id, employee_id, software_id, request_date, status)
    VALUES
    (seq_requests.NEXTVAL, p_employee_id, p_software_id, SYSDATE, 'NEW');
END;
/

-- Формирование записи отчета: создает запись о сгенерированном отчете
CREATE OR REPLACE PROCEDURE sp_generate_report (
    p_report_type VARCHAR2,    
    p_generated_by NUMBER    
)
IS
BEGIN
    INSERT INTO Reports
        (report_id, report_type, created_at, generated_by)
    VALUES
        (seq_reports.NEXTVAL, p_report_type, SYSDATE, p_generated_by);
    COMMIT;
END;
/

/* =========================================
   ТРИГГЕРЫ
========================================= */

-- Поддержка used_seats: автоматически обновляет счетчик использованных мест при назначении/отзыве лицензии
CREATE OR REPLACE TRIGGER trg_update_used_seats
AFTER INSERT OR DELETE ON LicenseAssignments
FOR EACH ROW
BEGIN
    IF INSERTING THEN
        UPDATE Licenses
        SET used_seats = used_seats + 1
        WHERE license_id = :NEW.license_id;
    ELSIF DELETING THEN
        UPDATE Licenses
        SET used_seats = used_seats - 1
        WHERE license_id = :OLD.license_id;
    END IF;
END;
/

-- Запрет превышения total_seats: проверяет наличие свободных мест перед назначением лицензии
CREATE OR REPLACE TRIGGER trg_check_seat_limit
BEFORE INSERT ON LicenseAssignments
FOR EACH ROW
DECLARE
    v_total NUMBER;
    v_used NUMBER;
BEGIN
    SELECT total_seats, used_seats
    INTO v_total, v_used
    FROM Licenses
    WHERE license_id = :NEW.license_id;

    IF v_used >= v_total THEN
        RAISE_APPLICATION_ERROR(-20001, 'Превышено количество лицензий');
    END IF;
END;
/

-- Запрет удаления поставщика при наличии ПО: предотвращает удаление поставщика, на которого ссылается ПО
CREATE OR REPLACE TRIGGER trg_prevent_delete_vendor
BEFORE DELETE ON Vendors
FOR EACH ROW
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count
    FROM Software
    WHERE vendor_id = :OLD.vendor_id;

    IF v_count > 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Нельзя удалить поставщика с существующим ПО');
    END IF;
END;
/

-- Запрет удаления лицензии при назначениях: предотвращает удаление лицензии, на которую есть назначения
CREATE OR REPLACE TRIGGER trg_prevent_delete_license
BEFORE DELETE ON Licenses
FOR EACH ROW
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count
    FROM LicenseAssignments
    WHERE license_id = :OLD.license_id;

    IF v_count > 0 THEN
        RAISE_APPLICATION_ERROR(-20003, 'Нельзя удалить лицензию с назначениями');
    END IF;
END;
/

/* =========================================
   ИНДЕКСЫ
========================================= */

-- Оптимизация JOIN: ускоряет поиск назначений по идентификатору лицензии
CREATE INDEX idx_license_assignments_license
ON LicenseAssignments(license_id);

-- Оптимизация поиска: ускоряет поиск назначений по идентификатору сотрудника
CREATE INDEX idx_license_assignments_employee
ON LicenseAssignments(employee_id);

-- Быстрый поиск заявок: ускоряет поиск заявок по идентификатору сотрудника
CREATE INDEX idx_requests_employee
ON Requests(employee_id);
/