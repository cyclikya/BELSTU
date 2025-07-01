--1
alter procedure P1
as
begin
    declare @k int;
    select @k = count(*) from PRODUCTS;
    return @k;
end;

declare @x int = 0;
exec @x = P1;
select cast(@x as varchar(3)) as [Кол-во строк]

--2
alter procedure P1
    @p varchar(20) = null,
    @c int output           
as
begin
    create table #tempresult (
        productname varchar(100),
        quantity int,
        price float,
        totalprice float
    );

    insert into #tempresult (productname, quantity, price, totalprice)
	select p.Название_товара, o.Количество, p.Цена, o.Цена_продажи
		from orders o inner join products p 
			on o.Название_товара = p.Название_товара
		where (@p is null or p.название_товара = @p);

    select @c = count(*) from #tempresult;

    select * from #tempresult;
    drop table #tempresult;

	return (select count(*) from products);
end;

declare @productcount int;
exec P1 @p = 'Банан', @c = @productcount output;
print 'Количество строк в результирующем наборе: ' + cast(@productcount as varchar(10));

--3
alter procedure P1
    @p varchar(20) = null
as
begin
    select p.Название_товара as productname, 
           o.Количество as quantity, 
           p.Цена as price, 
           o.Цена_продажи as totalprice
    from orders o
    inner join products p on o.Название_товара = p.Название_товара
    where (@p is null or p.Название_товара = @p);
end;

create table #Result (
    productname varchar(100),
    quantity int,
    price float,
    totalprice float
);

insert into #Result (productname, quantity, price, totalprice)
	execute P1 @p = 'Банан';

select * from #Result;

drop table #Result;

--4
create table #TEMP_PRODUCT (
    Название_товара VARCHAR(100),
    Количество INT,
    Цена FLOAT
);

alter procedure PRODUCT_INSERT
    @product_name VARCHAR(100), 
    @quantity INT,              
    @price FLOAT                 
as
begin
    begin try

        insert into #TEMP_PRODUCT (Название_товара, Количество, Цена)
			values (@product_name, @quantity, @price);

        select * from #TEMP_PRODUCT;

        return 1;
    end try
    begin catch        
        print 'номер ошибки: ' + cast(ERROR_NUMBER() as varchar(6));
        print 'сообщение: ' + ERROR_message();
        print 'уровень: ' + cast(ERROR_severity() as varchar(6));
        print 'метка: ' + cast(ERROR_state() as varchar(8));
        print 'номер строки: ' + cast(ERROR_line() as varchar(8));
		if error_procedure() is not null
        print 'имя процедуры: ' + ERROR_procedure();
        return -1;
    end catch;
end;

exec PRODUCT_INSERT @product_name = 'Банан', @quantity = 100, @price = 150.00;
exec PRODUCT_INSERT @product_name = 'Банан', @quantity = 100, @price = 150.00;

--5
alter procedure PRODUCT_REPORT
    @max_price float
as
begin
    begin try
        declare @product_list NVARCHAR(MAX);
        select @product_list = string_agg(RTRIM(Название_товара), ', ') 
			from PRODUCTS
				where Цена <= @max_price;

        print 'Список товаров с ценой меньше или равной ' + cast(@max_price as NVARCHAR) + ':';
        print @product_list;

        declare @product_count INT;
        select @product_count = count(*) 
			from PRODUCTS
				where Цена <= @max_price;

        return @product_count;
    end try
    begin catch
        declare @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        select @ErrorMessage = ERROR_MESSAGE(), 
               @ErrorSeverity = ERROR_SEVERITY(), 
               @ErrorState = ERROR_STATE();
        print 'Error Code: ' + cast(ERROR_NUMBER() as varchar(10)) + 
              ' Severity: ' + cast(@ErrorSeverity as varchar(10)) + 
              ' Message: ' + @ErrorMessage;
        return -1;
    end catch;
end;

--6
alter procedure PRODUCT_INSERT
    @product_name VARCHAR(100),  
    @quantity INT,              
    @price FLOAT                 
as
begin
    begin try
        insert into ORDERS (ID_заказа, Название_товара, Количество, ID_клиента, Дата_продажи, Цена_продажи)
        values (
            (select isnull(max(ID_заказа), 0) + 1 from ORDERS), -- Новый код заказа
            @product_name,
            @quantity,
            1, 
            getdate(),
            @quantity * @price
        );

        return 1;
    end try
    begin catch
        print 'Код ошибки: ' + cast(ERROR_NUMBER() as varchar(10));
        print 'Сообщение: ' + ERROR_MESSAGE();
        print 'Уровень: ' + cast(ERROR_SEVERITY() as varchar(10));
        print 'Метка: ' + cast(ERROR_STATE() as varchar(10));
        print 'Процедура: ' + isnull(ERROR_PROCEDURE(), 'N/A');
        print 'Номер строки: ' + cast(ERROR_LINE() as varchar(10));
        return -1;
    end catch;
end;


alter procedure PRODUCT_INSERTX
    @product_name VARCHAR(100), 
    @quantity INT,              
    @price FLOAT,               
    @unit VARCHAR(20),           
    @order_quantity INT          
as
begin
    set nocount on;
    declare @result INT;
    begin try
        set transaction isolation level serializable;
        begin transaction;
			insert into PRODUCTS (Название_товара, Количество_на_складе, Цена, Единица_измерения)
				values (@product_name, @quantity, @price, @unit);
			exec @result = PRODUCT_INSERT @product_name, @order_quantity, @price;
			if @result = -1
			begin
				rollback transaction;
				return -1;
			end;
        commit transaction;
        return 1;
    end try
    begin catch
        if @@trancount > 0
            rollback transaction;
        print 'Код ошибки: ' + cast(ERROR_NUMBER() as varchar(10));
        print 'Сообщение: ' + ERROR_MESSAGE();
        print 'Уровень: ' + cast(ERROR_SEVERITY() as varchar(10));
        print 'Метка: ' + cast(ERROR_STATE() as varchar(10));
        print 'Процедура: ' + isnull(ERROR_PROCEDURE(), 'N/A');
        print 'Номер строки: ' + cast(ERROR_LINE() as varchar(10));
        return -1;
    end catch;
end;

select * from ORDERS;
select * from PRODUCTS;

exec PRODUCT_INSERTX 
    @product_name = 'Манго', 
    @quantity = 100, 
    @price = 80.50, 
    @unit = 'кг', 
    @order_quantity = 50;

delete from PRODUCTS where Название_товара = 'Манго';


--доп
create procedure SALES
as
begin
    update PRODUCTS
    set цена = цена * 0.9;
end;

select * from PRODUCTS;
exec SALES;
select * from PRODUCTS;



create function FSALES(@current_price real)
returns real
as
begin
    return @current_price * 0.9;
end;

select * from PRODUCTS;
select 
    название_товара, 
    цена as текущая_цена, 
    dbo.FSALES(Цена) as новая_цена
from PRODUCTS;