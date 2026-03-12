-- Vendors
DELETE FROM Vendors;
DBCC CHECKIDENT ('Vendors', RESEED, 0);

INSERT INTO Vendors (vendor_name, contact_email, phone) VALUES
('Microsoft', 'contact@microsoft.com', '+1-800-111'),
('Adobe', 'contact@adobe.com', '+1-800-222'),
('JetBrains', 'contact@jetbrains.com', '+420-800-333'),
('Autodesk', 'contact@autodesk.com', '+1-800-444');

-- Departments
-- заполняла в lab03

-- Employees
-- заполняла в lab03

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
(license_id, employee_id, assigned_date, status)
VALUES
(1, 4, '2025-01-15', 'Active'),
(2, 5, '2025-02-18','Active'),
(3, 6, '2025-03-15', 'Active'),
(4, 7, '2025-04-10', 'Active'),
(5, 8, '2025-05-20', 'Active'),
(6, 9, '2025-06-25', 'Active'),
(7, 10, '2025-07-12', 'Active'),
(8, 11, '2025-08-30', 'Active'),
(9, 12, '2025-08-10', 'Active'),
(9, 13, '2025-08-15', 'Active'),
(10, 14, '2025-09-20', 'Active'),
(10, 15, '2025-09-25', 'Active'),
(10, 16, '2025-09-28', 'Active'),
(11, 17, '2025-10-22', 'Active'),
(11, 18, '2025-10-25', 'Active'),
(12, 19, '2025-11-28', 'Active'),
(12, 20, '2025-11-30', 'Active'),
(12, 21, '2025-12-02', 'Active'),
(13, 22, '2025-12-10', 'Active'),
(13, 23, '2025-12-15', 'Active'),
(13, 24, '2025-12-18', 'Active');

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