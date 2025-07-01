-------B
--4
set transaction isolation level read committed;

begin tran;
-- t1: ������ ������ �� ���������
	print '������ ������ (�� ���������)';
	select * from Tb where product = 'carrot';
	
	update Tb set count = 10 where product = 'carrot';

-- t2: ������ ������, ������� ��� �������������� ������
	print '������ ������ (����� ���������)';
	select * from Tb where product = 'carrot';
commit tran;

--5
begin tran;
    update Tb set count = count - 5 where product = 'carrot';
    select * from Tb where product = 'carrot';
commit tran;

select * from Tb where product = 'carrot';

--6
begin tran;
    update Tb set count = count - 5 where product = 'carrot';
commit tran;

select * from Tb where product = 'carrot';

--7
set transaction isolation level read committed;
begin tran;
	print '������ ������';
	select count from Tb where product = 'carrot';

	update Tb set count = count + 5 where product = 'carrot';

	print '������ ������';
	select count from Tb where product = 'carrot';
commit tran;
