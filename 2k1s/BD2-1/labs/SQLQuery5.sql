--1 
select ord.ID_������, ord.��������_������, ord.ID_�������, ord.����_�������, pr.����
	from ORDERS as ord, PRODUCTS as pr 
	where ord.��������_������ = pr.��������_������
		and
		ord.ID_������� in (select cl.ID_�������
							from CLIENTS as cl
							where (cl.����� like '%������%'));

--2
select ord.ID_������, ord.��������_������, ord.ID_�������, ord.����_�������, pr.����
	from ORDERS as ord join PRODUCTS as pr 
	on ord.��������_������ = pr.��������_������
	where ord.ID_������� in (select cl.ID_�������
								from CLIENTS as cl
								where (cl.����� like '%������%'));
--3
select ord.ID_������, ord.��������_������, ord.ID_�������, ord.����_�������, pr.����
	from ORDERS as ord join PRODUCTS as pr 
	on ord.��������_������ = pr.��������_������
		join CLIENTS as cl
		on ord.ID_������� = cl.ID_�������
		where (cl.����� like '%������%');

--4
select ��������_������, ����_�������
	from ORDERS ord
		where ID_������� = (select top(1) ID_�������
								from ORDERS ordd
								where ordd.��������_������ = ord.��������_������
									order by ����_������� asc);

--5
select pr.��������_������
	from PRODUCTS pr
	where not exists (select * from ORDERS ord
						where ord.��������_������ = pr.��������_������);

--6
select top(1) 
	(select avg(����_�������) from ORDERS
		where ORDERS.��������_������ like '������')[������� ��� �����],
	(select avg(����_�������) from ORDERS
		where ORDERS.��������_������ like '���������')[������� ��� ���������],
		(select avg(����_�������) from ORDERS
		where ORDERS.��������_������ like '�������')[������� ��� �������]
from ORDERS;

--7
select ��������_������, ����������
	from ORDERS t1
	where ���������� >= all (select ���������� from ORDERS t2
								where t2.��������_������ like '������' )

--8
select t1.ID_�������, t1.�������, t1.���, t1.��������
	from CLIENTS t1
		where ID_������� = ANY (select t2.ID_�������
									from ORDERS t2
									where t2.����_������� > 2000);




select *
	from CLIENTS c1
	where (select count(*)
			from CLIENTS c2
			where c2.������� <= c1.�������
	) between 3 and 5
	order by �������