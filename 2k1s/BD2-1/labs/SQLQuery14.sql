--1 скалярная, количество продуктов
alter function COUNT_PRODUCTS(@min_quantity int = NULL)
returns int
as
begin
	declare @count int;
    select @count = count(*)
		from PRODUCTS
			where Количество_на_складе > @min_quantity;
    return @count;
end;

select dbo.COUNT_PRODUCTS(200) as ProductsAbove100;


alter function COUNT_PRODUCTS(@min_quantity int = NULL, @max_price float = NULL)
returns int
as
begin
	declare @count int;
    select @count = count(*)
		from PRODUCTS
			where Количество_на_складе > @min_quantity
				and Цена < @max_price;
    return @count;
end;

select dbo.COUNT_PRODUCTS(200, 40.00) as ProductsAbove100;

--2 возвращает список
alter function FORDERS (@id int)
returns varchar(300)
as
begin
    declare @product_list varchar(300) = '';  

    declare product_cursor cursor for
		select o.Название_товара
			from ORDERS o
				where o.ID_клиента = @id;

    open product_cursor;
		declare @product_name varchar(100);
    
		fetch next from product_cursor into @product_name;
		while @@fetch_status = 0
		begin
			set @product_list = @product_list + @product_name + ', ';
        
			fetch next from product_cursor into @product_name;
		end
    close product_cursor;
    deallocate product_cursor;

    if len(@product_list) > 0
    begin
        set @product_list = left(@product_list, len(@product_list) - 1);
    end

    return @product_list;
end;

select dbo.FORDERS(3) as PurchasedProducts;

--3 табличная функция (выводит)
create table FACULTY (
    faculty_id int primary key, 
    faculty_name varchar(100)
);
create table PULPIT (
    pulpit_id int primary key, 
    faculty_id int, 
    pulpit_name varchar(100), 
    foreign key (faculty_id) references FACULTY(faculty_id)
);
insert into FACULTY (faculty_id, faculty_name)
values
(1, 'Факультет математики'),
(2, 'Факультет физики'),
(3, 'Факультет химии');
insert into PULPIT (pulpit_id, faculty_id, pulpit_name)
values
(1, 1, 'Кафедра высшей математики'),
(2, 1, 'Кафедра прикладной математики'),
(3, 2, 'Кафедра теоретической физики'),
(4, 2, 'Кафедра экспериментальной физики'),
(5, 3, 'Кафедра органической химии');

create function FFACPUL (@faculty_id int = null, @pulpit_id int = null)
returns table
as
return
(
    select f.faculty_name, p.pulpit_name
		from FACULTY f left join PULPIT p on f.faculty_id = p.faculty_id
		where 
			(@faculty_id is null or f.faculty_id = @faculty_id) and
			(@pulpit_id is null or p.pulpit_id = @pulpit_id)
);

drop function FFACPUL;

select * from dbo.FFACPUL(NULL, NULL);
select * from dbo.FFACPUL(1, NULL); 
select * from dbo.FFACPUL(NULL, 2); 
select * from dbo.FFACPUL(1, 2);  

--4 скалярная, кол-во преподов в кафедре
CREATE TABLE TEACHERS (
    teacher_id INT PRIMARY KEY,     
    teacher_name VARCHAR(100),      
    pulpit_id INT,                  
    FOREIGN KEY (pulpit_id) REFERENCES PULPIT(pulpit_id)  
);
INSERT INTO TEACHERS (teacher_id, teacher_name, pulpit_id)
VALUES
    (1, 'Иванов Иван Иванович', 1),  
    (2, 'Петров Петр Петрович', 2), 
    (3, 'Сидоров Сидор Сидорович', 2),  
    (4, 'Кузнецова Ольга Александровна', 3),  
    (5, 'Смирнов Алексей Николаевич', 4),  
    (6, 'Тимофеева Мария Павловна', 1), 
    (7, 'Михайлова Ирина Борисовна', 5);  

create function dbo.FCTEACHER (@pulpit_id int)
returns int
as
begin
    declare @result int;
    
    if @pulpit_id is null
    begin
        select @result = count(*) from teachers;
    end
    else
    begin
        select @result = count(*) from teachers where pulpit_id = @pulpit_id;
    end
    
    return @result;
end;

select dbo.FCTEACHER(NULL) AS TotalTeachers;

select pulpit_name, dbo.FCTEACHER(pulpit_id) AS TeachersOnPulpit from PULPIT

--6
-- Скалярная функция для подсчёта количества кафедр
create function count_pulpits(@faculty varchar(30))
returns int
as
begin
    return (select count(pulpit) from pulpit where faculty = @faculty);
end;

-- Скалярная функция для подсчёта количества групп
create function count_groups(@faculty varchar(30))
returns int
as
begin
    return (select count(idgroup) from groups where faculty = @faculty);
end;

-- Скалярная функция для подсчёта количества студентов
create function count_students(@faculty varchar(30))
returns int
as
begin
    return (select dbo.count_students(@faculty, default));
end;

-- Скалярная функция для подсчёта количества специальностей
create function count_professions(@faculty varchar(30))
returns int
as
begin
    return (select count(profession) from profession where faculty = @faculty);
end;

-- Изменённая многооператорная табличная функция faculty_report
create function faculty_report(@c int) 
returns @fr table
(
    [Факультет] varchar(50), 
    [Количество кафедр] int, 
    [Количество групп] int, 
    [Количество студентов] int, 
    [Количество специальностей] int
)
as
begin
    declare cc cursor static for 
        select faculty from faculty 
        where dbo.count_students(faculty, default) > @c; 

    declare @f varchar(30);
    open cc;  
    fetch cc into @f;

    while @@fetch_status = 0
    begin
        insert @fr values
        (
            @f,  
            dbo.count_pulpits(@f),
            dbo.count_groups(@f),
            dbo.count_students(@f),
            dbo.count_professions(@f)
        ); 
        fetch cc into @f;  
    end;   

    return; 
end;
