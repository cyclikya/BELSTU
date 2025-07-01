--1
select ord.��������_������, max(ord.����_�������)[������������ ���� �������], count(ord.����������)[���������� �������]
	from ORDERS ord inner join PRODUCTS pr
		on ord.��������_������=pr.��������_������
		and pr.����������_��_������ > 150
		group by ord.��������_������;

--2
select *
	from (select Case 
			when ����_������� between 1 and  1600 then '���� < 1600'
			when ����_������� between 1600 and  2000 then '���� �� 1600 �� 2000'
			else '���� ������ 2000'
		 end [������� ���], count(*) [����������]
		 from ORDERS group by Case
			when ����_������� between 1 and  1600 then '���� < 1600'
			when ����_������� between 1600 and  2000 then '���� �� 1600 �� 2000'
			else '���� ������ 2000'
		 end) as T
				order by Case[������� ���] 
					when '���� ������ 2000' then 3
					when '���� �� 1600 �� 2000' then 2
					when '���� < 1600' then 1
					else 0
					end;

--3/4
select ord.��������_������, pr.����, round(avg(cast(ord.����_������� as float(4))), 2)[������� ���]
	from ORDERS ord join PRODUCTS pr
			on pr.��������_������ = ord.��������_������
	where ord.��������_������ in ('�����', '��������')
	group by ord.��������_������, pr.����

--5
select o.��������_������, sum(o.����������)[����� �������]
	from ORDERS o
	group by o.��������_������

--6
select o.��������_������, sum(o.����������)[����� �������]
	from ORDERS o
	group by o.��������_������
	having o.��������_������ in ('�����', '��������', '���������')
	order by [����� �������]
desc 

-- ��� 6-8


create view [�����]
	as select 
		year(����_�������) as [���],
		cast(month(����_�������) as nvarchar) as [�����],
		sum(����������) as [���������� ��������� �������]
	from ORDERS
	group by rollup
		(year(����_�������), 
		month(����_�������));

select * from [�����];

drop view [�����];