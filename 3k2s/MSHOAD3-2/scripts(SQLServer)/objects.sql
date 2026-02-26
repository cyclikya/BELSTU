/* =========================================
   ПРЕДСТАВЛЕНИЯ
========================================= */

-- Отчет по использованию лицензий: показывает занятые и свободные места по каждому ПО
CREATE VIEW vw_license_usage AS
SELECT 
    l.license_id,
    s.name AS software_name,
    l.total_seats,
    l.used_seats,
    (l.total_seats - l.used_seats) AS free_seats
FROM Licenses l
JOIN Software s ON l.software_id = s.software_id;
GO

-- Список лицензий сотрудников: показывает какое ПО назначено каждому сотруднику
CREATE VIEW vw_employee_licenses AS
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
GO

-- Список активных заявок: показывает все заявки на ПО со статусом NEW
CREATE VIEW vw_active_requests AS
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
GO

/* =========================================
   ФУНКЦИЯ
========================================= */

-- Возвращает количество свободных мест по указанной лицензии
CREATE FUNCTION fn_free_seats (@license_id INT)
RETURNS INT
AS
BEGIN
    DECLARE @free INT;

    SELECT @free = total_seats - used_seats
    FROM Licenses
    WHERE license_id = @license_id;

    RETURN @free;
END;
GO

/* =========================================
   ПРОЦЕДУРЫ
========================================= */

-- Добавление поставщика: создает запись о новом поставщике ПО
CREATE PROCEDURE sp_add_vendor
    @vendor_name VARCHAR(200),
    @contact_email VARCHAR(200),
    @phone VARCHAR(50)
AS
BEGIN
    INSERT INTO Vendors (vendor_name, contact_email, phone)
    VALUES (@vendor_name, @contact_email, @phone);
END;
GO

-- Добавление ПО: создает запись о новом программном обеспечении
CREATE PROCEDURE sp_add_software
    @name VARCHAR(200),
    @version VARCHAR(100),
    @category VARCHAR(100),
    @vendor_id INT
AS
BEGIN
    INSERT INTO Software (name, version, category, vendor_id)
    VALUES (@name, @version, @category, @vendor_id);
END;
GO

-- Добавление лицензии: создает запись о новой лицензии на ПО
CREATE PROCEDURE sp_add_license
    @software_id INT,
    @license_key VARCHAR(255),
    @license_type VARCHAR(100),
    @purchase_date DATE,
    @expiration_date DATE,
    @total_seats INT,
    @cost DECIMAL(12,2)
AS
BEGIN
    INSERT INTO Licenses
    (software_id, license_key, license_type, purchase_date, expiration_date, total_seats, used_seats, cost)
    VALUES
    (@software_id, @license_key, @license_type, @purchase_date, @expiration_date, @total_seats, 0, @cost);
END;
GO

-- Добавление сотрудника: создает запись о новом сотруднике
CREATE PROCEDURE sp_add_employee
    @full_name VARCHAR(200),
    @email VARCHAR(200),
    @department_id INT,
    @position VARCHAR(100)
AS
BEGIN
    INSERT INTO Employees (full_name, email, department_id, position)
    VALUES (@full_name, @email, @department_id, @position);
END;
GO

-- Назначение лицензии: создает запись о назначении лицензии сотруднику
CREATE PROCEDURE sp_assign_license
    @license_id INT,
    @employee_id INT
AS
BEGIN
    INSERT INTO LicenseAssignments (license_id, employee_id, assigned_date, status)
    VALUES (@license_id, @employee_id, GETDATE(), 'ACTIVE');
END;
GO

-- Отзыв лицензии: удаляет запись о назначении лицензии
CREATE PROCEDURE sp_revoke_license
    @assignment_id INT
AS
BEGIN
    DELETE FROM LicenseAssignments
    WHERE assignment_id = @assignment_id;
END;
GO

-- Создание заявки: создает новую заявку сотрудника на получение ПО
CREATE PROCEDURE sp_create_request
    @employee_id INT,
    @software_id INT
AS
BEGIN
    INSERT INTO Requests (employee_id, software_id, request_date, status)
    VALUES (@employee_id, @software_id, GETDATE(), 'NEW');
END;
GO

-- Формирование записи отчета: создает запись о сгенерированном отчете
CREATE PROCEDURE sp_generate_report
    @report_type VARCHAR(200),    
    @generated_by INT              
AS
BEGIN
    INSERT INTO Reports (report_type, created_at, generated_by)
    VALUES (@report_type, GETDATE(), @generated_by);
END;
GO

/* =========================================
   ТРИГГЕРЫ
========================================= */

-- Поддержка used_seats: автоматически обновляет счетчик использованных мест при назначении/отзыве лицензии
CREATE TRIGGER trg_update_used_seats
ON LicenseAssignments
AFTER INSERT, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        UPDATE l
        SET used_seats = used_seats + 1
        FROM Licenses l
        INNER JOIN inserted i ON l.license_id = i.license_id;
    END
    
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        UPDATE l
        SET used_seats = used_seats - 1
        FROM Licenses l
        INNER JOIN deleted d ON l.license_id = d.license_id;
    END
END;
GO

-- Запрет превышения total_seats: проверяет наличие свободных мест перед назначением лицензии
CREATE TRIGGER trg_check_seat_limit
ON LicenseAssignments
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN Licenses l ON i.license_id = l.license_id
        WHERE l.used_seats >= l.total_seats
    )
    BEGIN
        RAISERROR('Превышено количество доступных лицензий', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    INSERT INTO LicenseAssignments (license_id, employee_id, assigned_date, status)
    SELECT license_id, employee_id, assigned_date, status
    FROM inserted;
END;
GO

-- Запрет удаления поставщика при наличии ПО: предотвращает удаление поставщика, на которого ссылается ПО
CREATE TRIGGER trg_prevent_delete_vendor
ON Vendors
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (
        SELECT 1
        FROM Software s
        INNER JOIN deleted d ON s.vendor_id = d.vendor_id
    )
    BEGIN
        RAISERROR('Нельзя удалить поставщика с существующим ПО', 16, 1);
        RETURN;
    END

    DELETE FROM Vendors
    WHERE vendor_id IN (SELECT vendor_id FROM deleted);
END;
GO

-- Запрет удаления лицензии при назначениях: предотвращает удаление лицензии, на которую есть назначения
CREATE TRIGGER trg_prevent_delete_license
ON Licenses
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (
        SELECT 1
        FROM LicenseAssignments la
        INNER JOIN deleted d ON la.license_id = d.license_id
    )
    BEGIN
        RAISERROR('Нельзя удалить лицензию с назначениями', 16, 1);
        RETURN;
    END

    DELETE FROM Licenses
    WHERE license_id IN (SELECT license_id FROM deleted);
END;
GO

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
GO