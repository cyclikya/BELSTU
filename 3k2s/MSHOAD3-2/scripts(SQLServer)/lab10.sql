USE master;
-- Включение CLR
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'clr enabled', 1;
RECONFIGURE;
EXEC sp_configure 'clr strict security', 0;
RECONFIGURE;
GO
Create database lab10;

-- Разрешение для сборки с доступом к внешнему файлу
ALTER DATABASE [lab10] SET TRUSTWORTHY ON;
GO

USE [lab10];
GO

-- Удаление старых объектов, если они уже существуют
DROP PROCEDURE IF EXISTS dbo.ReadLicenseFile;
DROP TYPE IF EXISTS dbo.LicenseData;
DROP ASSEMBLY IF EXISTS lab10;
GO

-- Подключение CLR-сборки
CREATE ASSEMBLY lab10
FROM 'C:\General\BELSTU\3k2s\MSHOAD3-2\lab10(CLR)\lab10\bin\Debug\lab10.dll'
WITH PERMISSION_SET = EXTERNAL_ACCESS;
GO

-- Создание пользовательского CLR-типа
CREATE TYPE dbo.LicenseData
    EXTERNAL NAME lab10.LicenseData;
GO

-- Создание CLR-процедуры по варианту 12: чтение данных из внешнего файла
CREATE PROCEDURE dbo.ReadLicenseFile
    @FilePath NVARCHAR(4000)
        AS EXTERNAL NAME lab10.FileOperations.ReadLicenseFile;
GO

-- создание тестового файла с данными лицензий
EXEC xp_cmdshell 'mkdir C:\CLR_Test';
EXEC xp_cmdshell 'echo 1^|Visual Studio^|AAAA-BBBB-CCCC^|2027-01-01^|199.99 > C:\CLR_Test\licenses.txt';
EXEC xp_cmdshell 'echo 2^|Microsoft Office^|DDDD-EEEE-FFFF^|2023-12-31^|149.50 >> C:\CLR_Test\licenses.txt';
EXEC xp_cmdshell 'echo 3^|Adobe Photoshop^|GGGG-HHHH-IIII^|2026-06-15^|299.00 >> C:\CLR_Test\licenses.txt';
GO

-- выполнение CLR-процедуры чтения файла
EXEC dbo.ReadLicenseFile 'C:\CLR_Test\licenses.txt';
GO

-- работа с пользовательским CLR-типом LicenseData
DECLARE @license dbo.LicenseData;

SET @license = dbo.LicenseData::Parse('1|Visual Studio|AAAA-BBBB-CCCC|2027-01-01|199.99');

SELECT 
    @license.ToString() AS LicenseAsString,
    @license.GetLicenseInfo() AS LicenseInfo,
    @license.IsExpired() AS IsExpired;
GO

-- проверка просроченной лицензии
DECLARE @expiredLicense dbo.LicenseData;

SET @expiredLicense = dbo.LicenseData::Parse('2|Microsoft Office|DDDD-EEEE-FFFF|2023-12-31|149.50');

SELECT 
    @expiredLicense.ToString() AS LicenseAsString,
    @expiredLicense.GetLicenseInfo() AS LicenseInfo,
    @expiredLicense.IsExpired() AS IsExpired;
GO
