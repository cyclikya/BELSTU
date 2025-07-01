-------A
--4 уровень изолированности READ UN-COMMITED
set transaction isolation level read uncommitted;
begin tran;
-- t1: Первое чтение до изменений
	print 'Первое чтение (до изменений)';
	select * from Tb where product = 'carrot';

WAITFOR DELAY '00:00:05';

-- t2: Второе чтение (до коммита)
	print 'Второе чтение (до коммита)';
	select * from Tb where product = 'carrot';
commit tran;

--5 уровнем изолированности READ COMMITED
set transaction isolation level read committed;
begin tran;
    print 'Первое чтение';
    select count from Tb where product = 'carrot';

WAITFOR DELAY '00:00:05';

    print 'Второе чтение';
    select count from Tb where product = 'carrot';
commit tran;

--6 уровень изолированности REPEATABLE READ
set transaction isolation level repeatable read;
begin tran;    
    print 'Первое чтение';
    select count from Tb where product = 'carrot';

WAITFOR DELAY '00:00:05';
    
    print 'Второе чтение';
    select count from Tb where product = 'carrot';
commit tran;

--7 уровень изолированности SERIALIZABLE. 
set transaction isolation level serializable;
begin tran;
	print 'Первое чтение';
	select count from Tb where product = 'carrot';

WAITFOR DELAY '00:00:05';

	print 'Второе чтение';
	select count from Tb where product = 'carrot';
commit tran;

