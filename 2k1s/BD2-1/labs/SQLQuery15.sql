--1
create table TR_AUDIT(
	ID int identity,
	STMT varchar(20)
		check (STMT in('INS', 'DEL', 'UPD')),
	TRNAME varchar(50),
	CC varchar(300)
)

select * from TR_AUDIT;

create trigger TR_PRODUCTS_INS
on PRODUCTS
after insert
as
begin
    insert into TR_AUDIT (STMT, TRNAME, CC)
		select 
			'INS',                    
			'PRODUCTS',              
			'Название_товара: ' + i.Название_товара + ', ' + 
			'Количество: ' + cast(i.Количество_на_складе as varchar(10)) + ', ' + 
			'Цена: ' + cast(i.Цена as varchar(10)) + ', ' + 
			'Единица_измерения: ' + i.Единица_измерения
		from inserted i;
end;

insert PRODUCTS values ('Персик', 50, 40.00, 'кг')

select * from TR_AUDIT;

--2
create trigger TR_PRODUCTS_DEL
on PRODUCTS
after delete
as
begin
    insert into TR_AUDIT (STMT, TRNAME, CC)
    select 
        'DEL',                      
        'PRODUCTS',               
        'Название_товара: ' + d.Название_товара + ', ' + 
        'Количество: ' + cast(d.Количество_на_складе as varchar(10)) + ', ' + 
        'Цена: ' + cast(d.Цена as varchar(10)) + ', ' + 
        'Единица_измерения: ' + d.Единица_измерения
    from deleted d;
end;

delete PRODUCTS where Название_товара = 'Персик';

select * from TR_AUDIT;

--3
create trigger TR_PRODUCTS_UPD
on PRODUCTS
after update
as
begin
    insert into TR_AUDIT (STMT, TRNAME, CC)
    select 
        'UPD',                 
        'PRODUCTS',                
        'Старое название: ' + d.Название_товара + ', ' + 
        'Новое название: ' + i.Название_товара + ', ' + 
        'Старое количество: ' + cast(d.Количество_на_складе as varchar(10)) + ', ' + 
        'Новое количество: ' + cast(i.Количество_на_складе as varchar(10)) + ', ' + 
        'Старая цена: ' + cast(d.Цена as varchar(10)) + ', ' + 
        'Новая цена: ' + cast(i.Цена as varchar(10)) + ', ' + 
        'Старая единица измерения: ' + d.Единица_измерения + ', ' + 
        'Новая единица измерения: ' + i.Единица_измерения
    from deleted d
    join inserted i on d.Название_товара = i.Название_товара;  
end;

update PRODUCTS
	set Количество_на_складе = 150, Цена = 35.00
		where Название_товара = 'Банан';

select * from TR_AUDIT;

--4
create trigger TR_PRODUCTS
on PRODUCTS
after insert, delete, update
as
begin
    if exists (select 1 from inserted)
    begin
        insert into TR_AUDIT (STMT, TRNAME, CC)
        select 
            'INS', 
            'PRODUCTS',
            'Вставлены данные: Название товара = ' + i.Название_товара + 
            ', Количество = ' + cast(i.Количество_на_складе as varchar(10)) + 
            ', Цена = ' + cast(i.Цена as varchar(10)) + 
            ', Единица измерения = ' + i.Единица_измерения
        from inserted i;
    end

    if exists (select 1 from deleted)
    begin
        insert into TR_AUDIT (STMT, TRNAME, CC)
        select 
            'DEL',
            'PRODUCTS',
            'Удалены данные: Название товара = ' + d.Название_товара + 
            ', Количество = ' + cast(d.Количество_на_складе as varchar(10)) + 
            ', Цена = ' + cast(d.Цена as varchar(10)) + 
            ', Единица измерения = ' + d.Единица_измерения
        from deleted d;
    end

    if exists (select 1 from inserted) and exists (select 1 from deleted)
    begin
        insert into TR_AUDIT (STMT, TRNAME, CC)
        select 
            'UPD', 
            'PRODUCTS',
            'Обновлены данные: Название товара (старое) = ' + d.Название_товара + 
            ', (новое) = ' + i.Название_товара + 
            ', Количество (старое) = ' + cast(d.Количество_на_складе as varchar(10)) + 
            ', (новое) = ' + cast(i.Количество_на_складе as varchar(10)) + 
            ', Цена (старое) = ' + cast(d.Цена as varchar(10)) + 
            ', (новое) = ' + cast(i.Цена as varchar(10)) + 
            ', Единица измерения (старое) = ' + d.Единица_измерения + 
            ', (новое) = ' + i.Единица_измерения
        from inserted i
        join deleted d on i.Название_товара = d.Название_товара;
    end
end;

insert into PRODUCTS (Название_товара, Количество_на_складе, Цена, Единица_измерения)
	values ('Груша', 120, 45.00, 'кг');

update PRODUCTS
	set Количество_на_складе = 150, Цена = 50.00
		where Название_товара = 'Груша';

delete from PRODUCTS
	where Название_товара = 'Груша';

select * from TR_AUDIT;

--5
alter table PRODUCTS add constraint Цена check(Цена >= 0)

update PRODUCTS set Цена = -10 WHERE Цена = 0;

select * from TR_AUDIT;

--6
create trigger TR_CLIENTS_DEL1
on CLIENTS
after delete
as
begin
    print 'Триггер 1';
    declare @a nvarchar(100) = (select Фамилия + ', ' + Имя from deleted);
    insert into TR_AUDIT (STMT, TRNAME, CC)
    values ('DEL', 'TR_CLIENTS_DEL1', 'Deleted client: ' + @a);
end;

go

create trigger TR_CLIENTS_DEL2
on CLIENTS
after delete
as
begin
    print 'Триггер 2';
    declare @a nvarchar(100) = (select Фамилия + ', ' + Имя from deleted);
    insert into TR_AUDIT (STMT, TRNAME, CC)
    values ('DEL', 'TR_CLIENTS_DEL2', 'Deleted client: ' + @a);
end;

go

create trigger TR_CLIENTS_DEL3
on CLIENTS
after delete
as
begin
    print 'Триггер 3';
    declare @a nvarchar(100) = (select Фамилия + ', ' + Имя from deleted);
    insert into TR_AUDIT (STMT, TRNAME, CC)
    values ('DEL', 'TR_CLIENTS_DEL3', 'Deleted client: ' + @a);
end;

go

exec sp_settriggerorder @triggername = 'TR_CLIENTS_DEL1', @order = 'None', @stmttype = 'DELETE';
exec sp_settriggerorder @triggername = 'TR_CLIENTS_DEL2', @order = 'Last', @stmttype = 'DELETE';
exec sp_settriggerorder @triggername = 'TR_CLIENTS_DEL3', @order = 'First', @stmttype = 'DELETE';

insert into CLIENTS (ID_клиента, Фамилия, Имя, Отчество, Адрес, телефон, [E-mail])
	values (8, 'Иванов', 'Иван', 'Иванович', 'Москва', '1234567', 'ivanov@mail.com');

delete from CLIENTS where ID_клиента = 8;

select * from TR_AUDIT;

select t.name, e.type_desc
	from sys.triggers t join sys.trigger_events e 
		on t.object_id = e.object_id
	where OBJECT_NAME(t.parent_id) = 'CLIENTS' 
		and e.type_desc = 'DELETE';

drop trigger TR_CLIENTS_DEL1;
drop trigger TR_CLIENTS_DEL2;
drop trigger TR_CLIENTS_DEL3;

--7
create trigger TR_CLIENTS_TRAN
on CLIENTS
after insert, delete, update
as
begin
    declare @count int = (select count(*) from CLIENTS);

    if (@count > 7)
    begin
        raiserror('Количество записей в CLIENTS превышает допустимое значение (7)', 10, 1);
        rollback;
    end;

    insert into TR_AUDIT (STMT, TRNAME, CC)
    values (
        case 
            when exists (select * from inserted) and not exists (select * from deleted) then 'INS'
            when not exists (select * from inserted) and exists (select * from deleted) then 'DEL'
            else 'UPD'
        end,
        'TR_CLIENTS_TRAN',
        'Количество записей проверено'
    );
end;

go

insert into CLIENTS (ID_клиента, Фамилия, Имя, Отчество, Адрес, телефон, [E-mail])
values (8, 'Кузнецов', 'Кузьма', 'Кузьмич', 'Москва, ул. Гагарина, 8', '1234567898', 'kuznetsov@mail.com');

select * from CLIENTS; 
select * from TR_AUDIT;

drop trigger TR_CLIENTS_TRAN;

--8
alter trigger TR_CLIENTS_INSTEAD_OF
on CLIENTS
instead of delete
as
begin
    raiserror(N'Удаление строк в таблице CLIENTS запрещено!', 10, 1);
    return;
end;

delete from CLIENTS where ID_Клиента = 7;

select * from CLIENTS;

drop trigger TR_CLIENTS_INSTEAD_OF;

DROP TRIGGER TR_PRODUCTS_INS;
DROP TRIGGER TR_PRODUCTS_DEL;
DROP TRIGGER TR_PRODUCTS_UPD;
DROP TRIGGER TR_PRODUCTS;

DROP TRIGGER TR_CLIENTS_DEL1;
DROP TRIGGER TR_CLIENTS_DEL2;
DROP TRIGGER TR_CLIENTS_DEL3;
DROP TRIGGER TR_CLIENTS_TRAN;
DROP TRIGGER TR_CLIENTS_INSTEAD_OF;

--9
create trigger ddl_prevent_create_drop_table
on database
for create_table, drop_table
as
begin
    declare @eventType nvarchar(50), @objectName nvarchar(50), @objectType nvarchar(50);

    set @eventType = eventdata().value('(/EVENT_INSTANCE/EventType)[1]', 'nvarchar(50)');
    set @objectName = eventdata().value('(/EVENT_INSTANCE/ObjectName)[1]', 'nvarchar(50)');
    set @objectType = eventdata().value('(/EVENT_INSTANCE/ObjectType)[1]', 'nvarchar(50)');

    print 'Тип события: ' + @eventType;
    print 'Имя объекта: ' + @objectName;
    print 'Тип объекта: ' + @objectType;

    if @eventType in ('CREATE_TABLE', 'DROP_TABLE')
    begin
        raiserror('Операция запрещена триггером: создание или удаление таблиц не разрешены.', 16, 1);
        rollback;
    end
end;
go

begin try
    create table test_table (id int primary key);
end try
begin catch
    print 'Ошибка: ' + error_message();
end catch;

begin try
    drop table test_table;
end try
begin catch
    print 'Ошибка: ' + error_message();
end catch;

drop trigger ddl_prevent_create_drop_table on database;
