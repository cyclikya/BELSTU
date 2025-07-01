-- 1
create view [Заказанные_товары]
	as select ID_заказа[ID],
		Название_товара[Товар],
		sum(Количество)[Купленное количество],
		sum(Цена_продажи)[Прибыль с товара] from ORDERS
											group by ID_заказа, Название_товара;
select * 
	from [Заказанные_товары];

drop view [Заказанные_товары];

-- 2
create view [Частые_покупатели]
	as select cl.ID_Клиента, cl.Фамилия, cl.Имя
		from ORDERS ord join CLIENTS cl
			on ord.ID_клиента = cl.ID_Клиента;

select * from [Частые_покупатели]

drop view [Частые_покупатели];

-- 3
create view [Догогие товары]
	as select Название_товара, Цена
		from PRODUCTS
		where Цена > 30;

select * from [Догогие товары];

insert [Догогие товары] values('Ананас', 55);

select * from [Догогие товары];

delete from [Догогие товары] where Название_товара = 'Ананас';
update [Догогие товары] set Цена = 100 where Название_товара = 'Огурец';

select * from [Догогие товары];

drop view [Догогие товары];

-- 4
create view [Догогие товары]
	as select Название_товара, Цена
		from PRODUCTS
		where Цена > 30 with check option;

select * from [Догогие товары];

insert [Догогие товары] values('Ананас', 10);

select * from [Догогие товары];

drop view [Догогие товары];

--5
create view [Популярные товары]
	as select top 5 Название_товара, Количество
		from ORDERS
		order by Количество desc;
select * from [Популярные товары];

drop view [Популярные товары];

--6
create view [Частые_покупатели] with schemabinding 
	as select cl.ID_Клиента, cl.Фамилия, cl.Имя
		from ORDERS ord join CLIENTS cl
			on ord.ID_клиента = cl.ID_Клиента;

insert [Частые_покупатели] values(1, 'Курчанов', 'Иван');

drop view [Частые_покупатели];
