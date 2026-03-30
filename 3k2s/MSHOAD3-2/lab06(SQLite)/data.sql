PRAGMA foreign_keys = ON;

-- Vendors
INSERT INTO Vendors (vendor_name, contact_email, phone) VALUES
('Microsoft', 'contact@microsoft.com', '+1-800-111'),
('Adobe',     'contact@adobe.com',     '+1-800-222'),
('JetBrains', 'contact@jetbrains.com', '+420-800-333'),
('Autodesk',  'contact@autodesk.com',  '+1-800-444');

-- Departments (корневые)
INSERT INTO Departments (department_name, parent_id) VALUES
('TOP MANAGEMENT', NULL),
('TECH TEAM', NULL),
('DOCUMENTATION TEAM', NULL),
('SUPPORT TEAM', NULL);

-- Departments (дочерние)
INSERT INTO Departments (department_name, parent_id) VALUES
('Director', 1),
('Deputy Director', 1),
('Project Manager', 1),
('Architects', 2),
('Engineers', 2),
('Technical Designers', 3),
('Estimators', 3),
('IT Team', 4),
('HR Team', 4),
('Marketing Team', 4);

-- Software
INSERT INTO Software (name, version, category, vendor_id) VALUES
('Windows',          '11 Pro',  'Operating System', 1),
('Microsoft Office', '365',     'Office',           1),
('Photoshop',        '2024',    'Graphics',         2),
('IntelliJ IDEA',    '2024.1',  'IDE',              3),
('AutoCAD',          '2023',    'Engineering',       4),
('Illustrator',      '2024',    'Graphics',         2);

-- Licenses
INSERT INTO Licenses
(software_id, license_key, license_type, purchase_date, expiration_date, total_seats, used_seats, cost)
VALUES
(1, 'WIN-001', 'Corporate',    '2025-01-10', '2026-01-10', 50,  30, 5000.00),
(2, 'OFF-001', 'Subscription', '2025-02-15', '2026-02-15', 100, 60, 8000.00),
(3, 'PHO-001', 'Corporate',    '2025-03-12', '2026-03-12', 20,  15, 4000.00),
(4, 'IDE-001', 'Subscription', '2025-04-05', '2026-04-05', 25,  10, 3000.00),
(5, 'CAD-001', 'Corporate',    '2025-05-18', '2026-05-18', 15,  8,  7000.00),
(6, 'ILL-001', 'Corporate',    '2025-06-20', '2026-06-20', 20,  12, 3500.00),
(2, 'OFF-002', 'Subscription', '2025-07-10', '2026-07-10', 50,  35, 5000.00),
(3, 'PHO-002', 'Corporate',    '2025-08-22', '2026-08-22', 10,  5,  2000.00),
(2, 'OFF-003', 'Subscription', '2025-08-05', '2026-08-05', 40,  25, 4500.00),
(2, 'OFF-004', 'Corporate',    '2025-09-12', '2026-09-12', 60,  40, 7500.00),
(2, 'OFF-005', 'Subscription', '2025-10-18', '2026-10-18', 30,  20, 3800.00),
(2, 'OFF-006', 'Corporate',    '2025-11-25', '2026-11-25', 45,  30, 6000.00),
(2, 'OFF-007', 'Subscription', '2025-12-03', '2026-12-03', 55,  35, 7200.00);

-- Employees
INSERT INTO Employees (full_name, email, position, department_id) VALUES
('John Director',       'john.director@repairbuild.com',       'Lead',       5),
('Sarah Deputy',        'sarah.deputy@repairbuild.com',        'Senior',     6),
('Mike Project',        'mike.project@repairbuild.com',        'Manager',    7),
('Alice Smith',         'alice.smith@repairbuild.com',         'Lead',       8),
('Bob Johnson',         'bob.johnson@repairbuild.com',         'Senior',     8),
('Carol Williams',      'carol.williams@repairbuild.com',      'Senior',     8),
('David Brown',         'david.brown@repairbuild.com',         'Middle',     8),
('Emma Jones',          'emma.jones@repairbuild.com',          'Middle',     8),
('Frank Miller',        'frank.miller@repairbuild.com',        'Junior',     8),
('Grace Davis',         'grace.davis@repairbuild.com',         'Junior',     8),
('Henry Wilson',        'henry.wilson@repairbuild.com',        'Middle',     8),
('Ivy Taylor',          'ivy.taylor@repairbuild.com',          'Lead',       9),
('Jack Anderson',       'jack.anderson@repairbuild.com',       'Senior',     9),
('Kelly Thomas',        'kelly.thomas@repairbuild.com',        'Middle',     9),
('Leo Jackson',         'leo.jackson@repairbuild.com',         'Middle',     9),
('Mia White',           'mia.white@repairbuild.com',           'Junior',     9),
('Noah Harris',         'noah.harris@repairbuild.com',         'Junior',     9),
('Oliver Martin',       'oliver.martin@repairbuild.com',       'Senior',     10),
('Patricia Thompson',   'patricia.thompson@repairbuild.com',   'Middle',     10),
('Quentin Garcia',      'quentin.garcia@repairbuild.com',      'Middle',     10),
('Rachel Martinez',     'rachel.martinez@repairbuild.com',     'Junior',     10),
('Samuel Robinson',     'samuel.robinson@repairbuild.com',     'Junior',     10),
('Tina Clark',          'tina.clark@repairbuild.com',          'Middle',     10),
('Ulysses Rodriguez',   'ulysses.rodriguez@repairbuild.com',   'Senior',     11),
('Victoria Lewis',      'victoria.lewis@repairbuild.com',      'Middle',     11),
('William Lee',         'william.lee@repairbuild.com',         'Middle',     11),
('Xena Walker',         'xena.walker@repairbuild.com',         'Junior',     11),
('Yan Hall',            'yan.hall@repairbuild.com',            'Lead',       12),
('Zoe Allen',           'zoe.allen@repairbuild.com',           'Senior',     12),
('Adam Young',          'adam.young@repairbuild.com',          'Middle',     12),
('Brenda King',         'brenda.king@repairbuild.com',         'Manager',    13),
('Chris Wright',        'chris.wright@repairbuild.com',        'Specialist', 13),
('Diana Lopez',         'diana.lopez@repairbuild.com',         'Manager',    14),
('Edward Hill',         'edward.hill@repairbuild.com',         'Specialist', 14);

-- LicenseAssignments
INSERT INTO LicenseAssignments
(license_id, employee_id, assigned_date, status, device_type, device_name)
VALUES
(1,  4,  '2025-01-15', 'Active', 'Laptop',  'Dell Latitude 3420'),
(2,  5,  '2025-02-18', 'Active', 'Laptop',  'HP EliteBook 840'),
(3,  6,  '2025-03-15', 'Active', 'Desktop', 'HP ProDesk 400'),
(4,  7,  '2025-04-10', 'Active', 'Laptop',  'Lenovo ThinkPad X1'),
(5,  8,  '2025-05-20', 'Active', 'Tablet',  'iPad Pro 11'),
(6,  9,  '2025-06-25', 'Active', 'Laptop',  'Dell XPS 15'),
(7,  10, '2025-07-12', 'Active', 'Desktop', 'Custom PC'),
(8,  11, '2025-08-30', 'Active', 'Laptop',  'MacBook Pro 14'),
(9,  12, '2025-08-10', 'Active', 'Laptop',  'ASUS ZenBook'),
(9,  13, '2025-08-15', 'Active', 'Tablet',  'Samsung Tab S9'),
(10, 14, '2025-09-20', 'Active', 'Laptop',  'Acer Swift 3'),
(10, 15, '2025-09-25', 'Active', 'Desktop', 'Dell OptiPlex'),
(10, 16, '2025-09-28', 'Active', 'Laptop',  'Microsoft Surface'),
(11, 17, '2025-10-22', 'Active', 'Laptop',  'Lenovo Legion 5'),
(11, 18, '2025-10-25', 'Active', 'Tablet',  'iPad Air'),
(12, 19, '2025-11-28', 'Active', 'Desktop', 'HP Z440 Workstation'),
(12, 20, '2025-11-30', 'Active', 'Laptop',  'Dell Precision 3571'),
(12, 21, '2025-12-02', 'Active', 'Laptop',  'Razer Blade 15'),
(13, 22, '2025-12-10', 'Active', 'Desktop', 'Custom Gaming PC'),
(13, 23, '2025-12-15', 'Active', 'Tablet',  'Microsoft Surface Pro'),
(13, 24, '2025-12-18', 'Active', 'Laptop',  'MacBook Air M2');

-- Requests
INSERT INTO Requests (employee_id, software_id, request_date, status) VALUES
(4,  2, '2025-01-05', 'Approved'),
(5,  3, '2025-02-10', 'Approved'),
(6,  4, '2025-03-11', 'Approved'),
(7,  2, '2025-04-14', 'Approved'),
(8,  5, '2025-05-16', 'Rejected'),
(9,  3, '2025-06-21', 'Approved'),
(10, 6, '2025-07-07', 'Approved'),
(11, 4, '2025-08-19', 'Approved');

-- Reports
INSERT INTO Reports (report_type, created_at, generated_by) VALUES
('License usage report',       '2025-01-31 00:00:00', 1),
('Software requests report',   '2025-02-28 00:00:00', 2),
('License cost analysis',      '2025-03-31 00:00:00', 3),
('Department software usage',  '2025-04-30 00:00:00', 4),
('Quarter license statistics', '2025-06-30 00:00:00', 5),
('Annual software expenses',   '2025-12-31 00:00:00', 1);