SELECT name, open_mode, con_id FROM v$pdbs;

ALTER SESSION SET CONTAINER = CDB$ROOT;
ALTER PLUGGABLE DATABASE orclpdb OPEN READ WRITE;

show con_name;
select * from all_users;
-- 2. Создать пользователя

Drop USER Vi;

CREATE USER Vi IDENTIFIED BY 1234
DEFAULT TABLESPACE USERS
QUOTA UNLIMITED ON USERS
TEMPORARY TABLESPACE TEMP;

GRANT CREATE SESSION TO Vi;
GRANT CREATE TABLE TO Vi;
GRANT UNLIMITED TABLESPACE TO Vi;
GRANT CONNECT TO Vi;
GRANT CREATE DATABASE LINK TO Vi;

CREATE DATABASE LINK linkdb
CONNECT TO Hleb IDENTIFIED BY "1234"
USING '(DESCRIPTION=
         (ADDRESS=(PROTOCOL=TCP)(HOST=172.20.10.3)(PORT=1521))
         (CONNECT_DATA=(SERVICE_NAME=ORCLPDB)))';
        
DROP DATABASE LINK linkdb;     
SELECT * FROM DUAL@linkdb;
SELECT * FROM remote_table@linkdb WHERE ROWNUM = 1;
select * from local_table;
select * from remote_table;

DROP TABLE remote_table CASCADE CONSTRAINTS PURGE;
DROP TABLE local_table CASCADE CONSTRAINTS PURGE;

CREATE TABLE remote_table (
    id NUMBER PRIMARY KEY,
    data VARCHAR2(100),
    status VARCHAR2(20)
);
CREATE TABLE local_table (
    id NUMBER PRIMARY KEY,
    data VARCHAR2(100),
    status VARCHAR2(20)
);

--6.1
SET TRANSACTION NAME 'txn_insert_insert';
INSERT INTO local_table VALUES (1, 'Start local', 'NEW');
INSERT INTO remote_table@linkdb VALUES (1, 'Start remote from Vi', 'NEW');
COMMIT;

select * from local_table;
select * from remote_table@linkdb;

--6.2
begin
INSERT INTO local_table VALUES (2, 'Second row', 'PENDING');
UPDATE remote_table@linkdb SET value='Updated by Vi', processed_flag='Y' WHERE id=1;

COMMIT;
end;

--6.3
begin
UPDATE local_table SET status='PROCESSED' WHERE id=2;
INSERT INTO remote_table@linkdb VALUES (2, 'New remote row', 'N');
COMMIT;
end;

--7.1
--DELETE FROM remote_table@linkdb WHERE id=2;
--COMMIT;

begin
-- Этот блок выдаст ошибку (дубликат ключа 100)
--INSERT INTO local_table VALUES (101, 'Will be rolled back', 'ERROR');
INSERT INTO remote_table@linkdb VALUES (2, 'DUPLICATE KEY!', 'X'); -- дубликат по id 2
COMMIT;
end;

--
-- Подключаешься как SYS или SYSTEM
GRANT EXECUTE ON SYS.DBMS_LOCK TO Vi;
SET SERVEROUTPUT ON;

DECLARE
   v_lock_handle VARCHAR2(128);
   v_result NUMBER;
BEGIN
   DBMS_OUTPUT.PUT_LINE('TXN B (удаленный комп) НАЧАЛО');
   DBMS_OUTPUT.PUT_LINE('Время: ' || TO_CHAR(SYSDATE, 'HH24:MI:SS'));
   
   -- Создаем именованную блокировку
   DBMS_LOCK.ALLOCATE_UNIQUE('MY_TEST_LOCK', v_lock_handle);
   
   -- Захватываем эксклюзивную блокировку
   v_result := DBMS_LOCK.REQUEST(
      lockhandle => v_lock_handle,
      lockmode => DBMS_LOCK.X_MODE,
      timeout => 0,
      release_on_commit => TRUE
   );
   
   IF v_result = 0 THEN
      -- Обновляем строку
      UPDATE remote_table
      SET data = 'BLOCKED BY Vi', status = 'Y' 
      WHERE id = 2;
      
      
      DBMS_OUTPUT.PUT_LINE('TXN B: Строка обновлена, блокировка DBMS_LOCK удерживается');
      DBMS_OUTPUT.PUT_LINE('TXN B: Ждем 15 секунд...');
      
      -- Держим блокировку 15 секунд
      DBMS_LOCK.SLEEP(15);
      
      -- Фиксируем изменения
      COMMIT;
      DBMS_OUTPUT.PUT_LINE('TXN B: COMMIT выполнен, блокировка снята');
   ELSE
      DBMS_OUTPUT.PUT_LINE('TXN B: ОШИБКА - не удалось получить блокировку');
   END IF;
   
   DBMS_OUTPUT.PUT_LINE('TXN B КОНЕЦ');
   DBMS_OUTPUT.PUT_LINE('Время: ' || TO_CHAR(SYSDATE, 'HH24:MI:SS'));
END;
/

---
DECLARE
   v_start DATE := SYSDATE;
BEGIN
   DBMS_OUTPUT.PUT_LINE('Время: '  TO_CHAR(SYSDATE, 'HH24:MI:SS'));
   DBMS_OUTPUT.PUT_LINE('TXN A: Пытаемся обновить строку через dblink');
   
   UPDATE remote_table@linkdb 
   SET value = 'UPDATED BY TXN A', processed_flag = 'X' 
   WHERE id = 1;
   
   -- Сюда дойдем только после COMMIT от TXN B
   DBMS_OUTPUT.PUT_LINE('TXN A: Обновление выполнено! Ожидание заняло '  
                        ROUND((SYSDATE - v_start) * 86400)  ' секунд');
   
   -- Обновляем локальную таблицу
   UPDATE local_table SET status = 'UPDATED BY TXN A' WHERE id = 1;
   
   COMMIT;
   DBMS_OUTPUT.PUT_LINE('TXN A: COMMIT выполнен');
   DBMS_OUTPUT.PUT_LINE('Время: '  TO_CHAR(SYSDATE, 'HH24:MI:SS'));
END;
/