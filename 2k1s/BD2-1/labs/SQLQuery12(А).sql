-------A
--4 ������� ��������������� READ UN-COMMITED
set transaction isolation level read uncommitted;
begin tran;
-- t1: ������ ������ �� ���������
	print '������ ������ (�� ���������)';
	select * from Tb where product = 'carrot';

WAITFOR DELAY '00:00:05';

-- t2: ������ ������ (�� �������)
	print '������ ������ (�� �������)';
	select * from Tb where product = 'carrot';
commit tran;

--5 ������� ��������������� READ COMMITED
set transaction isolation level read committed;
begin tran;
    print '������ ������';
    select count from Tb where product = 'carrot';

WAITFOR DELAY '00:00:05';

    print '������ ������';
    select count from Tb where product = 'carrot';
commit tran;

--6 ������� ��������������� REPEATABLE READ
set transaction isolation level repeatable read;
begin tran;    
    print '������ ������';
    select count from Tb where product = 'carrot';

WAITFOR DELAY '00:00:05';
    
    print '������ ������';
    select count from Tb where product = 'carrot';
commit tran;

--7 ������� ��������������� SERIALIZABLE. 
set transaction isolation level serializable;
begin tran;
	print '������ ������';
	select count from Tb where product = 'carrot';

WAITFOR DELAY '00:00:05';

	print '������ ������';
	select count from Tb where product = 'carrot';
commit tran;

