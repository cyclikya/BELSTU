--1
select ��������_������, ID_�������, avg(����_�������)[���]
	from ORDERS
	where not ID_������� is null
	group by rollup (��������_������, ID_�������)

--2
select ��������_������, ID_�������, avg(����_�������)[���]
	from ORDERS
	where not ID_������� is null
	group by cube (��������_������, ID_�������)

--3
select ��������_������, ID_�������
	from ORDERS
	where ID_������� > 3
		union
select ��������_������, ID_�������
	from ORDERS
	where ID_������� > 4

select ��������_������, ID_�������
	from ORDERS
	where ID_������� > 3
		union all
select ��������_������, ID_�������
	from ORDERS
	where ID_������� > 4

--4
select ��������_������, ID_�������
	from ORDERS
	where ID_������� > 3
		intersect
select ��������_������, ID_�������
	from ORDERS
	where ID_������� > 4

--5
select ��������_������, ID_�������
	from ORDERS
	where ID_������� > 3
		except
select ��������_������, ID_�������
	from ORDERS
	where ID_������� > 4