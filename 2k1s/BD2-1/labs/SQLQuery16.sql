--1
select 
    id_заказа as [@id_заказа],
    название_товара as [товар/название],
    количество as [товар/количество],
    дата_продажи as [дата_продажи],
    цена_продажи as [цена_продажи]
		from orders
			where id_клиента = 3
for xml path('заказ'), root('заказы');

--2
select 
    clients.id_клиента,
    clients.фамилия,
    clients.имя,
    clients.отчество,
    clients.адрес,
    orders.id_заказа,
    orders.название_товара,
    orders.количество,
    orders.дата_продажи,
    orders.цена_продажи
from clients
inner join orders 
    on clients.id_клиента = orders.id_клиента
for xml auto, root('клиенты_с_заказами');

--3
declare @xmlDoc xml = '
<products>
  <product>
    <название_товара>Тюльпан</название_товара>
    <количество_на_складе>150</количество_на_складе>
    <цена>5.00</цена>
    <единица_измерения>шт.</единица_измерения>
  </product>
  <product>
    <название_товара>Роза</название_товара>
    <количество_на_складе>100</количество_на_складе>
    <цена>8.50</цена>
    <единица_измерения>шт.</единица_измерения>
  </product>
  <product>
    <название_товара>Лилия</название_товара>
    <количество_на_складе>200</количество_на_складе>
    <цена>6.75</цена>
    <единица_измерения>шт.</единица_измерения>
  </product>
</products>';

declare @handle int;
exec sp_xml_preparedocument @handle output, @xmlDoc;
insert into products (название_товара, количество_на_складе, цена, единица_измерения)
	select 
		название_товара,
		количество_на_складе,
		цена,
		единица_измерения
			from openxml(@handle, '/products/product', 2)
				with (
					название_товара nvarchar(50) 'название_товара',
					количество_на_складе int 'количество_на_складе',
					цена real 'цена',
					единица_измерения nvarchar(10) 'единица_измерения'
				);

exec sp_xml_removedocument @handle;

--4


--5
create xml schema collection StudentInfoSchema as
'<?xml version="1.0" encoding="utf-8"?>
<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xs:element name="Student">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="Name" type="xs:string"/>
        <xs:element name="Age" type="xs:int"/>
        <xs:element name="Course" type="xs:string"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>';

create table STUDENT (
    ID int primary key,
    Name nvarchar(100),
    INFO xml(StudentInfoSchema)
);
insert into STUDENT (ID, Name, INFO)
values (1, 'Иван Иванов', 
        '<Student><Name>Иван Иванов</Name><Age>22</Age><Course>Mathematics</Course></Student>');

update STUDENT
set INFO = '<Student><Name>Иван Иванов</Name><Age>23</Age><Course>Physics</Course></Student>'
where ID = 1;

drop table STUDENT;