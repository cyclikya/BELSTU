--1
select ord.Название_товара, max(ord.Цена_продажи)[Максимальная цена продажи], count(ord.Количество)[Количество заказов]
	from ORDERS ord inner join PRODUCTS pr
		on ord.Название_товара=pr.Название_товара
		and pr.Количество_на_складе > 150
		group by ord.Название_товара;

--2
select *
	from (select Case 
			when Цена_продажи between 1 and  1600 then 'цена < 1600'
			when Цена_продажи between 1600 and  2000 then 'цена от 1600 до 2000'
			else 'цена больше 2000'
		 end [Пределы цен], count(*) [количество]
		 from ORDERS group by Case
			when Цена_продажи between 1 and  1600 then 'цена < 1600'
			when Цена_продажи between 1600 and  2000 then 'цена от 1600 до 2000'
			else 'цена больше 2000'
		 end) as T
				order by Case[Пределы цен] 
					when 'цена больше 2000' then 3
					when 'цена от 1600 до 2000' then 2
					when 'цена < 1600' then 1
					else 0
					end;

--3/4
select ord.Название_товара, pr.Цена, round(avg(cast(ord.Цена_продажи as float(4))), 2)[Средний чек]
	from ORDERS ord join PRODUCTS pr
			on pr.Название_товара = ord.Название_товара
	where ord.Название_товара in ('Банан', 'Апельсин')
	group by ord.Название_товара, pr.Цена

--5
select o.Название_товара, sum(o.Количество)[Всего продано]
	from ORDERS o
	group by o.Название_товара

--6
select o.Название_товара, sum(o.Количество)[Всего продано]
	from ORDERS o
	group by o.Название_товара
	having o.Название_товара in ('Банан', 'Апельсин', 'Картофель')
	order by [Всего продано]
desc 

-- доп 6-8


create view [Отчет]
	as select 
		year(Дата_продажи) as [Год],
		cast(month(Дата_продажи) as nvarchar) as [Месяц],
		sum(Количество) as [Количество проданных товаров]
	from ORDERS
	group by rollup
		(year(Дата_продажи), 
		month(Дата_продажи));

select * from [Отчет];

drop view [Отчет];