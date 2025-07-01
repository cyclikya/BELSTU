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
			'��������_������: ' + i.��������_������ + ', ' + 
			'����������: ' + cast(i.����������_��_������ as varchar(10)) + ', ' + 
			'����: ' + cast(i.���� as varchar(10)) + ', ' + 
			'�������_���������: ' + i.�������_���������
		from inserted i;
end;

insert PRODUCTS values ('������', 50, 40.00, '��')

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
        '��������_������: ' + d.��������_������ + ', ' + 
        '����������: ' + cast(d.����������_��_������ as varchar(10)) + ', ' + 
        '����: ' + cast(d.���� as varchar(10)) + ', ' + 
        '�������_���������: ' + d.�������_���������
    from deleted d;
end;

delete PRODUCTS where ��������_������ = '������';

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
        '������ ��������: ' + d.��������_������ + ', ' + 
        '����� ��������: ' + i.��������_������ + ', ' + 
        '������ ����������: ' + cast(d.����������_��_������ as varchar(10)) + ', ' + 
        '����� ����������: ' + cast(i.����������_��_������ as varchar(10)) + ', ' + 
        '������ ����: ' + cast(d.���� as varchar(10)) + ', ' + 
        '����� ����: ' + cast(i.���� as varchar(10)) + ', ' + 
        '������ ������� ���������: ' + d.�������_��������� + ', ' + 
        '����� ������� ���������: ' + i.�������_���������
    from deleted d
    join inserted i on d.��������_������ = i.��������_������;  
end;

update PRODUCTS
	set ����������_��_������ = 150, ���� = 35.00
		where ��������_������ = '�����';

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
            '��������� ������: �������� ������ = ' + i.��������_������ + 
            ', ���������� = ' + cast(i.����������_��_������ as varchar(10)) + 
            ', ���� = ' + cast(i.���� as varchar(10)) + 
            ', ������� ��������� = ' + i.�������_���������
        from inserted i;
    end

    if exists (select 1 from deleted)
    begin
        insert into TR_AUDIT (STMT, TRNAME, CC)
        select 
            'DEL',
            'PRODUCTS',
            '������� ������: �������� ������ = ' + d.��������_������ + 
            ', ���������� = ' + cast(d.����������_��_������ as varchar(10)) + 
            ', ���� = ' + cast(d.���� as varchar(10)) + 
            ', ������� ��������� = ' + d.�������_���������
        from deleted d;
    end

    if exists (select 1 from inserted) and exists (select 1 from deleted)
    begin
        insert into TR_AUDIT (STMT, TRNAME, CC)
        select 
            'UPD', 
            'PRODUCTS',
            '��������� ������: �������� ������ (������) = ' + d.��������_������ + 
            ', (�����) = ' + i.��������_������ + 
            ', ���������� (������) = ' + cast(d.����������_��_������ as varchar(10)) + 
            ', (�����) = ' + cast(i.����������_��_������ as varchar(10)) + 
            ', ���� (������) = ' + cast(d.���� as varchar(10)) + 
            ', (�����) = ' + cast(i.���� as varchar(10)) + 
            ', ������� ��������� (������) = ' + d.�������_��������� + 
            ', (�����) = ' + i.�������_���������
        from inserted i
        join deleted d on i.��������_������ = d.��������_������;
    end
end;

insert into PRODUCTS (��������_������, ����������_��_������, ����, �������_���������)
	values ('�����', 120, 45.00, '��');

update PRODUCTS
	set ����������_��_������ = 150, ���� = 50.00
		where ��������_������ = '�����';

delete from PRODUCTS
	where ��������_������ = '�����';

select * from TR_AUDIT;

--5
alter table PRODUCTS add constraint ���� check(���� >= 0)

update PRODUCTS set ���� = -10 WHERE ���� = 0;

select * from TR_AUDIT;

--6
create trigger TR_CLIENTS_DEL1
on CLIENTS
after delete
as
begin
    print '������� 1';
    declare @a nvarchar(100) = (select ������� + ', ' + ��� from deleted);
    insert into TR_AUDIT (STMT, TRNAME, CC)
    values ('DEL', 'TR_CLIENTS_DEL1', 'Deleted client: ' + @a);
end;

go

create trigger TR_CLIENTS_DEL2
on CLIENTS
after delete
as
begin
    print '������� 2';
    declare @a nvarchar(100) = (select ������� + ', ' + ��� from deleted);
    insert into TR_AUDIT (STMT, TRNAME, CC)
    values ('DEL', 'TR_CLIENTS_DEL2', 'Deleted client: ' + @a);
end;

go

create trigger TR_CLIENTS_DEL3
on CLIENTS
after delete
as
begin
    print '������� 3';
    declare @a nvarchar(100) = (select ������� + ', ' + ��� from deleted);
    insert into TR_AUDIT (STMT, TRNAME, CC)
    values ('DEL', 'TR_CLIENTS_DEL3', 'Deleted client: ' + @a);
end;

go

exec sp_settriggerorder @triggername = 'TR_CLIENTS_DEL1', @order = 'None', @stmttype = 'DELETE';
exec sp_settriggerorder @triggername = 'TR_CLIENTS_DEL2', @order = 'Last', @stmttype = 'DELETE';
exec sp_settriggerorder @triggername = 'TR_CLIENTS_DEL3', @order = 'First', @stmttype = 'DELETE';

insert into CLIENTS (ID_�������, �������, ���, ��������, �����, �������, [E-mail])
	values (8, '������', '����', '��������', '������', '1234567', 'ivanov@mail.com');

delete from CLIENTS where ID_������� = 8;

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
        raiserror('���������� ������� � CLIENTS ��������� ���������� �������� (7)', 10, 1);
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
        '���������� ������� ���������'
    );
end;

go

insert into CLIENTS (ID_�������, �������, ���, ��������, �����, �������, [E-mail])
values (8, '��������', '������', '�������', '������, ��. ��������, 8', '1234567898', 'kuznetsov@mail.com');

select * from CLIENTS; 
select * from TR_AUDIT;

drop trigger TR_CLIENTS_TRAN;

--8
alter trigger TR_CLIENTS_INSTEAD_OF
on CLIENTS
instead of delete
as
begin
    raiserror(N'�������� ����� � ������� CLIENTS ���������!', 10, 1);
    return;
end;

delete from CLIENTS where ID_������� = 7;

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

    print '��� �������: ' + @eventType;
    print '��� �������: ' + @objectName;
    print '��� �������: ' + @objectType;

    if @eventType in ('CREATE_TABLE', 'DROP_TABLE')
    begin
        raiserror('�������� ��������� ���������: �������� ��� �������� ������ �� ���������.', 16, 1);
        rollback;
    end
end;
go

begin try
    create table test_table (id int primary key);
end try
begin catch
    print '������: ' + error_message();
end catch;

begin try
    drop table test_table;
end try
begin catch
    print '������: ' + error_message();
end catch;

drop trigger ddl_prevent_create_drop_table on database;
