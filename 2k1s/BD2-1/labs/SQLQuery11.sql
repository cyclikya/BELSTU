create table #T (
    firstname nvarchar(50),
    lastname nvarchar(50)
);
insert into #T (firstname, lastname)
values 
    ('����', '������'),
    ('����', '������'),
    ('������', '�������'),
    ('�������', '��������'),
    ('�������', '��������'),
    ('����', '��������'),
    ('�����', '���������'),
    ('�����', '���������'),
    ('���������', '������'),
    ('�������', '�������');
select * from #T;
drop table #T;



--1
declare @tv char(20),@t char(300) = '';
declare x cursor
	for select ������� from CLIENTS;
open x;
	fetch x into @tv;
	print '�������';
	while @@fetch_status = 0
		begin 
			set @t = rtrim(@tv) + ', ' + @t;
			fetch x into @tv;
		end;
	print @t;
close x;
deallocate x;

--2
declare @tv char(20),@t char(300) = '';
declare y cursor local
	for select ������� from CLIENTS;
open y;
	fetch y into @tv;
	print '�������';
	while @@fetch_status = 0
		begin 
			set @t = rtrim(@tv) + ', ' + @t;
			fetch y into @tv;
		end;
	print @t;
close y;

declare @tv char(20),@t char(300) = '';
declare y cursor global
	for select ������� from CLIENTS;
open y;
	fetch y into @tv;
	print '�������';
	while @@fetch_status = 0
		begin 
			set @t = rtrim(@tv) + ', ' + @t;
			fetch y into @tv;
		end;
	print @t;
close y;
deallocate y;

--3
declare @firstname nvarchar(50), @lastname nvarchar(50);  
declare z cursor local dynamic                              
    for select firstname, lastname 
        from #T;
open z;
	print '���������� �����: ' + cast(@@CURSOR_ROWS as varchar(5));
	update #T set firstname = '���������' where lastname = '���������';
	delete #T where lastname = '������';
	insert #T (firstname, lastname) 
		values ('��������', '�������');
	fetch z into @firstname, @lastname;     
	while @@fetch_status = 0                                    
		begin 
			print @firstname + ' ' + @lastname;      
			fetch z into @firstname, @lastname; 
		end;          
close z; 

--4
declare @tc int, @rn char(50);
declare l cursor local dynamic scroll
  for select ROW_NUMBER() over (order by ��������_������),
  ��������_������ from ORDERS;
open l;
	fetch l into @tc, @rn;
	print '��������� ������: ' + cast(@tc as varchar(3)) + '.' + rtrim(@rn);
	fetch last from l into @tc, @rn;
	print '��������� ������: ' + cast(@tc as varchar(3)) + '.' + rtrim(@rn);
	fetch absolute 7 from l into @tc, @rn;
	print '7 ������ � ������: ' + cast(@tc as varchar(3)) + '.' + rtrim(@rn);
	fetch prior from l into @tc, @rn;
	print '���������� ������(6): ' + cast(@tc as varchar(3)) + '.' + rtrim(@rn);
	fetch relative 4 from l into @tc, @rn;
	print '4 ������ ������(10): ' + cast(@tc as varchar(3)) + '.' + rtrim(@rn);
	fetch relative -8 from l into @tc, @rn;
	print '8 ������ �����(2): ' + cast(@tc as varchar(3)) + '.' + rtrim(@rn);
close l;

--5
declare @f char(20), @l char(50);
declare f cursor local dynamic
  for select firstname, lastname from #T
  for update;
open f;
	fetch f into @f, @l;
	delete #T where current of f;
	fetch f into @f, @l;
	update #T set firstname = '����' where current of f;
close f;

--6-1
declare @f char(20), @l char(50);
declare f cursor local dynamic
  for select firstname, lastname from #T
  where lastname like '�%'
  for update;
open f;
	fetch f into @f, @l 
	while @@fetch_status = 0
		begin 
			delete #T where current of f;
			fetch f into @f, @l 
		end;
close f;

--6-2
declare @f char(20), @l char(50);
declare f cursor local dynamic
  for select firstname, lastname from #T
  where lastname = '��������'
  for update;
open f;
	fetch f into @f, @l 
	update #T set firstname = '����' where current of f;
close f;


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

declare @n float;               
declare @new_i_2 int;           
declare @new_i_1 int;           
declare @new_i_8_3 float;       

declare cur cursor local 
	for select n from #Table;

open cur;

	fetch cur into @n;

	while @@fetch_status = 0
	begin
		if @i_2 < 2 
			set @new_i_2 = @i_2 * tan(@n - 4) - exp(1 + @b)
		else 
			set @new_i_2 = sqrt(@i_2 * @b - power(@b, 2) * @a) - abs(@n);

		if @i_1 < 2
			set @new_i_1 = @i_1 * tan(@n - 4) - exp(1 + @b);
		else
			set @new_i_1 = sqrt(@i_1 * @b - power(@b, 2) * @a) - abs(@n);

		 if @i_8_3 < 2
			set @new_i_8_3 = @i_8_3 * tan(@n - 4) - exp(1 + @b);
		else
			set @new_i_8_3 = sqrt(@i_8_3 * @b - power(@b, 2) * @a) - abs(@n);

		update #Table
		set i_2 = @new_i_2,
			i_1 = @new_i_1,
			i_8_3 = @new_i_8_3
		where n = @n;
		fetch cur into @n;
	end;

close cur;

select * from #Table;


drop table #Table;