--1 
select ord.ID_заказа, ord.Название_товара, ord.ID_клиента, ord.Дата_продажи, pr.Цена
	from ORDERS as ord, PRODUCTS as pr 
	where ord.Название_товара = pr.Название_товара
		and
		ord.ID_клиента in (select cl.ID_Клиента
							from CLIENTS as cl
							where (cl.Адрес like '%Москва%'));

--2
select ord.ID_заказа, ord.Название_товара, ord.ID_клиента, ord.Дата_продажи, pr.Цена
	from ORDERS as ord join PRODUCTS as pr 
	on ord.Название_товара = pr.Название_товара
	where ord.ID_клиента in (select cl.ID_Клиента
								from CLIENTS as cl
								where (cl.Адрес like '%Москва%'));
--3
select ord.ID_заказа, ord.Название_товара, ord.ID_клиента, ord.Дата_продажи, pr.Цена
	from ORDERS as ord join PRODUCTS as pr 
	on ord.Название_товара = pr.Название_товара
		join CLIENTS as cl
		on ord.ID_клиента = cl.ID_Клиента
		where (cl.Адрес like '%Москва%');

--4
select Название_товара, Цена_продажи
	from ORDERS ord
		where ID_клиента = (select top(1) ID_клиента
								from ORDERS ordd
								where ordd.Название_товара = ord.Название_товара
									order by Цена_продажи asc);

--5
select pr.Название_товара
	from PRODUCTS pr
	where not exists (select * from ORDERS ord
						where ord.Название_товара = pr.Название_товара);

--6
select top(1) 
	(select avg(Цена_продажи) from ORDERS
		where ORDERS.Название_товара like 'Яблоко')[Средний чек яблок],
	(select avg(Цена_продажи) from ORDERS
		where ORDERS.Название_товара like 'Картофель')[Средний чек картофеля],
		(select avg(Цена_продажи) from ORDERS
		where ORDERS.Название_товара like 'Морковь')[Средний чек моркови]
from ORDERS;

--7
select Название_товара, Количество
	from ORDERS t1
	where Количество >= all (select Количество from ORDERS t2
								where t2.Название_товара like 'Яблоко' )

--8
select t1.ID_Клиента, t1.Фамилия, t1.Имя, t1.Отчество
	from CLIENTS t1
		where ID_Клиента = ANY (select t2.ID_клиента
									from ORDERS t2
									where t2.Цена_продажи > 2000);




select *
	from CLIENTS c1
	where (select count(*)
			from CLIENTS c2
			where c2.Фамилия <= c1.Фамилия
	) between 3 and 5
	order by Фамилия