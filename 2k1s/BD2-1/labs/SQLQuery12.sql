create table Tb(product nvarchar(10) primary key, count int);
insert Tb
	values 
		('carrot', 15),
		('orange', 20),
		('potato', 25);
select * from Tb;
drop table Tb;

--1 ������� ����������
SET IMPLICIT_TRANSACTIONS on;

declare @c int = 2;
declare @k int = (select count(*) from Tb)

if @k = @c
	commit;
	else rollback;

SET IMPLICIT_TRANSACTIONS  off

if  exists (select * from  SYS.OBJECTS
	            where OBJECT_ID= object_id(N'DBO.Tb') )
	print '������� Tb ����';  
      else print '������� Tb ���'

--2 �������� ����������� ����� ���������� 
begin try
	begin tran
		delete Tb where count < 20;
		insert Tb values ('orange', 30);
		insert Tb values ('banana', 5,5);
	commit tran;
end try
begin catch
	print 'error: '+ case
	when error_number() = 2627 and patindex('%Tb%', error_message()) > 0
	then '������������ ������'
	else '����������� ������: '+ cast(error_number() as varchar(5))+ error_message()
	end;
	if @@trancount > 0 rollback tran ;
end catch;

--3 ���������� ��������� SAVE TRAN 
declare @point nvarchar(32);

begin try
    begin tran
    delete from Tb where product = 'apple';
    set @point = 'p1'; save transaction @point;

    insert into Tb values ('beetroot', 10);
    set @point = 'p2'; save tran @point;

	insert into Tb values ('carrot', 30);
    commit tran
end try
begin catch
    print '��������� ������: ' + error_message();
    if @@trancount > 0
    begin
        print '����� � ����������� �����: ' + @point;
        rollback tran @point;
        commit tran;
    end
end catch;

--8 ��������� ����������
begin tran MainTran;
    print '�������� ���������� ������';
    begin tran NestedTran;
        print '��������� ���������� ������';
        update Tb set count = count + 5 where product = 'carrot';        
	commit tran NestedTran;
commit tran MainTran;

select * from Tb;