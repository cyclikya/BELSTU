ALTER TABLE Licenses MODIFY license_id DROP IDENTITY;
ALTER TABLE Licenses MODIFY license_id ADD IDENTITY (START WITH 1);

-- Vendors
DELETE FROM Vendors;

INSERT INTO Vendors VALUES (1,'Microsoft','contact@microsoft.com','+1-800-642-7676');
INSERT INTO Vendors VALUES (2,'Adobe','support@adobe.com','+1-800-833-6687');
INSERT INTO Vendors VALUES (3,'JetBrains','sales@jetbrains.com','+420-241-722-501');
INSERT INTO Vendors VALUES (4,'Autodesk','info@autodesk.com','+1-415-507-5000');

-- Departments
DELETE FROM Departments;

INSERT INTO Departments VALUES (1, 'TOP MANAGEMENT', NULL);
INSERT INTO Departments VALUES (2, 'TECH TEAM', NULL);
INSERT INTO Departments VALUES (3, 'DOCUMENTATION TEAM', NULL);
INSERT INTO Departments VALUES (4, 'SUPPORT TEAM', NULL);
INSERT INTO Departments VALUES (5, 'Director', 1);
INSERT INTO Departments VALUES (6, 'Deputy Director', 1);
INSERT INTO Departments VALUES (7, 'Project Manager', 1);
INSERT INTO Departments VALUES (8, 'Architects', 2);
INSERT INTO Departments VALUES (9, 'Engineers', 2);
INSERT INTO Departments VALUES (10, 'Technical Designers', 3);
INSERT INTO Departments VALUES (11, 'Estimators', 3);
INSERT INTO Departments VALUES (12, 'IT Team', 4);
INSERT INTO Departments VALUES (13, 'HR Team', 4);
INSERT INTO Departments VALUES (14, 'Marketing Team', 4);

-- Employees
DELETE FROM Employees;

INSERT INTO Employees VALUES (1, 'John Director', 'john.director@repairbuild.com', 'Lead', 5);
INSERT INTO Employees VALUES (2, 'Sarah Deputy', 'sarah.deputy@repairbuild.com', 'Senior', 6);
INSERT INTO Employees VALUES (3, 'Mike Project', 'mike.project@repairbuild.com', 'Manager', 7);
INSERT INTO Employees VALUES (4, 'Alice Smith', 'alice.smith@repairbuild.com', 'Lead', 8);
INSERT INTO Employees VALUES (5, 'Bob Johnson', 'bob.johnson@repairbuild.com', 'Senior', 8);
INSERT INTO Employees VALUES (6, 'Carol Williams', 'carol.williams@repairbuild.com', 'Senior', 8);
INSERT INTO Employees VALUES (7, 'David Brown', 'david.brown@repairbuild.com', 'Middle', 8);
INSERT INTO Employees VALUES (8, 'Emma Jones', 'emma.jones@repairbuild.com', 'Middle', 8);
INSERT INTO Employees VALUES (9, 'Frank Miller', 'frank.miller@repairbuild.com', 'Junior', 8);
INSERT INTO Employees VALUES (10, 'Grace Davis', 'grace.davis@repairbuild.com', 'Junior', 8);
INSERT INTO Employees VALUES (11, 'Henry Wilson', 'henry.wilson@repairbuild.com', 'Middle', 8);
INSERT INTO Employees VALUES (12, 'Ivy Taylor', 'ivy.taylor@repairbuild.com', 'Lead', 9);
INSERT INTO Employees VALUES (13, 'Jack Anderson', 'jack.anderson@repairbuild.com', 'Senior', 9);
INSERT INTO Employees VALUES (14, 'Kelly Thomas', 'kelly.thomas@repairbuild.com', 'Middle', 9);
INSERT INTO Employees VALUES (15, 'Leo Jackson', 'leo.jackson@repairbuild.com', 'Middle', 9);
INSERT INTO Employees VALUES (16, 'Mia White', 'mia.white@repairbuild.com', 'Junior', 9);
INSERT INTO Employees VALUES (17, 'Noah Harris', 'noah.harris@repairbuild.com', 'Junior', 9);
INSERT INTO Employees VALUES (18, 'Oliver Martin', 'oliver.martin@repairbuild.com', 'Senior', 10);
INSERT INTO Employees VALUES (19, 'Patricia Thompson', 'patricia.thompson@repairbuild.com', 'Middle', 10);
INSERT INTO Employees VALUES (20, 'Quentin Garcia', 'quentin.garcia@repairbuild.com', 'Middle', 10);
INSERT INTO Employees VALUES (21, 'Rachel Martinez', 'rachel.martinez@repairbuild.com', 'Junior', 10);
INSERT INTO Employees VALUES (22, 'Samuel Robinson', 'samuel.robinson@repairbuild.com', 'Junior', 10);
INSERT INTO Employees VALUES (23, 'Tina Clark', 'tina.clark@repairbuild.com', 'Middle', 10);
INSERT INTO Employees VALUES (24, 'Ulysses Rodriguez', 'ulysses.rodriguez@repairbuild.com', 'Senior', 11);
INSERT INTO Employees VALUES (25, 'Victoria Lewis', 'victoria.lewis@repairbuild.com', 'Middle', 11);
INSERT INTO Employees VALUES (26, 'William Lee', 'william.lee@repairbuild.com', 'Middle', 11);
INSERT INTO Employees VALUES (27, 'Xena Walker', 'xena.walker@repairbuild.com', 'Junior', 11);
INSERT INTO Employees VALUES (28, 'Yan Hall', 'yan.hall@repairbuild.com', 'Lead', 12);
INSERT INTO Employees VALUES (29, 'Zoe Allen', 'zoe.allen@repairbuild.com', 'Senior', 12);
INSERT INTO Employees VALUES (30, 'Adam Young', 'adam.young@repairbuild.com', 'Middle', 12);
INSERT INTO Employees VALUES (31, 'Brenda King', 'brenda.king@repairbuild.com', 'Manager', 13);
INSERT INTO Employees VALUES (32, 'Chris Wright', 'chris.wright@repairbuild.com', 'Specialist', 13);
INSERT INTO Employees VALUES (33, 'Diana Lopez', 'diana.lopez@repairbuild.com', 'Manager', 14);
INSERT INTO Employees VALUES (34, 'Edward Hill', 'edward.hill@repairbuild.com', 'Specialist', 14);

-- Software
DELETE FROM Software;

INSERT INTO Software VALUES (1,'Windows','11 Pro','Operating System',1);
INSERT INTO Software VALUES (2,'Microsoft Office','365','Office',1);
INSERT INTO Software VALUES (3,'Photoshop','2024','Graphics',2);
INSERT INTO Software VALUES (4,'IntelliJ IDEA','2024.1','IDE',3);
INSERT INTO Software VALUES (5,'AutoCAD','2023','Engineering',4);
INSERT INTO Software VALUES (6,'Illustrator','2024','Graphics',2);

-- Licenses
DELETE FROM Licenses;

INSERT INTO Licenses VALUES (1,1,'WIN-001','Corporate',DATE '2025-01-10',DATE '2026-01-10',50,30,5000);
INSERT INTO Licenses VALUES (2,2,'OFF-001','Subscription',DATE '2025-02-15',DATE '2026-02-15',100,60,8000);
INSERT INTO Licenses VALUES (3,3,'PHO-001','Corporate',DATE '2025-03-12',DATE '2026-03-12',20,15,4000);
INSERT INTO Licenses VALUES (4,4,'IDE-001','Subscription',DATE '2025-04-05',DATE '2026-04-05',25,10,3000);
INSERT INTO Licenses VALUES (5,5,'CAD-001','Corporate',DATE '2025-05-18',DATE '2026-05-18',15,8,7000);
INSERT INTO Licenses VALUES (6,6,'ILL-001','Corporate',DATE '2025-06-20',DATE '2026-06-20',20,12,3500);
INSERT INTO Licenses VALUES (7,2, 'OFF-002', 'Subscription', DATE '2025-07-10', DATE '2026-07-10', 50, 35, 5000);
INSERT INTO Licenses VALUES (8,3, 'PHO-002', 'Corporate', DATE '2025-08-22', DATE '2026-08-22', 10, 5, 2000);
INSERT INTO Licenses VALUES (9,2, 'OFF-003', 'Subscription', DATE '2025-08-05', DATE '2026-08-05', 40, 25, 4500);
INSERT INTO Licenses VALUES (10,2, 'OFF-004', 'Corporate', DATE '2025-09-12', DATE '2026-09-12', 60, 40, 7500);
INSERT INTO Licenses VALUES (11,2, 'OFF-005', 'Subscription', DATE '2025-10-18', DATE '2026-10-18', 30, 20, 3800);
INSERT INTO Licenses VALUES (12,2, 'OFF-006', 'Corporate', DATE '2025-11-25', DATE '2026-11-25', 45, 30, 6000);
INSERT INTO Licenses VALUES (13,2, 'OFF-007', 'Subscription', DATE '2025-12-03', DATE '2026-12-03', 55, 35, 7200);

--LicenseAssignments
DELETE FROM LicenseAssignments;

INSERT INTO LicenseAssignments VALUES (1, 1, 4, DATE '2025-01-15', 'Active');
INSERT INTO LicenseAssignments VALUES (2, 2, 5, DATE '2025-02-18', 'Active');
INSERT INTO LicenseAssignments VALUES (3, 3, 6, DATE '2025-03-15', 'Active');
INSERT INTO LicenseAssignments VALUES (4, 4, 7, DATE '2025-04-10', 'Active');
INSERT INTO LicenseAssignments VALUES (5, 5, 8, DATE '2025-05-20', 'Active');
INSERT INTO LicenseAssignments VALUES (6, 6, 9, DATE '2025-06-25', 'Active');
INSERT INTO LicenseAssignments VALUES (7, 10, DATE '2025-07-12', 'Active');
INSERT INTO LicenseAssignments VALUES (8, 8, 11, DATE '2025-08-30', 'Active');
INSERT INTO LicenseAssignments VALUES (9, 9, 12, DATE '2025-08-10', 'Active');
INSERT INTO LicenseAssignments VALUES (10, 9, 13, DATE '2025-08-15', 'Active');
INSERT INTO LicenseAssignments VALUES (11, 10, 14, DATE '2025-09-20', 'Active');
INSERT INTO LicenseAssignments VALUES (12, 10, 15, DATE '2025-09-25', 'Active');
INSERT INTO LicenseAssignments VALUES (13, 10, 16, DATE '2025-09-28', 'Active');
INSERT INTO LicenseAssignments VALUES (14, 11, 17, DATE '2025-10-22', 'Active');
INSERT INTO LicenseAssignments VALUES (15, 11, 18, DATE '2025-10-25', 'Active');
INSERT INTO LicenseAssignments VALUES (16, 12, 19, DATE '2025-11-28', 'Active');
INSERT INTO LicenseAssignments VALUES (17, 12, 20, DATE '2025-11-30', 'Active');
INSERT INTO LicenseAssignments VALUES (18, 12, 21, DATE '2025-12-02', 'Active');
INSERT INTO LicenseAssignments VALUES (19, 13, 22, DATE '2025-12-10', 'Active');
INSERT INTO LicenseAssignments VALUES (20, 13, 23, DATE '2025-12-15', 'Active');
INSERT INTO LicenseAssignments VALUES (21, 13, 24, DATE '2025-12-18', 'Active');

--Requests
DELETE FROM Requests;

INSERT INTO Requests VALUES (1,4,2,DATE '2025-01-05','Approved');
INSERT INTO Requests VALUES (2,5,3,DATE '2025-02-10','Approved');
INSERT INTO Requests VALUES (3,6,4,DATE '2025-03-11','Approved');
INSERT INTO Requests VALUES (4,7,2,DATE '2025-04-14','Approved');
INSERT INTO Requests VALUES (5,8,5,DATE '2025-05-16','Rejected');

--Reports
DELETE FROM Reports;

INSERT INTO Reports VALUES (1,'License usage report',DATE '2025-01-31',1);
INSERT INTO Reports VALUES (2,'Software requests report',DATE '2025-02-28',2);
INSERT INTO Reports VALUES (3,'License cost analysis',DATE '2025-03-31',3);
INSERT INTO Reports VALUES (4,'Department software usage',DATE '2025-04-30',4);
INSERT INTO Reports VALUES (5,'Quarter license statistics',DATE '2025-06-30',5);