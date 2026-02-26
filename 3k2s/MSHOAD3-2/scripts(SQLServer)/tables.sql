-- Содержит информацию о поставщиках программного обеспечения
CREATE TABLE Vendors (
    vendor_id INT PRIMARY KEY IDENTITY(1,1),
    vendor_name VARCHAR(200) NOT NULL,
    contact_email VARCHAR(200),
    phone VARCHAR(50)
);

-- Содержит информацию о подразделениях компании
CREATE TABLE Departments (
    department_id INT PRIMARY KEY IDENTITY(1,1),
    department_name VARCHAR(200) NOT NULL
);

-- Содержит информацию о программном обеспечении
CREATE TABLE Software (
    software_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(200) NOT NULL,
    version VARCHAR(100),
    category VARCHAR(100),
    vendor_id INT,
    CONSTRAINT FK_Software_Vendors 
        FOREIGN KEY (vendor_id) REFERENCES Vendors(vendor_id)
);

-- Содержит информацию о приобретенных лицензиях
CREATE TABLE Licenses (
    license_id INT PRIMARY KEY IDENTITY(1,1),
    software_id INT NOT NULL,
    license_key VARCHAR(255) UNIQUE NOT NULL,
    license_type VARCHAR(100),
    purchase_date DATE,
    expiration_date DATE,
    total_seats INT NOT NULL,
    used_seats INT DEFAULT 0,
    cost DECIMAL(12,2),
    CONSTRAINT FK_Licenses_Software 
        FOREIGN KEY (software_id) REFERENCES Software(software_id)
);

-- Содержит информацию о сотрудниках компании
CREATE TABLE Employees (
    employee_id INT PRIMARY KEY IDENTITY(1,1),
    full_name VARCHAR(200) NOT NULL,
    email VARCHAR(200) UNIQUE NOT NULL,
    position VARCHAR(150),
    department_id INT,
    CONSTRAINT FK_Employees_Departments 
        FOREIGN KEY (department_id) REFERENCES Departments(department_id)
);

-- Фиксирует назначение лицензий сотрудникам
CREATE TABLE LicenseAssignments (
    assignment_id INT PRIMARY KEY IDENTITY(1,1),
    license_id INT NOT NULL,
    employee_id INT NOT NULL,
    assigned_date DATE NOT NULL,
    status VARCHAR(50),
    CONSTRAINT FK_Assignments_Licenses 
        FOREIGN KEY (license_id) REFERENCES Licenses(license_id),
    CONSTRAINT FK_Assignments_Employees 
        FOREIGN KEY (employee_id) REFERENCES Employees(employee_id)
);

-- Содержит заявки сотрудников на получение ПО
CREATE TABLE Requests (
    request_id INT PRIMARY KEY IDENTITY(1,1),
    employee_id INT NOT NULL,
    software_id INT NOT NULL,
    request_date DATE NOT NULL,
    status VARCHAR(50),
    CONSTRAINT FK_Requests_Employees 
        FOREIGN KEY (employee_id) REFERENCES Employees(employee_id),
    CONSTRAINT FK_Requests_Software 
        FOREIGN KEY (software_id) REFERENCES Software(software_id)
);

-- Хранит сгенерированные отчеты
CREATE TABLE Reports (
    report_id INT PRIMARY KEY IDENTITY(1,1),
    report_type VARCHAR(100),
    created_at DATETIME DEFAULT GETDATE(),
    generated_by INT,
    CONSTRAINT FK_Reports_Employees 
        FOREIGN KEY (generated_by) REFERENCES Employees(employee_id)
);

GO