-- Vendors
DELETE FROM Vendors;
DBCC CHECKIDENT ('Vendors', RESEED, 0);

INSERT INTO Vendors (vendor_name, contact_email, phone) VALUES
('Microsoft', 'contact@microsoft.com', '+1-800-111'),
('Adobe', 'contact@adobe.com', '+1-800-222'),
('JetBrains', 'contact@jetbrains.com', '+420-800-333'),
('Autodesk', 'contact@autodesk.com', '+1-800-444');

-- Departments
-- заполняла в lab03(код внизу)

-- Employees
-- заполняла в lab03(код внизу)

-- Software
DELETE FROM Software;
DBCC CHECKIDENT ('Software', RESEED, 0);

INSERT INTO Software (name, version, category, vendor_id) VALUES
('Windows', '11 Pro', 'Operating System', 1),
('Microsoft Office', '365', 'Office', 1),
('Photoshop', '2024', 'Graphics', 2),
('IntelliJ IDEA', '2024.1', 'IDE', 3),
('AutoCAD', '2023', 'Engineering', 4),
('Illustrator', '2024', 'Graphics', 2);

-- Licenses
DELETE FROM Licenses;
DBCC CHECKIDENT ('Licenses', RESEED, 0);

INSERT INTO Licenses
(software_id, license_key, license_type, purchase_date, expiration_date, total_seats, used_seats, cost)
VALUES
(1,'WIN-001','Corporate','2025-01-10','2026-01-10',50,30,5000),
(2,'OFF-001','Subscription','2025-02-15','2026-02-15',100,60,8000),
(3,'PHO-001','Corporate','2025-03-12','2026-03-12',20,15,4000),
(4,'IDE-001','Subscription','2025-04-05','2026-04-05',25,10,3000),
(5,'CAD-001','Corporate','2025-05-18','2026-05-18',15,8,7000),
(6,'ILL-001','Corporate','2025-06-20','2026-06-20',20,12,3500),
(2,'OFF-002','Subscription','2025-07-10','2026-07-10',50,35,5000),
(3,'PHO-002','Corporate','2025-08-22','2026-08-22',10,5,2000),
(2, 'OFF-003', 'Subscription', '2025-08-05', '2026-08-05', 40, 25, 4500),
(2, 'OFF-004', 'Corporate',   '2025-09-12', '2026-09-12', 60, 40, 7500),
(2, 'OFF-005', 'Subscription', '2025-10-18', '2026-10-18', 30, 20, 3800),
(2, 'OFF-006', 'Corporate',   '2025-11-25', '2026-11-25', 45, 30, 6000),
(2, 'OFF-007', 'Subscription', '2025-12-03', '2026-12-03', 55, 35, 7200);

--LicenseAssignments
DELETE FROM LicenseAssignments;
DBCC CHECKIDENT ('LicenseAssignments', RESEED, 0);

INSERT INTO LicenseAssignments
(license_id, employee_id, assigned_date, status, device_type, device_name)
VALUES
(1, 4, '2025-01-15', 'Active', 'Laptop', 'Dell Latitude 3420'),
(2, 5, '2025-02-18', 'Active', 'Laptop', 'HP EliteBook 840'),
(3, 6, '2025-03-15', 'Active', 'Desktop', 'HP ProDesk 400'),
(4, 7, '2025-04-10', 'Active', 'Laptop', 'Lenovo ThinkPad X1'),
(5, 8, '2025-05-20', 'Active', 'Tablet', 'iPad Pro 11'),
(6, 9, '2025-06-25', 'Active', 'Laptop', 'Dell XPS 15'),
(7, 10, '2025-07-12', 'Active', 'Desktop', 'Custom PC'),
(8, 11, '2025-08-30', 'Active', 'Laptop', 'MacBook Pro 14'),
(9, 12, '2025-08-10', 'Active', 'Laptop', 'ASUS ZenBook'),
(9, 13, '2025-08-15', 'Active', 'Tablet', 'Samsung Tab S9'),
(10, 14, '2025-09-20', 'Active', 'Laptop', 'Acer Swift 3'),
(10, 15, '2025-09-25', 'Active', 'Desktop', 'Dell OptiPlex'),
(10, 16, '2025-09-28', 'Active', 'Laptop', 'Microsoft Surface'),
(11, 17, '2025-10-22', 'Active', 'Laptop', 'Lenovo Legion 5'),
(11, 18, '2025-10-25', 'Active', 'Tablet', 'iPad Air'),
(12, 19, '2025-11-28', 'Active', 'Desktop', 'HP Z440 Workstation'),
(12, 20, '2025-11-30', 'Active', 'Laptop', 'Dell Precision 3571'),
(12, 21, '2025-12-02', 'Active', 'Laptop', 'Razer Blade 15'),
(13, 22, '2025-12-10', 'Active', 'Desktop', 'Custom Gaming PC'),
(13, 23, '2025-12-15', 'Active', 'Tablet', 'Microsoft Surface Pro'),
(13, 24, '2025-12-18', 'Active', 'Laptop', 'MacBook Air M2');

--Requests
DELETE FROM Requests;
DBCC CHECKIDENT ('Requests', RESEED, 0);

INSERT INTO Requests
(employee_id, software_id, request_date, status)
VALUES
(4,2,'2025-01-05','Approved'),
(5,3,'2025-02-10','Approved'),
(6,4,'2025-03-11','Approved'),
(7,2,'2025-04-14','Approved'),
(8,5,'2025-05-16','Rejected'),
(9,3,'2025-06-21','Approved'),
(10,6,'2025-07-07','Approved'),
(11,4,'2025-08-19','Approved');

--Reports
DELETE FROM Reports;
DBCC CHECKIDENT ('Reports', RESEED, 0);

INSERT INTO Reports (report_type, created_at, generated_by) VALUES
('License usage report', '2025-01-31T00:00:00', 1),
('Software requests report', '2025-02-28T00:00:00', 2),
('License cost analysis', '2025-03-31T00:00:00', 3),
('Department software usage', '2025-04-30T00:00:00', 4),
('Quarter license statistics', '2025-06-30T00:00:00', 5),
('Annual software expenses', '2025-12-31T00:00:00', 1);



------------------------------------------ ЗАПОЛНЕНИЕ EMPLOYEES
DELETE FROM Employees;
DBCC CHECKIDENT ('Employees', RESEED, 0);


DECLARE @dir_id INT, @dep_dir_id INT, @pm_id INT,
        @arch_id INT, @eng_id INT,
        @draft_id INT, @est_id INT,
        @it_id INT, @hr_id INT, @mkt_id INT;

-- TOP MANAGEMENT IDs
SELECT @dir_id = department_id FROM Departments WHERE department_name = 'Director';
SELECT @dep_dir_id = department_id FROM Departments WHERE department_name = 'Deputy Director';
SELECT @pm_id = department_id FROM Departments WHERE department_name = 'Project Manager';

-- TECH TEAM IDs
SELECT @arch_id = department_id FROM Departments WHERE department_name = 'Architects';
SELECT @eng_id = department_id FROM Departments WHERE department_name = 'Engineers';

-- DOCUMENTATION TEAM IDs
SELECT @draft_id = department_id FROM Departments WHERE department_name = 'Technical Designers';
SELECT @est_id = department_id FROM Departments WHERE department_name = 'Estimators';

-- SUPPORT TEAM IDs
SELECT @it_id = department_id FROM Departments WHERE department_name = 'IT Team';
SELECT @hr_id = department_id FROM Departments WHERE department_name = 'HR Team';
SELECT @mkt_id = department_id FROM Departments WHERE department_name = 'Marketing Team';

-- TOP MANAGEMENT
INSERT INTO Employees (full_name, email, position, department_id) VALUES
('John Director', 'john.director@repairbuild.com', 'Lead', @dir_id),
('Sarah Deputy', 'sarah.deputy@repairbuild.com', 'Senior', @dep_dir_id),
('Mike Project', 'mike.project@repairbuild.com', 'Manager', @pm_id);

-- TECH TEAM
-- Architects
INSERT INTO Employees (full_name, email, position, department_id) VALUES
('Alice Smith', 'alice.smith@repairbuild.com', 'Lead', @arch_id),
('Bob Johnson', 'bob.johnson@repairbuild.com', 'Senior', @arch_id),
('Carol Williams', 'carol.williams@repairbuild.com', 'Senior', @arch_id),
('David Brown', 'david.brown@repairbuild.com', 'Middle', @arch_id),
('Emma Jones', 'emma.jones@repairbuild.com', 'Middle', @arch_id),
('Frank Miller', 'frank.miller@repairbuild.com', 'Junior', @arch_id),
('Grace Davis', 'grace.davis@repairbuild.com', 'Junior', @arch_id),
('Henry Wilson', 'henry.wilson@repairbuild.com', 'Middle', @arch_id);

-- Engineers
INSERT INTO Employees (full_name, email, position, department_id) VALUES
('Ivy Taylor', 'ivy.taylor@repairbuild.com', 'Lead', @eng_id),
('Jack Anderson', 'jack.anderson@repairbuild.com', 'Senior', @eng_id),
('Kelly Thomas', 'kelly.thomas@repairbuild.com', 'Middle', @eng_id),
('Leo Jackson', 'leo.jackson@repairbuild.com', 'Middle', @eng_id),
('Mia White', 'mia.white@repairbuild.com', 'Junior', @eng_id),
('Noah Harris', 'noah.harris@repairbuild.com', 'Junior', @eng_id);

-- DOCUMENTATION TEAM
-- Technical Designers
INSERT INTO Employees (full_name, email, position, department_id) VALUES
('Oliver Martin', 'oliver.martin@repairbuild.com', 'Senior', @draft_id),
('Patricia Thompson', 'patricia.thompson@repairbuild.com', 'Middle', @draft_id),
('Quentin Garcia', 'quentin.garcia@repairbuild.com', 'Middle', @draft_id),
('Rachel Martinez', 'rachel.martinez@repairbuild.com', 'Junior', @draft_id),
('Samuel Robinson', 'samuel.robinson@repairbuild.com', 'Junior', @draft_id),
('Tina Clark', 'tina.clark@repairbuild.com', 'Middle', @draft_id);

-- Estimators
INSERT INTO Employees (full_name, email, position, department_id) VALUES
('Ulysses Rodriguez', 'ulysses.rodriguez@repairbuild.com', 'Senior', @est_id),
('Victoria Lewis', 'victoria.lewis@repairbuild.com', 'Middle', @est_id),
('William Lee', 'william.lee@repairbuild.com', 'Middle', @est_id),
('Xena Walker', 'xena.walker@repairbuild.com', 'Junior', @est_id);

-- SUPPORT TEAM
-- IT Team
INSERT INTO Employees (full_name, email, position, department_id) VALUES
('Yan Hall', 'yan.hall@repairbuild.com', 'Lead', @it_id),
('Zoe Allen', 'zoe.allen@repairbuild.com', 'Senior', @it_id),
('Adam Young', 'adam.young@repairbuild.com', 'Middle', @it_id);

-- HR Team
INSERT INTO Employees (full_name, email, position, department_id) VALUES
('Brenda King', 'brenda.king@repairbuild.com', 'Manager', @hr_id),
('Chris Wright', 'chris.wright@repairbuild.com', 'Specialist', @hr_id);

-- Marketing Team
INSERT INTO Employees (full_name, email, position, department_id) VALUES
('Diana Lopez', 'diana.lopez@repairbuild.com', 'Manager', @mkt_id),
('Edward Hill', 'edward.hill@repairbuild.com', 'Specialist', @mkt_id);











------------------------------------------ЗАПОЛНЕНИЕ DEPARTMENTS

-- Объявляем переменные
DECLARE @mgmt_node hierarchyid,
        @tech_node hierarchyid,
        @doc_node hierarchyid,
        @support_node hierarchyid;

-- Получаем узлы
SELECT @mgmt_node = node FROM Departments WHERE department_name = 'TOP MANAGEMENT';
SELECT @tech_node = node FROM Departments WHERE department_name = 'TECH TEAM';
SELECT @doc_node = node FROM Departments WHERE department_name = 'DOCUMENTATION TEAM';
SELECT @support_node = node FROM Departments WHERE department_name = 'SUPPORT TEAM';

-- ===========================================
-- 1. TOP MANAGEMENT - РАБОЧИЙ ВАРИАНТ
-- ===========================================
-- Director
INSERT INTO Departments (department_name, node) 
SELECT 'Director', @mgmt_node.GetDescendant(
    (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @mgmt_node),
    NULL
);

-- Deputy Director
INSERT INTO Departments (department_name, node) 
SELECT 'Deputy Director', @mgmt_node.GetDescendant(
    (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @mgmt_node),
    NULL
);

-- Project Manager
INSERT INTO Departments (department_name, node) 
SELECT 'Project Manager', @mgmt_node.GetDescendant(
    (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @mgmt_node),
    NULL
);

-- ===========================================
-- 2. TECH TEAM
-- ===========================================
-- Architects
INSERT INTO Departments (department_name, node) 
SELECT 'Architects', @tech_node.GetDescendant(
    (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @tech_node),
    NULL
);

-- Engineers
INSERT INTO Departments (department_name, node) 
SELECT 'Engineers', @tech_node.GetDescendant(
    (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @tech_node),
    NULL
);

-- ===========================================
-- 3. DOCUMENTATION TEAM
-- ===========================================
-- Technical Designers
INSERT INTO Departments (department_name, node) 
SELECT 'Technical Designers', @doc_node.GetDescendant(
    (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @doc_node),
    NULL
);

-- Estimators
INSERT INTO Departments (department_name, node) 
SELECT 'Estimators', @doc_node.GetDescendant(
    (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @doc_node),
    NULL
);

-- ===========================================
-- 4. SUPPORT TEAM
-- ===========================================
-- IT Team
INSERT INTO Departments (department_name, node) 
SELECT 'IT Team', @support_node.GetDescendant(
    (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @support_node),
    NULL
);

-- HR Team
INSERT INTO Departments (department_name, node) 
SELECT 'HR Team', @support_node.GetDescendant(
    (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @support_node),
    NULL
);

-- Marketing Team
INSERT INTO Departments (department_name, node) 
SELECT 'Marketing Team', @support_node.GetDescendant(
    (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @support_node),
    NULL
);