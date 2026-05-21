-- Разрешить удалённые подключения
sp_configure 'remote access', 1;
RECONFIGURE;

-- Создать пустую БД
CREATE DATABASE CarDealershipReplica;
DROP DATABASE CarDealershipReplica;

-- Создать логин
CREATE LOGIN repl_user
WITH PASSWORD = '1234';

USE CarDealershipReplica;


CREATE USER repl_user FOR LOGIN repl_user;

ALTER ROLE db_owner ADD MEMBER repl_user;


SELECT * FROM Cars;	