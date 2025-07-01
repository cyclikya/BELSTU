-- 1
create view [����������_������]
	as select ID_������[ID],
		��������_������[�����],
		sum(����������)[��������� ����������],
		sum(����_�������)[������� � ������] from ORDERS
											group by ID_������, ��������_������;
select * 
	from [����������_������];

drop view [����������_������];

-- 2
create view [������_����������]
	as select cl.ID_�������, cl.�������, cl.���
		from ORDERS ord join CLIENTS cl
			on ord.ID_������� = cl.ID_�������;

select * from [������_����������]

drop view [������_����������];

-- 3
create view [������� ������]
	as select ��������_������, ����
		from PRODUCTS
		where ���� > 30;

select * from [������� ������];

insert [������� ������] values('������', 55);

select * from [������� ������];

delete from [������� ������] where ��������_������ = '������';
update [������� ������] set ���� = 100 where ��������_������ = '������';

select * from [������� ������];

drop view [������� ������];

-- 4
create view [������� ������]
	as select ��������_������, ����
		from PRODUCTS
		where ���� > 30 with check option;

select * from [������� ������];

insert [������� ������] values('������', 10);

select * from [������� ������];

drop view [������� ������];

--5
create view [���������� ������]
	as select top 5 ��������_������, ����������
		from ORDERS
		order by ���������� desc;
select * from [���������� ������];

drop view [���������� ������];

--6
create view [������_����������] with schemabinding 
	as select cl.ID_�������, cl.�������, cl.���
		from ORDERS ord join CLIENTS cl
			on ord.ID_������� = cl.ID_�������;

insert [������_����������] values(1, '��������', '����');

drop view [������_����������];
