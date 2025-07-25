SELECT USER FROM DUAL;

GRANT CREATE ANY PROFILE TO SYSTEM;
SELECT * FROM USER_SYS_PRIVS;
CONNECT system/Ugorenko AS SYSDBA;


--1
SHOW PARAMETER spfile;

--2
SELECT value FROM v$parameter WHERE name = 'spfile';

--3
CREATE PFILE='UVR_PFILE.ORA' FROM SPFILE;

--4
--В UVR_PFILE.ORA изменяем sga_target = 800M

--5
CREATE SPFILE='UVR_SPFILE.ORA' 
FROM PFILE='UVR_PFILE.ORA';

--6
-- sqlplus / as sysdba    SHUTDOWN IMMEDIATE;
-- sqlplus / as sysdba    STARTUP;

--7
-- sqlplus / as sysdba    ALTER SYSTEM RESET db_block_size SCOPE=SPFILE;

--8
SELECT REGEXP_SUBSTR(name, '[^/]+$', 1, 1) AS controlfile_name
FROM v$controlfile;

--9
ALTER DATABASE BACKUP CONTROLFILE TO TRACE;

ALTER SYSTEM SET CONTROL_FILES = 'C:\ORCL\ORADATA\ORCL\CONTROL01.CTL',
'C:\Orcl\oradata\ORCL\CONTROL01.copy.CTL' SCOPE = SPFILE;

--10
SELECT * FROM V$PASSWORDFILE_INFO;

--12
SELECT * FROM V$DIAG_INFO;

--14
SELECT * FROM v$diag_info WHERE name = 'Diag Trace';