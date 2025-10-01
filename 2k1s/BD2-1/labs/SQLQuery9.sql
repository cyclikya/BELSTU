--1
declare @i int = 5,
		@v varchar(20) = 'Огурец',
		@c char,
		@dt datetime,
		@t time,
		@si smallint,
		@ti tinyint,
		@nu numeric(12, 5);
set @c = 'k';
set @dt = getdate();
set @t = cast(getdate() as time);
set @si = 320;
set @ti = 21;
set @nu = 12345.67890;

select @i = 7,
	   @v = 'Тыква';

print 'intValue = ' + cast(@i as varchar);
print 'varcharValue = ' + cast(@v as varchar(20));
print 'timeValue = ' + cast(@t as varchar);

select @i intValue,
	   @v varcharValue,
	   @t timeValue;

--2
declare @y1 float = (select cast(avg(Цена) as float) from PRODUCTS),
		@y2 int
if @y1 < 30
begin
	set @y2 = (select cast(count(*) as int) from PRODUCTS)
	select @y1 'Средняя оценка', @y2 'Количество учитанных оценок'
end 
else if @y1 > 31 and @y1 < 40 print 'Средняя цена от 31 до 40 '
else if @y1 > 41 and @y1 < 50 print 'Средняя цена от 40 до 50'
else print 'Средняя цена больше 50'

---3
print 'Версия SQL: ' + cast(@@version as varchar(300));
print 'Уровень вложенности транзакции: ' + cast(@@trancount as varchar);
print 'Системный идентификатор процесса, назначенный сервером текущему подключению: ' + cast(@@SPID as varchar);
print 'Количество обратботанных строк: ' + cast(@@rowcount as varchar);
print 'Код последней ошибки: ' + cast(@@error as varchar);
print 'Имя сервера: ' + cast(@@servername as varchar);
print 'Проверка результата считывания строк результирующего набора: ' + cast(@@fetch_status as varchar);
print 'Уровень вложенности текущей процедуры: ' + cast(@@nestlevel as varchar);

---4
-----1
declare @tt int = 5,
		@x float = 3.7,
		@z float;
if @tt > @x 
	set @z = power(sin(@tt), 2);
else if @tt = @x
	set @z = 1 - exp(@x - 2);
else set @z = 4 * (@tt + @x);
print 'z = ' + cast(@z as varchar(10));

-----2
declare @surn varchar(50),
		@name varchar(50),
		@f varchar(50);
select @surn = Фамилия,
	   @name = Имя,
	   @f = Отчество
	from CLIENTS
	where ID_Клиента = 4;
select top 1 @surn + ' ' + left(@name, 1) + '. ' + left(@f, 1) + '.' [Сокращенная форма ФИО] from CLIENTS;

-----3
declare @currentD date = getdate();

select Название_товара, 
       Дата_продажи, 
       datediff(day, Дата_продажи, @currentD) as [Дней прошло с момента продажи]
from ORDERS;

----4
declare @product_name nvarchar(50) = 'Морковь';

select Название_товара, Количество, month(Дата_продажи) as Месяц
from ORDERS
where Название_товара = @product_name;
--5
declare @y11 float = (select cast(avg(Цена) as float) from PRODUCTS),
		@y22 int
if @y11 < 30
begin
	set @y22 = (select cast(count(*) as int) from PRODUCTS)
	select @y11 'Средняя оценка', @y22 'Количество учитанных оценок'
end 
else if @y11 > 31 and @y11 < 40 print 'Средняя цена от 31 до 40 '
else if @y11 > 41 and @y11 < 50 print 'Средняя цена от 40 до 50'
else print 'Средняя цена больше 50'

--6
declare @expensiveGood nvarchar(50) = 'Высокая цена';
declare @normGood nvarchar(50) = 'Приемлимая цена';
declare @cheapGood nvarchar(50) = 'Низкая цена';

select *
from (
    select case 
            when Цена between 30.0000 and 45.0000 then @expensiveGood
            when Цена between 20.0000 and 30.0000 then @normGood
            else @cheapGood
        end as [Пояснение оценки], 
        count(*) as [Количество]
    from PRODUCTS
    group by case 
            when Цена between 30.0000 and 45.0000 then @expensiveGood
            when Цена between 20.0000 and 30.0000 then @normGood
            else @cheapGood
        end
) as T;

--7
create table #T (fir int, sec int, th int);
set nocount on; --откл. вывода счетчиков
declare @ii int = 0;
while @ii < 100
begin
	insert into #T (fir, sec, th)
	values 
	(
		floor(100 * rand()),
		floor(100 * rand()),
		floor (50 * rand())
	);
	set @ii = @ii + 1;
end;

select * from #T;
drop table #T;

--8
declare @p int = 1
	print @p + 1
	print @p + 2
	return 
	print @p + 3;

--9
begin try 
	insert into dbo.ORDERS (ID_заказа, Название_товара)
    values (1, 'А');
end try
begin catch
	print 'Код ошибки: ' + cast(error_number() as varchar);
	print 'Сообщение об ошибке: ' + error_message();
	print 'Номер строки с ошибкой: ' + cast(error_line() as varchar);
	print 'Уровень серьезности ошибки: ' + cast(error_severity() as varchar);
	print 'Метка ошибки: ' + cast(error_state() as varchar);
end catch;



--dop
create table #Table (
	n float,
	i_2 int,
	i_1 int,
	i_8_3 float
);

declare @a float = 2 * power(10, -3),
        @b float = 8.5;

insert into #Table (n)
values (1), (1.5), (2), (2.5), (3);

declare @i_2 int = 2;
declare @i_1 int = 1;
declare @i_8_3 float = 8.3;

update #Table
set i_2 = 
    case
        when @i_2 < 2 then @i_2 * tan(n - 4) - exp(1 + @b)
        else sqrt(@i_2 * @b - power(@b, 2) * @a) - abs(n)
    end;

update #Table
set i_1 = 
    case
        when @i_1 < 2 then @i_1 * tan(n - 4) - exp(1 + @b)
        else sqrt(@i_1 * @b - power(@b, 2) * @a) - abs(n)
    end;

update #Table
set i_8_3 = 
    case
        when @i_8_3 < 2 then @i_8_3 * tan(n - 4) - exp(1 + @b)
        else sqrt(@i_8_3 * @b - power(@b, 2) * @a) - abs(n)
    end;

select * from #Table;



