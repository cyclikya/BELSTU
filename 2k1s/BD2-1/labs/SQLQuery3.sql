create database [2024_UGORENKO_lb3]
-- 6 �������
on primary
(name =N'2024_UGORENKO_lb3_mdf', filename= N'C:\Users\vugor\OneDrive\������� ����\2k1s\BD2-1\lab3\2024_UGORENKO_lb3_mdf.mdf',
size = 10240Kb, maxsize=UNLIMITED, filegrowth=1024Kb),
filegroup FG1
(name = N'2024_UGORENKO_lb3_gr1', filename = N'C:\Users\vugor\OneDrive\������� ����\2k1s\BD2-1\lab3\2024_UGORENKO_lb3_gr1.ndf',
 size = 10240Kb, maxsize = 1Gb, filegrowth = 25%)
log on 
(name =N'2024_UGORENKO_lb3_log', filename= N'C:\Users\vugor\OneDrive\������� ����\2k1s\BD2-1\lab3\2024_UGORENKO_lb3_log.ldf',
size=10240Kb, maxsize=2048Gb, filegrowth=10%)
go

use [2024_UGORENKO_lb3]
	create table �������
	(
		ID_������� int primary key,
		������� nvarchar(30),
		��� nvarchar(30),
		�������� nvarchar(30),
		����� nvarchar(30),
		������� int,
		[E-mail] nvarchar(30),
		�������_������ bit,
	)on FG1; --7 �������
	create table ������
	(
		��������_������ nvarchar(30) primary key,
		����������_��_������ int,
		���� money,
		�������_��������� nvarchar(30),
	) on FG1;
	create table ������
	(
		ID_������ int primary key,
		��������_������ nvarchar(30) foreign key references ������(��������_������),
		���������� int,
		ID_������� int foreign key references �������(ID_�������),
		����_������� date,
	)on FG1;

-- 3 �������
	alter table ������ add ��������_����� money;
	alter table ������ drop column ��������_�����;

-- 4 �������
	insert into ������� (ID_�������, �������, ���, ��������, �����, �������, [E-mail], �������_������)
		values
			(1, '������', '����', '��������', '������', 1234567, 'ivanov@mail.ru', 0),
			(2, '������', NULL , '��������', '�����-���������', 2345678, 'petrov@mail.ru', 0),
			(3, '�������', '�����', '���������', '������', 3456789, 'sidorov@mail.ru', 1),
			(4, '��������', '������', '�������', '������������', 4567890, 'kuznetsov@mail.ru', 1),
			(5, '�������', NULL, '����������', '�����������', 5467925, 'baranov@gmail.ru', 0);
	insert into ������ (��������_������, ����������_��_������, ����, �������_���������)
		values
			('�����', 150, 30.00, '��'),
			('���������', 500, 15.00, '��'),
			('�������', 200, 20.00, '��'),
			('������', 100, 50.00, '��');
	insert into ������ (ID_������, ��������_������, ����������, ID_�������, ����_�������)
		values
			(1, '������', 1, 1, '2024-09-27'), 
			(2, '���������', 2, 1, '2024-09-28'),
			(3, '�������', 0.5, 1, '2024-09-29'),

			(4, '�����', 2, 2, '2024-09-27'),
			(5, '�������', 1.5, 2, '2024-09-28'),

			(6, '���������', 5, 3, '2024-09-27'),
			(7, '������', 1, 3, '2024-09-28'),
			(8, '�������', 2.5, 3, '2024-09-29'),

			(9, '���������', 10, 4, '2024-09-27'),
			(10, '�����', 2, 4, '2024-09-28'),
			(11, '�����', 4, NULL, '2024-09-30');
;

-- 5 �������
use [2024_UGORENKO_lb3]
	select ��������_������, ���� from ������;
	select count(*) from ������;
	update ������ set ����_������� = '2024-09-29' where ID_������ = 10;
	select * from ������;

	





USE [master];
GO
SELECT name, physical_name AS current_file_location
FROM sys.master_files
WHERE database_id = DB_ID('2024_UGORENKO_lb3');