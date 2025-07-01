--1
select 
    id_������ as [@id_������],
    ��������_������ as [�����/��������],
    ���������� as [�����/����������],
    ����_������� as [����_�������],
    ����_������� as [����_�������]
		from orders
			where id_������� = 3
for xml path('�����'), root('������');

--2
select 
    clients.id_�������,
    clients.�������,
    clients.���,
    clients.��������,
    clients.�����,
    orders.id_������,
    orders.��������_������,
    orders.����������,
    orders.����_�������,
    orders.����_�������
from clients
inner join orders 
    on clients.id_������� = orders.id_�������
for xml auto, root('�������_�_��������');

--3
declare @xmlDoc xml = '
<products>
  <product>
    <��������_������>�������</��������_������>
    <����������_��_������>150</����������_��_������>
    <����>5.00</����>
    <�������_���������>��.</�������_���������>
  </product>
  <product>
    <��������_������>����</��������_������>
    <����������_��_������>100</����������_��_������>
    <����>8.50</����>
    <�������_���������>��.</�������_���������>
  </product>
  <product>
    <��������_������>�����</��������_������>
    <����������_��_������>200</����������_��_������>
    <����>6.75</����>
    <�������_���������>��.</�������_���������>
  </product>
</products>';

declare @handle int;
exec sp_xml_preparedocument @handle output, @xmlDoc;
insert into products (��������_������, ����������_��_������, ����, �������_���������)
	select 
		��������_������,
		����������_��_������,
		����,
		�������_���������
			from openxml(@handle, '/products/product', 2)
				with (
					��������_������ nvarchar(50) '��������_������',
					����������_��_������ int '����������_��_������',
					���� real '����',
					�������_��������� nvarchar(10) '�������_���������'
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
values (1, '���� ������', 
        '<Student><Name>���� ������</Name><Age>22</Age><Course>Mathematics</Course></Student>');

update STUDENT
set INFO = '<Student><Name>���� ������</Name><Age>23</Age><Course>Physics</Course></Student>'
where ID = 1;

drop table STUDENT;