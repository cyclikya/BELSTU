-- вкл внешние ключи
PRAGMA foreign_keys = ON;

-- Поставщики ПО
CREATE TABLE IF NOT EXISTS Vendors (
    vendor_id     INTEGER PRIMARY KEY AUTOINCREMENT,
    vendor_name   TEXT    NOT NULL,
    contact_email TEXT,
    phone         TEXT
);

-- Подразделения (parent_id вместо hierarchyid из SQL Server)
CREATE TABLE IF NOT EXISTS Departments (
    department_id   INTEGER PRIMARY KEY AUTOINCREMENT,
    department_name TEXT    NOT NULL,
    parent_id       INTEGER,
    FOREIGN KEY (parent_id) REFERENCES Departments(department_id)
);

-- Программное обеспечение
CREATE TABLE IF NOT EXISTS Software (
    software_id INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT    NOT NULL,
    version     TEXT,
    category    TEXT,
    vendor_id   INTEGER,
    FOREIGN KEY (vendor_id) REFERENCES Vendors(vendor_id)
);

-- Лицензии
CREATE TABLE IF NOT EXISTS Licenses (
    license_id      INTEGER PRIMARY KEY AUTOINCREMENT,
    software_id     INTEGER NOT NULL,
    license_key     TEXT    UNIQUE NOT NULL,
    license_type    TEXT,
    purchase_date   TEXT,
    expiration_date TEXT,
    total_seats     INTEGER NOT NULL,
    used_seats      INTEGER DEFAULT 0,
    cost            REAL,
    FOREIGN KEY (software_id) REFERENCES Software(software_id)
);

-- Сотрудники
CREATE TABLE IF NOT EXISTS Employees (
    employee_id   INTEGER PRIMARY KEY AUTOINCREMENT,
    full_name     TEXT    NOT NULL,
    email         TEXT    UNIQUE NOT NULL,
    position      TEXT,
    department_id INTEGER,
    FOREIGN KEY (department_id) REFERENCES Departments(department_id)
);

-- Назначения лицензий (ПОДЧИНЁННАЯ ТАБЛИЦА)
CREATE TABLE IF NOT EXISTS LicenseAssignments (
    assignment_id INTEGER PRIMARY KEY AUTOINCREMENT,
    license_id    INTEGER NOT NULL,
    employee_id   INTEGER NOT NULL,
    assigned_date TEXT    NOT NULL,
    status        TEXT,
    device_type   TEXT,
    device_name   TEXT,
    FOREIGN KEY (license_id)  REFERENCES Licenses(license_id),
    FOREIGN KEY (employee_id) REFERENCES Employees(employee_id)
);

-- Заявки на ПО
CREATE TABLE IF NOT EXISTS Requests (
    request_id   INTEGER PRIMARY KEY AUTOINCREMENT,
    employee_id  INTEGER NOT NULL,
    software_id  INTEGER NOT NULL,
    request_date TEXT    NOT NULL,
    status       TEXT,
    FOREIGN KEY (employee_id) REFERENCES Employees(employee_id),
    FOREIGN KEY (software_id) REFERENCES Software(software_id)
);

-- Отчёты
CREATE TABLE IF NOT EXISTS Reports (
    report_id    INTEGER PRIMARY KEY AUTOINCREMENT,
    report_type  TEXT,
    created_at   TEXT DEFAULT (datetime('now')),
    generated_by INTEGER,
    FOREIGN KEY (generated_by) REFERENCES Employees(employee_id)
);
