create database [2024_UGORENKO_lb3]
-- 6 задание
on primary
(name =N'2024_UGORENKO_lb3_mdf', filename= N'C:\Users\vugor\OneDrive\Рабочий стол\2k1s\BD2-1\lab3\2024_UGORENKO_lb3_mdf.mdf',
size = 10240Kb, maxsize=UNLIMITED, filegrowth=1024Kb),
filegroup FG1
(name = N'2024_UGORENKO_lb3_gr1', filename = N'C:\Users\vugor\OneDrive\Рабочий стол\2k1s\BD2-1\lab3\2024_UGORENKO_lb3_gr1.ndf',
 size = 10240Kb, maxsize = 1Gb, filegrowth = 25%)
log on 
(name =N'2024_UGORENKO_lb3_log', filename= N'C:\Users\vugor\OneDrive\Рабочий стол\2k1s\BD2-1\lab3\2024_UGORENKO_lb3_log.ldf',
size=10240Kb, maxsize=2048Gb, filegrowth=10%)
go

use [2024_UGORENKO_lb3]
	create table КЛИЕНТЫ
	(
		ID_Клиента int primary key,
		Фамилия nvarchar(30),
		Имя nvarchar(30),
		Отчество nvarchar(30),
		Адрес nvarchar(30),
		Телефон int,
		[E-mail] nvarchar(30),
		Признак_скидки bit,
	)on FG1; --7 задание
	create table ТОВАРЫ
	(
		Название_товара nvarchar(30) primary key,
		Количество_на_складе int,
		Цена money,
		Единица_измерения nvarchar(30),
	) on FG1;
	create table ЗАКАЗЫ
	(
		ID_заказа int primary key,
		Название_товара nvarchar(30) foreign key references ТОВАРЫ(Название_товара),
		Количество int,
		ID_клиента int foreign key references КЛИЕНТЫ(ID_клиента),
		Дата_продажи date,
	)on FG1;

-- 3 задание
	alter table ЗАКАЗЫ add Итоговая_сумма money;
	alter table ЗАКАЗЫ drop column Итоговая_сумма;

-- 4 задание
	insert into КЛИЕНТЫ (ID_Клиента, Фамилия, Имя, Отчество, Адрес, Телефон, [E-mail], Признак_скидки)
		values
			(1, 'Иванов', 'Иван', 'Иванович', 'Москва', 1234567, 'ivanov@mail.ru', 0),
			(2, 'Петров', NULL , 'Петрович', 'Санкт-Петербург', 2345678, 'petrov@mail.ru', 0),
			(3, 'Сидоров', 'Сидор', 'Сидорович', 'Казань', 3456789, 'sidorov@mail.ru', 1),
			(4, 'Кузнецов', 'Кузьма', 'Кузьмич', 'Екатеринбург', 4567890, 'kuznetsov@mail.ru', 1),
			(5, 'Баранов', NULL, 'Викторович', 'Новосибирск', 5467925, 'baranov@gmail.ru', 0);
	insert into ТОВАРЫ (Название_товара, Количество_на_складе, Цена, Единица_измерения)
		values
			('Банан', 150, 30.00, 'кг'),
			('Картофель', 500, 15.00, 'кг'),
			('Морковь', 200, 20.00, 'кг'),
			('Яблоко', 100, 50.00, 'кг');
	insert into ЗАКАЗЫ (ID_заказа, Название_товара, Количество, ID_клиента, Дата_продажи)
		values
			(1, 'Яблоко', 1, 1, '2024-09-27'), 
			(2, 'Картофель', 2, 1, '2024-09-28'),
			(3, 'Морковь', 0.5, 1, '2024-09-29'),

			(4, 'Банан', 2, 2, '2024-09-27'),
			(5, 'Морковь', 1.5, 2, '2024-09-28'),

			(6, 'Картофель', 5, 3, '2024-09-27'),
			(7, 'Яблоко', 1, 3, '2024-09-28'),
			(8, 'Морковь', 2.5, 3, '2024-09-29'),

			(9, 'Картофель', 10, 4, '2024-09-27'),
			(10, 'Банан', 2, 4, '2024-09-28'),
			(11, 'Банан', 4, NULL, '2024-09-30');
;

-- 5 задание
use [2024_UGORENKO_lb3]
	select Название_товара, Цена from ТОВАРЫ;
	select count(*) from ТОВАРЫ;
	update ЗАКАЗЫ set Дата_продажи = '2024-09-29' where ID_заказа = 10;
	select * from ЗАКАЗЫ;

	





USE [master];
GO
SELECT name, physical_name AS current_file_location
FROM sys.master_files
WHERE database_id = DB_ID('2024_UGORENKO_lb3');