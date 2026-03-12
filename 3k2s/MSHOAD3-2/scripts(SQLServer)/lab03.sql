--Я удалила все данные, для заполнения таблицы Departments выполняй код который в самом конце



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

-- Содержит информацию о подразделениях компании
CREATE TABLE Departments (
    department_id INT PRIMARY KEY IDENTITY(1,1),
    department_name VARCHAR(200) NOT NULL
);

ALTER TABLE Departments
ADD node hierarchyid NOT NULL;

CREATE UNIQUE INDEX IX_Departments_node
ON Departments(node);
GO




DELETE FROM Departments;
DBCC CHECKIDENT ('Departments', RESEED, 0);


INSERT INTO Departments (department_name, node) 
VALUES 
('TOP MANAGEMENT', hierarchyid::Parse('/1/')),       
('TECH TEAM', hierarchyid::Parse('/2/')),            
('DOCUMENTATION TEAM', hierarchyid::Parse('/3/')),   
('SUPPORT TEAM', hierarchyid::Parse('/4/'));         

DECLARE @mgmt_node hierarchyid,
        @design_node hierarchyid,
        @doc_node hierarchyid,
        @support_node hierarchyid;

SELECT @mgmt_node = node FROM Departments WHERE department_name = 'TOP MANAGEMENT';
SELECT @design_node = node FROM Departments WHERE department_name = 'TECH TEAM';
SELECT @doc_node = node FROM Departments WHERE department_name = 'DOCUMENTATION TEAM';
SELECT @support_node = node FROM Departments WHERE department_name = 'SUPPORT TEAM';

-- 1. TOP MANAGEMENT
INSERT INTO Departments (department_name, node) VALUES ('Director', @mgmt_node.GetDescendant(NULL, NULL));
INSERT INTO Departments (department_name, node) VALUES ('Deputy Director', @mgmt_node.GetDescendant(NULL, NULL));
INSERT INTO Departments (department_name, node) VALUES ('Project Manager', @mgmt_node.GetDescendant(NULL, NULL));

-- 2. TECH TEAM
INSERT INTO Departments (department_name, node) VALUES ('Architects', @design_node.GetDescendant(NULL, NULL));
INSERT INTO Departments (department_name, node) VALUES ('Engineers', @design_node.GetDescendant(NULL, NULL));

-- 3. DOCUMENTATION TEAM
INSERT INTO Departments (department_name, node) VALUES ('Technical Designers', @doc_node.GetDescendant(NULL, NULL));
INSERT INTO Departments (department_name, node) VALUES ('Estimators', @doc_node.GetDescendant(NULL, NULL));

-- 4. SUPPORT TEAM
INSERT INTO Departments (department_name, node) VALUES ('IT Team', @support_node.GetDescendant(NULL, NULL));
INSERT INTO Departments (department_name, node) VALUES ('HR Team', @support_node.GetDescendant(NULL, NULL));
INSERT INTO Departments (department_name, node) VALUES ('Marketing Team', @support_node.GetDescendant(NULL, NULL));






-- Задание 2: Процедура отображения всех подчиненных узлов
CREATE OR ALTER PROCEDURE sp_ShowSubtree
     @node_path NVARCHAR(100)
AS
BEGIN
    DECLARE @node hierarchyid = hierarchyid::Parse(@node_path);
    
    SELECT 
        department_name,
        node.ToString() AS NodePath,
        node.GetLevel() AS Level
    FROM Departments
    WHERE node.IsDescendantOf(@node) = 1
    ORDER BY node;
END;
GO

EXEC sp_ShowSubtree '/1/';

EXEC sp_ShowSubtree '/2/';

EXEC sp_ShowSubtree '/2/1/';




-- Задание 3: Процедура добавления подчиненного узла
CREATE OR ALTER PROCEDURE sp_AddChildNode
    @parent_node_path NVARCHAR(100),
    @new_department_name VARCHAR(200)
AS
BEGIN
    DECLARE @parent_node hierarchyid = hierarchyid::Parse(@parent_node_path);
    
    DECLARE @last_child hierarchyid = 
        (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @parent_node);
    
    DECLARE @new_node hierarchyid = @parent_node.GetDescendant(@last_child, NULL);
    
    INSERT INTO Departments (department_name, node) 
    VALUES (@new_department_name, @new_node);
    
    SELECT 
        @new_department_name AS NewDepartment,
        @new_node.ToString() AS NodePath,
        @new_node.GetLevel() AS Level;
END;


EXEC sp_AddChildNode '/2/', 'QA Team';
EXEC sp_AddChildNode '/2/1/', 'Senior Architects';


EXEC sp_AddChildNode '/1/3/1/', 'Add';





-- Задание 4: Процедура перемещения подчиненной ветки
CREATE OR ALTER PROCEDURE sp_MoveSubtree
    @source_node_path NVARCHAR(100),
    @target_node_path NVARCHAR(100)
AS
BEGIN
    DECLARE @source_node hierarchyid = hierarchyid::Parse(@source_node_path);
    DECLARE @target_node hierarchyid = hierarchyid::Parse(@target_node_path);
    
    DECLARE @last_child hierarchyid = 
        (SELECT MAX(node) FROM Departments WHERE node.GetAncestor(1) = @target_node);
    
    DECLARE @new_root hierarchyid = @target_node.GetDescendant(@last_child, NULL);
    
    UPDATE Departments 
    SET node = node.GetReparentedValue(@source_node, @new_root)
    WHERE node.IsDescendantOf(@source_node) = 1;
END;


EXEC sp_ShowSubtree '/2/';
EXEC sp_MoveSubtree '/2/3/', '/3/';
EXEC sp_ShowSubtree '/3/';

EXEC sp_ShowSubtree '/1/';
EXEC sp_MoveSubtree '/4/1/', '/2/';
EXEC sp_ShowSubtree '/1/';


EXEC sp_ShowSubtree '/1/';
EXEC sp_MoveSubtree '/1/3/', '/3/';
EXEC sp_ShowSubtree '/3/';



SELECT 
    department_id,
    REPLICATE('    ', node.GetLevel()) + department_name AS TreeView,
    department_name,
    node.ToString() AS NodePath,
    node.GetLevel() AS Level
FROM Departments
ORDER BY node;















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