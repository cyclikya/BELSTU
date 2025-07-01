---1
select *
	from ORDERS 
	where ORDER_DATE > '2007-05-01';

--2
select *
	from OFFICES
	where REGION = 'Western' and MGR = 108;

--3
select *
	from ORDERS
	where ORDER_DATE between '2007-01-01' and '2007-12-31';		--!!!

--4
select *
	from OFFICES
	where OFFICE = 11 or OFFICE = 13 or OFFICE = 21;

--5
select *
	from SALESREPS
	where MANAGER is NULL;										--!!!

--6
select *
	from OFFICES
	where REGION like 'East%';

--7
select NAME, HIRE_DATE
	from SALESREPS
	order by AGE asc;												

--8
select *
	from ORDERS
	order by AMOUNT desc, QTY asc;								--!!! asc и desc

--9
select top(5) *
	from PRODUCTS
	order by PRICE desc

--10
select top 30 percent *
	from ORDERS
	order by AMOUNT

--11


select EMPL_NUM, NAME, HIRE_DATE
	from SALESREPS s1
	where (select count(*)
			from SALESREPS s2
			where s2.EMPL_NUM <= s1.EMPL_NUM
	)between 4 and 7
	order by s1.HIRE_DATE 


--12
select distinct o1.PRODUCT										--distinct!!!
 from ORDERS o1

--13
select CUST, count(*) [Кол-во заказов]							--!!!
	from ORDERS
	group by CUST;

--14
select CUST, count(*) [Кол-во заказов], sum(AMOUNT)[Итого]		--!!!
	from ORDERS
	group by CUST;

--15
select REP, avg(AMOUNT)											--!!!
	from ORDERS
	group by REP;

--16
select MFR_ID, max(PRICE)
	from PRODUCTS
	group by MFR_ID;

--17
select c.COMPANY, o.PRODUCT, o.MFR, o.QTY, o.AMOUNT
	from ORDERS o join CUSTOMERS c
	on o.CUST = c.CUST_NUM

--18
select CUST						
	from ORDERS
	group by CUST
	having count(*) > 3;										--!!!

--19
select c.CUST_NUM, c.COMPANY
	from ORDERS o join CUSTOMERS c
	on o.CUST = c.CUST_NUM
	where o.ORDER_NUM is null;

--20
select c.CUST_NUM, c.COMPANY
	from ORDERS o join CUSTOMERS c
	on o.CUST = c.CUST_NUM
	where not o.ORDER_DATE between '2007-01-01' and '2007-01-01'

--21
select o.PRODUCT
	from ORDERS o join CUSTOMERS c
	on o.CUST = c.CUST_NUM
	where c.CREDIT_LIMIT > 40000;

--22
select *
	from SALESREPS s1
	where AGE in (select s2.AGE
					from SALESREPS s2
					where not s1.EMPL_NUM = s2.EMPL_NUM)

	--или 
select AGE, NAME
from SALESREPS
  group by AGE
  having count(*) > 1

--23
select CUST, sum(AMOUNT)[Итого]
	from ORDERS
	group by CUST
	order by [Итого] desc;

--24
SELECT *
FROM ORDERS
WHERE AMOUNT > (SELECT AVG(AMOUNT) FROM ORDERS);

--25

SELECT o.OFFICE, SUM(ord.AMOUNT) AS Total_Sales
	FROM OFFICES o JOIN ORDERS ord
	ON o.MGR = ord.REP
	GROUP BY o.OFFICE
	ORDER BY Total_Sales DESC;

--26
select EMPL_NUM
	from SALESREPS
	where EMPL_NUM in (select MANAGER from SALESREPS)

--27
select EMPL_NUM
	from SALESREPS
	where EMPL_NUM not in (select MANAGER from SALESREPS WHERE MANAGER IS NOT NULL)

--28
select ord.PRODUCT
	from OFFICES ofi join ORDERS ord
	on ofi.MGR = ord.REP
	where ofi.REGION = 'Western'

--29
SELECT DISTINCT p.PRODUCT_ID, p.DESCRIPTION, p.PRICE
FROM PRODUCTS p
WHERE p.PRICE < ANY (
    SELECT AVG(o.AMOUNT)
    FROM ORDERS o
    GROUP BY o.CUST
);

