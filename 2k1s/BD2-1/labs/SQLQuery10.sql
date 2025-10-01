exec sp_helpindex 'ORDERS'
set nocount on;
drop table #T6;
select * from #T6;


--1
create table #T1 (x int, y int);
declare @i int = 0;
while @i < 1000
begin
	insert into #T1 (x, y)
	values 
	(
		@i + 1,
		floor (50 * rand())
	);
	set @i = @i + 1;
end;

select * 
	from #T1
	where y between 10 and 20;

create clustered index #T_CL on #T1(y);

--2
create table #T2 (x int, y int);
declare @i2 int = 0;
while @i2 < 10000
begin
	insert into #T2 (x, y)
	values 
	(
		@i2 + 1,
		floor (1000 * rand())
	);
	set @i2 = @i2 + 1;
end;

select * 
	from #T2
	where y between 10 and 20 and x < 1000;

create index #T2_CL on #T2(x, y);

--3
create table #T3 (x int, y int);
declare @i3 int = 0;
while @i3 < 10000
begin
	insert into #T3 (x, y)
	values 
	(
		@i3 + 1,
		floor (1000 * rand())
	);
	set @i3 = @i3 + 1;
end;

select * 
	from #T3
	where y between 10 and 20 and x < 1000;

create index #T3_CL on #T3(x) include(y);

--4
create table T4 (x int, y int);
declare @i4 int = 0;
while @i4 < 50
begin
	insert into T4 (x, y)
	values 
	(
		@i4 + 1,
		floor (50 * rand())
	);
	set @i4 = @i4 + 1;
end;

select y 
	from T4
	where y between 10 and 20;

select y
	from T4
	where y = 12;

create index T4_CL on T4(y) where (y <= 30 and y >= 5);

--5
create table #T5 (x int, y int);
declare @i5 int = 0;
while @i5 < 50
begin
	insert into #T5(x, y)
	values 
	(
		@i5 + 1,
		floor (30 * rand())
	);
	set @i5 = @i5 + 1;
end;

create index #T5_CL on #T5(x);

select 
    ii.name [Имя индекса], 
    ss.avg_fragmentation_in_percent [Фрагментация (%)]
from sys.dm_db_index_physical_stats(db_id(), object_id('#T5'), null, null, 'DETAILED') ss
  join sys.indexes ii on ss.object_id = ii.object_id and ss.index_id = ii.index_id;

insert top(1000) #T5(x, y) select x, y from #T5;

select 
    ii.name [Имя индекса], 
    ss.avg_fragmentation_in_percent [Фрагментация (%)]
from sys.dm_db_index_physical_stats(db_id(), object_id('#T5'), null, null, 'DETAILED') ss
  join sys.indexes ii on ss.object_id = ii.object_id and ss.index_id = ii.index_id;

  alter index #T5_CL on #T5 reorganize;

  alter index #T5_CL on #T5 rebuild with (online = off);


--6
create table #T6 (x int, y int);
declare @i6 int = 1;
while @i6 <= 10000
begin
	insert into #T6(x, y)
	values 
	(
		@i6 + 1,
		floor(30 * rand())
	);
	set @i6 = @i6 + 1;
end;

create index #T6_CL on #T6(y) with (fillfactor = 70);

insert top(50) percent into #T6(x, y) select x, y from #T6;

select 
    ii.name [Имя индекса], 
    ss.avg_fragmentation_in_percent [Фрагментация (%)]
from sys.dm_db_index_physical_stats(db_id(), object_id('#T6'), null, null, 'DETAILED') ss
  join sys.indexes ii on ss.object_id = ii.object_id and ss.index_id = ii.index_id;
