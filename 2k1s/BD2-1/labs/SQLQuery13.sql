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
select cast(@x as varchar(3)) as [���-�� �����]

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
	select p.��������_������, o.����������, p.����, o.����_�������
		from orders o inner join products p 
			on o.��������_������ = p.��������_������
		where (@p is null or p.��������_������ = @p);

    select @c = count(*) from #tempresult;

    select * from #tempresult;
    drop table #tempresult;

	return (select count(*) from products);
end;

declare @productcount int;
exec P1 @p = '�����', @c = @productcount output;
print '���������� ����� � �������������� ������: ' + cast(@productcount as varchar(10));

--3
alter procedure P1
    @p varchar(20) = null
as
begin
    select p.��������_������ as productname, 
           o.���������� as quantity, 
           p.���� as price, 
           o.����_������� as totalprice
    from orders o
    inner join products p on o.��������_������ = p.��������_������
    where (@p is null or p.��������_������ = @p);
end;

create table #Result (
    productname varchar(100),
    quantity int,
    price float,
    totalprice float
);

insert into #Result (productname, quantity, price, totalprice)
	execute P1 @p = '�����';

select * from #Result;

drop table #Result;

--4
create table #TEMP_PRODUCT (
    ��������_������ VARCHAR(100),
    ���������� INT,
    ���� FLOAT
);

alter procedure PRODUCT_INSERT
    @product_name VARCHAR(100), 
    @quantity INT,              
    @price FLOAT                 
as
begin
    begin try

        insert into #TEMP_PRODUCT (��������_������, ����������, ����)
			values (@product_name, @quantity, @price);

        select * from #TEMP_PRODUCT;

        return 1;
    end try
    begin catch        
        print '����� ������: ' + cast(ERROR_NUMBER() as varchar(6));
        print '���������: ' + ERROR_message();
        print '�������: ' + cast(ERROR_severity() as varchar(6));
        print '�����: ' + cast(ERROR_state() as varchar(8));
        print '����� ������: ' + cast(ERROR_line() as varchar(8));
		if error_procedure() is not null
        print '��� ���������: ' + ERROR_procedure();
        return -1;
    end catch;
end;

exec PRODUCT_INSERT @product_name = '�����', @quantity = 100, @price = 150.00;
exec PRODUCT_INSERT @product_name = '�����', @quantity = 100, @price = 150.00;

--5
alter procedure PRODUCT_REPORT
    @max_price float
as
begin
    begin try
        declare @product_list NVARCHAR(MAX);
        select @product_list = string_agg(RTRIM(��������_������), ', ') 
			from PRODUCTS
				where ���� <= @max_price;

        print '������ ������� � ����� ������ ��� ������ ' + cast(@max_price as NVARCHAR) + ':';
        print @product_list;

        declare @product_count INT;
        select @product_count = count(*) 
			from PRODUCTS
				where ���� <= @max_price;

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
        insert into ORDERS (ID_������, ��������_������, ����������, ID_�������, ����_�������, ����_�������)
        values (
            (select isnull(max(ID_������), 0) + 1 from ORDERS), -- ����� ��� ������
            @product_name,
            @quantity,
            1, 
            getdate(),
            @quantity * @price
        );

        return 1;
    end try
    begin catch
        print '��� ������: ' + cast(ERROR_NUMBER() as varchar(10));
        print '���������: ' + ERROR_MESSAGE();
        print '�������: ' + cast(ERROR_SEVERITY() as varchar(10));
        print '�����: ' + cast(ERROR_STATE() as varchar(10));
        print '���������: ' + isnull(ERROR_PROCEDURE(), 'N/A');
        print '����� ������: ' + cast(ERROR_LINE() as varchar(10));
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
			insert into PRODUCTS (��������_������, ����������_��_������, ����, �������_���������)
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
        print '��� ������: ' + cast(ERROR_NUMBER() as varchar(10));
        print '���������: ' + ERROR_MESSAGE();
        print '�������: ' + cast(ERROR_SEVERITY() as varchar(10));
        print '�����: ' + cast(ERROR_STATE() as varchar(10));
        print '���������: ' + isnull(ERROR_PROCEDURE(), 'N/A');
        print '����� ������: ' + cast(ERROR_LINE() as varchar(10));
        return -1;
    end catch;
end;

select * from ORDERS;
select * from PRODUCTS;

exec PRODUCT_INSERTX 
    @product_name = '�����', 
    @quantity = 100, 
    @price = 80.50, 
    @unit = '��', 
    @order_quantity = 50;

delete from PRODUCTS where ��������_������ = '�����';


--���
create procedure SALES
as
begin
    update PRODUCTS
    set ���� = ���� * 0.9;
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
    ��������_������, 
    ���� as �������_����, 
    dbo.FSALES(����) as �����_����
from PRODUCTS;