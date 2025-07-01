--1 
select * from v$sga;

--2
select * from v$parameter;

--3
select * from v$controlfile;
    
--4
create pfile from MEMORY;
create pfile = '/tmp/init/ora' from spfile;

--5
create table test_table(
    id int primary key,
    name varchar2(10)
)

select segment_name from user_segments;

INSERT INTO test_table(id, name) VALUES (1, 'irina');
INSERT INTO test_table(id, name) VALUES (2, 'masha');

select * FROM user_tab_columns where table_name = 'TEST_TABLE';
select * from user_segments where segment_name = 'TEST_TABLE';
 
--6
SELECT
    p.spid AS OS_PROCESS_ID,
    p.pid AS ORACLE_PROCESS_ID,
    p.pname AS PROCESS_NAME,
    s.sid,
    s.serial#,
    s.username,
    s.status,
    s.program,
    s.type,
    s.logon_time,
    s.machine,
    s.osuser,
    s.process,
    s.terminal,
    s.module,
    s.action,
    s.client_info,
    s.server
FROM
    v$process p
LEFT JOIN
    v$session s ON p.addr = s.paddr
ORDER BY
    p.pname, s.status;

--7
SELECT * FROM dba_data_files;

--8
SELECT *
FROM dba_sys_privs 
where Grantee = 'C##RL_UVRCORE';

--9
select * from dba_users;

--10
create role UVR;

--11
create user UVR identified by password;

--12
select * from dba_profiles;

--13
select * from dba_profiles where profile = 'DEFAULT';

--14
create profile UVR limit 
    failed_login_attempts 3;

--15
create SEQUENCE s1
    start with 1000
    minvalue 0
    maxvalue 10000
    cycle
    cache 30
    order;
    
create table t1(
    ID int primary key,
    NAME varchar(10)
);
   
begin
    for i in 1..10 loop
        insert into t1 (ID, NAME) values (s1.Nextval, 'Some Name');
    end loop;
end;  

select * from t1;

drop table t1;
drop SEQUENCE s1;

--16
create synonym pr_syn for t1;
create public synonym pub_syn for t1;

select * from user_synonyms where synonym_name = 'PR_SYN';
select * from all_synonyms where synonym_name = 'PUB_SYN';


--17
DECLARE
  v_data NUMBER;
BEGIN
  SELECT ID INTO v_data FROM t1 WHERE id = 9999; 
EXCEPTION
  WHEN NO_DATA_FOUND THEN
    DBMS_OUTPUT.PUT_LINE('Ошибка: Данные не найдены');
  WHEN TOO_MANY_ROWS THEN
    DBMS_OUTPUT.PUT_LINE('Ошибка: Слишком много строк');
END;

--18
select * from v$logfile;

--19
select * from v$log
    where status = 'CURRENT';
    
--20
select * from v$controlfile;

--21
create table t2( ID int primary key, name varchar(10));

begin 
    for i in 1..100 loop
        insert into t2 (id, name) values (i, 'name');
    end loop;
end;

select * from t2;

select * from user_tables where TABLE_NAME = 'T2';


--22
select * from dba_segments where TABLESPACE_NAME = 'SYSTEM';

--23
select * from user_objects;

--24
select TABLE_NAME, BLOCKS from user_tables where TABLE_NAME = 'T2';

--25
select * from v$session;

--26
select archiver from v$instance;

--27
create VIEW V1 as 
    select id, name from t2 where id < 5;

select * from V1;

--28
CREATE DATABASE LINK remote_db 
CONNECT TO remote_user IDENTIFIED BY password 
USING 'remote_service';

--29
BEGIN
  BEGIN
    RAISE_APPLICATION_ERROR(-20001, 'Искусственно вызванное исключение');
  EXCEPTION
    WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE('Внутренний блок: поймано исключение, но пробрасываем его дальше');
      RAISE;
  END;
EXCEPTION
  WHEN OTHERS THEN
    DBMS_OUTPUT.PUT_LINE('Внешний блок: поймано исключение: ' || SQLERRM);
END;


