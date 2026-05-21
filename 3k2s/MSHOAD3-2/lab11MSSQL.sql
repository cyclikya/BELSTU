CREATE DATABASE lab11_migration;
GO

USE lab11_migration;
GO

CREATE TABLE Students
(
    StudentId INT PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    GroupName NVARCHAR(20) NOT NULL,
    AverageMark DECIMAL(4,2),
    AdmissionDate DATE
);
GO

INSERT INTO Students (StudentId, FullName, GroupName, AverageMark, AdmissionDate)
VALUES
(1, N'Иванова Анна', N'ПО-31', 8.75, '2023-09-01'),
(2, N'Петров Иван', N'ПО-31', 7.40, '2023-09-01'),
(3, N'Сидорова Мария', N'ПО-32', 9.10, '2023-09-01');
GO

SELECT * FROM Students;