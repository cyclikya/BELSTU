--1
select ТОВАРЫ.Название_товара, ТОВАРЫ.Цена, ЗАКАЗЫ.Количество
	from ЗАКАЗЫ Inner Join ТОВАРЫ
	on ЗАКАЗЫ.Название_товара = ТОВАРЫ.Название_товара;

-- 2
select T1.ID_клиента, T1.Фамилия, T1.[E-mail], T2.ID_заказа, T2.Название_товара
	from КЛИЕНТЫ as T1 Inner Join ЗАКАЗЫ as T2
	on T1.ID_клиента = T2.ID_клиента and
		T1.[E-mail] like '%gmail%';

-- 3
select T2.ID_заказа, T1.Название_товара, T2.Количество, T1.Цена,  
	case
		when (T1.Цена between 0 and 15) then 'Дёшево'
		when (T1.Цена between 16 and 20) then 'Норм'
		when (T1.Цена between 21 and 30) then 'Дорого'
		else 'Тридорого'
	end Оценка_цены
	from ТОВАРЫ as T1 Inner Join ЗАКАЗЫ as T2
		on T1.Название_товара = T2.Название_товара
			order by T1.Название_товара;

-- 4
select isnull(КЛИЕНТЫ.Имя, '**') as [Name], ЗАКАЗЫ.Название_товара
	from КЛИЕНТЫ left outer join ЗАКАЗЫ
		on КЛИЕНТЫ.ID_Клиента = ЗАКАЗЫ.ID_Клиента;

-- 5 
select * 
	from КЛИЕНТЫ full outer join ЗАКАЗЫ
	on КЛИЕНТЫ.ID_Клиента = ЗАКАЗЫ.ID_клиента
	where 
    ЗАКАЗЫ.ID_заказа is null;

select * 
	from КЛИЕНТЫ full outer join ЗАКАЗЫ
	on КЛИЕНТЫ.ID_Клиента = ЗАКАЗЫ.ID_клиента
	where 
    КЛИЕНТЫ.Фамилия is null;

select * 
	from КЛИЕНТЫ full outer join ЗАКАЗЫ
	on КЛИЕНТЫ.ID_Клиента = ЗАКАЗЫ.ID_клиента
	where 
    КЛИЕНТЫ.ID_Клиента is not null and
	ЗАКАЗЫ.ID_заказа is not null;

-- 6
select t.Название_товара, t.Цена, z.Количество
	from ЗАКАЗЫ as z cross join ТОВАРЫ as t
	where z.Название_товара = t.Название_товара and
	t.Цена like '2%';
