USE master;
GO
DROP DATABASE IF EXISTS LogisticsDW;
GO
CREATE DATABASE LogisticsDW;
GO
USE LogisticsDW;
GO

CREATE TABLE DimClient (ClientID INT PRIMARY KEY, ClientName NVARCHAR(100), City NVARCHAR(50), Segment NVARCHAR(20), RegistrationDate DATE);
CREATE TABLE DimDeliveryType (DeliveryTypeID INT PRIMARY KEY, DeliveryName NVARCHAR(100), Category NVARCHAR(50), Subcategory NVARCHAR(50), Price DECIMAL(10,2), Cost DECIMAL(10,2));
CREATE TABLE DimDate (DateID INT PRIMARY KEY, FullDate DATE, Year INT, Quarter INT, QuarterName NVARCHAR(10), Month INT, MonthName NVARCHAR(20), Week INT, DayOfWeek NVARCHAR(20));
CREATE TABLE DimStatus (StatusID INT PRIMARY KEY, StatusName NVARCHAR(30), StatusGroup NVARCHAR(20));
CREATE TABLE FactDeliveries (DeliveryID INT PRIMARY KEY, ClientID INT, DeliveryTypeID INT, DateID INT, StatusID INT, Quantity INT, UnitPrice DECIMAL(10,2), Discount DECIMAL(5,2), TotalAmount DECIMAL(10,2));

INSERT INTO DimClient VALUES (1, N'ООО "Логистик-Про"', N'Москва', N'B2B', '2024-01-15'),(2, N'ИП Быстров', N'СПб', N'B2C', '2024-02-20'),(3, N'АО "Глобал Транс"', N'Казань', N'VIP', '2023-11-10');
INSERT INTO DimDeliveryType VALUES (101, N'Экспресс-доставка', N'Курьерские', N'Срочные', 2500, 1200),(102, N'Фулфилмент', N'Складские', N'Комплектация', 60000, 30000),(103, N'Морской контейнер', N'Международные', N'Море', 25000, 14000);
INSERT INTO DimStatus VALUES (1, N'Новый', N'Активные'),(2, N'В пути', N'Активные'),(3, N'Доставлен', N'Завершенные');
INSERT INTO DimDate VALUES (20250101, '2025-01-01', 2025, 1, N'Q1', 1, N'Январь', 1, N'Пн'),(20250115, '2025-01-15', 2025, 1, N'Q1', 1, N'Январь', 3, N'Ср'),(20250210, '2025-02-10', 2025, 1, N'Q1', 2, N'Февраль', 6, N'Пн');
INSERT INTO FactDeliveries VALUES (1001, 1, 101, 20250101, 3, 2, 2500, 0, 5000),(1002, 2, 102, 20250101, 3, 1, 60000, 0, 60000),(1003, 1, 103, 20250115, 2, 3, 25000, 0, 75000),(1004, 3, 101, 20250210, 3, 5, 2500, 10, 11250);

ALTER TABLE FactDeliveries ADD FOREIGN KEY (ClientID) REFERENCES DimClient(ClientID);
ALTER TABLE FactDeliveries ADD FOREIGN KEY (DeliveryTypeID) REFERENCES DimDeliveryType(DeliveryTypeID);
ALTER TABLE FactDeliveries ADD FOREIGN KEY (DateID) REFERENCES DimDate(DateID);
ALTER TABLE FactDeliveries ADD FOREIGN KEY (StatusID) REFERENCES DimStatus(StatusID);

USE master;
GO

CREATE LOGIN cube_user WITH PASSWORD = 'Qwerty123!';
GO

USE LogisticsDW;
GO

CREATE USER cube_user FOR LOGIN cube_user;
GO

ALTER ROLE db_datareader ADD MEMBER cube_user;
GO
ALTER ROLE db_datawriter ADD MEMBER cube_user;
GO
ALTER ROLE db_owner ADD MEMBER cube_user;
GO