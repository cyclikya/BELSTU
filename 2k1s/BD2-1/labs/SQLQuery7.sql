--1
select Название_товара, ID_клиента, avg(Цена_продажи)[чек]
	from ORDERS
	where not ID_клиента is null
	group by rollup (Название_товара, ID_клиента)

--2
select Название_товара, ID_клиента, avg(Цена_продажи)[чек]
	from ORDERS
	where not ID_клиента is null
	group by cube (Название_товара, ID_клиента)

--3
select Название_товара, ID_клиента
	from ORDERS
	where ID_клиента > 3
		union
select Название_товара, ID_клиента
	from ORDERS
	where ID_клиента > 4

select Название_товара, ID_клиента
	from ORDERS
	where ID_клиента > 3
		union all
select Название_товара, ID_клиента
	from ORDERS
	where ID_клиента > 4

--4
select Название_товара, ID_клиента
	from ORDERS
	where ID_клиента > 3
		intersect
select Название_товара, ID_клиента
	from ORDERS
	where ID_клиента > 4

--5
select Название_товара, ID_клиента
	from ORDERS
	where ID_клиента > 3
		except
select Название_товара, ID_клиента
	from ORDERS
	where ID_клиента > 4