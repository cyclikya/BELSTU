--1
select ������.��������_������, ������.����, ������.����������
	from ������ Inner Join ������
	on ������.��������_������ = ������.��������_������;

-- 2
select T1.ID_�������, T1.�������, T1.[E-mail], T2.ID_������, T2.��������_������
	from ������� as T1 Inner Join ������ as T2
	on T1.ID_������� = T2.ID_������� and
		T1.[E-mail] like '%gmail%';

-- 3
select T2.ID_������, T1.��������_������, T2.����������, T1.����,  
	case
		when (T1.���� between 0 and 15) then 'ĸ����'
		when (T1.���� between 16 and 20) then '����'
		when (T1.���� between 21 and 30) then '������'
		else '���������'
	end ������_����
	from ������ as T1 Inner Join ������ as T2
		on T1.��������_������ = T2.��������_������
			order by T1.��������_������;

-- 4
select isnull(�������.���, '**') as [Name], ������.��������_������
	from ������� left outer join ������
		on �������.ID_������� = ������.ID_�������;

-- 5 
select * 
	from ������� full outer join ������
	on �������.ID_������� = ������.ID_�������
	where 
    ������.ID_������ is null;

select * 
	from ������� full outer join ������
	on �������.ID_������� = ������.ID_�������
	where 
    �������.������� is null;

select * 
	from ������� full outer join ������
	on �������.ID_������� = ������.ID_�������
	where 
    �������.ID_������� is not null and
	������.ID_������ is not null;

-- 6
select t.��������_������, t.����, z.����������
	from ������ as z cross join ������ as t
	where z.��������_������ = t.��������_������ and
	t.���� like '2%';
