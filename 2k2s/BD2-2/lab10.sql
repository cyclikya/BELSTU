-- Создание таблицы FACULTY (Факультет)
CREATE TABLE FACULTY (
    FACULTY VARCHAR2(50) NOT NULL,
    FACULTY_NAME VARCHAR2(100) NOT NULL,
    CONSTRAINT PK_FACULTY PRIMARY KEY (FACULTY)
);

-- Создание таблицы PULPIT (Кафедра)
CREATE TABLE PULPIT (
    PULPIT VARCHAR2(50) NOT NULL,
    PULPIT_NAME VARCHAR2(100) NOT NULL,
    FACULTY VARCHAR2(50) NOT NULL,
    CONSTRAINT PK_PULPIT PRIMARY KEY (PULPIT),
    CONSTRAINT FK_PULPIT_FACULTY FOREIGN KEY (FACULTY) 
        REFERENCES FACULTY(FACULTY)
);

-- Создание таблицы TEACHER (Преподаватель)
CREATE TABLE TEACHER (
    TEACHER VARCHAR2(50) NOT NULL,
    TEACHER_NAME VARCHAR2(100) NOT NULL,
    PULPIT VARCHAR2(50) NOT NULL,
    CONSTRAINT PK_TEACHER PRIMARY KEY (TEACHER),
    CONSTRAINT FK_TEACHER_PULPIT FOREIGN KEY (PULPIT) 
        REFERENCES PULPIT(PULPIT)
);

-- Создание таблицы AUDITORIUM_TYPE (Тип аудитории)
CREATE TABLE AUDITORIUM_TYPE (
    AUDITORIUM_TYPE VARCHAR2(50) NOT NULL,
    AUDITORIUM_TYPENAME VARCHAR2(100) NOT NULL,
    CONSTRAINT PK_AUDITORIUM_TYPE PRIMARY KEY (AUDITORIUM_TYPE)
);

-- Создание таблицы AUDITORIUM (Аудитория)
CREATE TABLE AUDITORIUM (
    AUDITORIUM VARCHAR2(50) NOT NULL,
    AUDITORIUM_NAME VARCHAR2(100) NOT NULL,
    AUDITORIUM_CAPACITY NUMBER NOT NULL,
    AUDITORIUM_TYPE VARCHAR2(50) NOT NULL,
    CONSTRAINT PK_AUDITORIUM PRIMARY KEY (AUDITORIUM),
    CONSTRAINT FK_AUDITORIUM_TYPE FOREIGN KEY (AUDITORIUM_TYPE) 
        REFERENCES AUDITORIUM_TYPE(AUDITORIUM_TYPE)
);

-- Создание таблицы SUBJECT (Дисциплина)
CREATE TABLE SUBJECT (
    SUBJECT VARCHAR2(50) NOT NULL,
    SUBJECT_NAME VARCHAR2(100) NOT NULL,
    PULPIT VARCHAR2(50) NOT NULL,
    CONSTRAINT PK_SUBJECT PRIMARY KEY (SUBJECT),
    CONSTRAINT FK_SUBJECT_PULPIT FOREIGN KEY (PULPIT) 
        REFERENCES PULPIT(PULPIT)
);

-- Заполнение таблицы FACULTY
INSERT INTO FACULTY (FACULTY, FACULTY_NAME) VALUES ('ФКН', 'Факультет компьютерных наук');
INSERT INTO FACULTY (FACULTY, FACULTY_NAME) VALUES ('ФЭН', 'Факультет экономики');
INSERT INTO FACULTY (FACULTY, FACULTY_NAME) VALUES ('ФИЯ', 'Факультет иностранных языков');

-- Заполнение таблицы PULPIT
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES ('ИСиТ', 'Информационные системы и технологии', 'ФКН');
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES ('ПОИТ', 'Программное обеспечение информационных технологий', 'ФКН');
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES ('ЭК', 'Экономика', 'ФЭН');
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES ('БИ', 'Бухгалтерский учет', 'ФЭН');
INSERT INTO PULPIT (PULPIT, PULPIT_NAME, FACULTY) VALUES ('ЛН', 'Лингвистика', 'ФИЯ');

-- Заполнение таблицы TEACHER
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES ('Иванов', 'Иван Иванович Иванов', 'ИСиТ');
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES ('Петров', 'Петр Петрович Петров', 'ПОИТ');
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES ('Сидоров', 'Сидор Сидорович Сидоров', 'ЭК');
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES ('Кузнецова', 'Анна Васильевна Кузнецова', 'БИ');
INSERT INTO TEACHER (TEACHER, TEACHER_NAME, PULPIT) VALUES ('Смирнова', 'Елена Александровна Смирнова', 'ЛН');

-- Заполнение таблицы AUDITORIUM_TYPE
INSERT INTO AUDITORIUM_TYPE (AUDITORIUM_TYPE, AUDITORIUM_TYPENAME) VALUES ('ЛК', 'Лекционная');
INSERT INTO AUDITORIUM_TYPE (AUDITORIUM_TYPE, AUDITORIUM_TYPENAME) VALUES ('ЛБ', 'Лаборатория');
INSERT INTO AUDITORIUM_TYPE (AUDITORIUM_TYPE, AUDITORIUM_TYPENAME) VALUES ('СР', 'Семинарная');

-- Заполнение таблицы AUDITORIUM
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE) VALUES ('101-1', 'Аудитория 101 корпус 1', 50, 'ЛК');
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE) VALUES ('102-1', 'Аудитория 102 корпус 1', 30, 'СР');
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE) VALUES ('201-1', 'Аудитория 201 корпус 1', 25, 'ЛБ');
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE) VALUES ('301-2', 'Аудитория 301 корпус 2', 70, 'ЛК');
INSERT INTO AUDITORIUM (AUDITORIUM, AUDITORIUM_NAME, AUDITORIUM_CAPACITY, AUDITORIUM_TYPE) VALUES ('202-2', 'Аудитория 202 корпус 2', 20, 'ЛБ');

-- Заполнение таблицы SUBJECT
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES ('БД', 'Базы данных', 'ИСиТ');
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES ('ПО', 'Программное обеспечение', 'ПОИТ');
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES ('ЭТ', 'Экономическая теория', 'ЭК');
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES ('БУ', 'Бухгалтерский учет', 'БИ');
INSERT INTO SUBJECT (SUBJECT, SUBJECT_NAME, PULPIT) VALUES ('ЛГ', 'Лингвистика', 'ЛН');

COMMIT;



 --drop table faculty; drop table pulpit; drop table teacher; drop table auditorium_type; drop table auditorium; drop table subject;
 
 SET SERVEROUTPUT ON;
--1
BEGIN
  NULL;
END;
/

--2
BEGIN
  DBMS_OUTPUT.PUT_LINE('Hello World!');
END;
/

--3
SELECT * FROM V$RESERVED_WORDS WHERE RESERVED = 'Y' AND LENGTH(KEYWORD) = 1;

--4
SELECT * FROM V$RESERVED_WORDS WHERE RESERVED = 'Y';

--5
DECLARE
  v_int1 NUMBER := 15; -- Целое число
  v_int2 NUMBER := 4; -- Целое число
  v_math_result NUMBER;
  v_fixed1 NUMBER(6,2) := 456.78; -- Фиксированная точка
  v_fixed2 NUMBER(6,-2) := 98765.43; -- Отрицательный масштаб (округление до сотен)
  v_sci NUMBER := 2.3E3; -- Экспоненциальная запись (2300)
  v_date1 DATE := SYSDATE; -- Текущая дата
  v_date2 DATE := TO_DATE('2025-10-01', 'YYYY-MM-DD'); -- Конкретная дата
  v_char CHAR(10) := 'XYZ'; -- Символьный тип (фикс. длина)
  v_varch VARCHAR2(100) := 'Динамический текст'; -- Символьный тип (перемен. длина)
  v_bool1 BOOLEAN := TRUE; -- Логическое значение
  v_bool2 BOOLEAN := FALSE; -- Логическое значение 
  v_bool3 BOOLEAN := NULL; -- Трехзначная логика
BEGIN
  -- Арифметические операции
  v_math_result := v_int1 + v_int2;
  DBMS_OUTPUT.PUT_LINE('15 + 4 = ' || v_math_result);
  
  v_math_result := v_int1 - v_int2;
  DBMS_OUTPUT.PUT_LINE('15 - 4 = ' || v_math_result);
  
  v_math_result := v_int1 * v_int2;
  DBMS_OUTPUT.PUT_LINE('15 × 4 = ' || v_math_result);
  
  v_math_result := v_int1 / v_int2;
  DBMS_OUTPUT.PUT_LINE('15 / 4 = ' || ROUND(v_math_result,2));
  
  v_math_result := MOD(v_int1, v_int2);
  DBMS_OUTPUT.PUT_LINE('15 % 4 = ' || v_math_result);

  -- Вывод специальных числовых форматов
  DBMS_OUTPUT.PUT_LINE('Фикс.точка: ' || v_fixed1);
  DBMS_OUTPUT.PUT_LINE('Округление: ' || v_fixed2); -- Выведет 98800
  DBMS_OUTPUT.PUT_LINE('Экспонента: ' || v_sci);

  -- Работа с датами
  DBMS_OUTPUT.PUT_LINE('Сегодня: ' || TO_CHAR(v_date1, 'DD.MM.YYYY'));
  DBMS_OUTPUT.PUT_LINE('Событие: ' || TO_CHAR(v_date2, 'DD Month YYYY'));

  -- Символьные типы
  DBMS_OUTPUT.PUT_LINE('CHAR: [' || v_char || ']'); -- Заполнение пробелами
  DBMS_OUTPUT.PUT_LINE('VARCHAR2: ' || v_varch);

  -- Логические значения
  IF v_bool1 THEN 
    DBMS_OUTPUT.PUT_LINE('Флаг1: АКТИВЕН');
  END IF;
  
  IF NOT v_bool2 THEN
    DBMS_OUTPUT.PUT_LINE('Флаг2: НЕАКТИВЕН');
  END IF;
  
  IF v_bool3 IS NULL THEN
    DBMS_OUTPUT.PUT_LINE('Флаг3: НЕ ОПРЕДЕЛЁН');
  END IF;
END;

--6
DECLARE
  c_city CONSTANT VARCHAR2(30) := 'Москва';
  c_symbol CONSTANT CHAR(1) := 'A';
  c_rate CONSTANT NUMBER(4,2) := 1.23;
BEGIN
  DBMS_OUTPUT.PUT_LINE('Город: ' || c_city);
  DBMS_OUTPUT.PUT_LINE('Символ: ' || c_symbol);
  DBMS_OUTPUT.PUT_LINE('Курс: ' || c_rate);
  DBMS_OUTPUT.PUT_LINE('Новый курс: ' || (c_rate + 0.50));
  DBMS_OUTPUT.PUT_LINE('Размер города: ' || LENGTH(c_city));
END;
/


--7
DECLARE
  -- Объявляем переменную с типом данных, совпадающим с полем EMPLOYEE.EMPLOYEE_ID
  v_teacher_id TEACHER.TEACHER%TYPE;
BEGIN
  BEGIN
    -- Пробуем получить первый идентификатор сотрудника из таблицы EMPLOYEE
    SELECT TEACHER INTO v_teacher_id FROM TEACHER WHERE ROWNUM = 1;
    DBMS_OUTPUT.PUT_LINE('Фамилия преподавателя: ' || v_teacher_id);
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      DBMS_OUTPUT.PUT_LINE('Таблица TEACHER пуста или не существует');
  END;
END;
/


--8
DECLARE
  v_subject_row SUBJECT%ROWTYPE;
BEGIN
  BEGIN
    SELECT * INTO v_subject_row FROM SUBJECT WHERE ROWNUM = 1;
    DBMS_OUTPUT.PUT_LINE('Преподаватель: ' || v_subject_row.SUBJECT || 
                        ', Кафедра: ' || v_subject_row.PULPIT);
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      DBMS_OUTPUT.PUT_LINE('Таблица SUBJECT пуста или не существует');
  END;
END;
/

--9
DECLARE
  v_name VARCHAR2(30) := 'Иванов';
  v_found NUMBER;
BEGIN
  SELECT COUNT(*) INTO v_found FROM TEACHER WHERE TEACHER = v_name;

  IF v_found > 0 THEN
    DBMS_OUTPUT.PUT_LINE('Запись с фамилией "' || v_name || '" найдена.');
  ELSE
    DBMS_OUTPUT.PUT_LINE('Запись с фамилией "' || v_name || '" отсутствует.');
  END IF;
END;
/

--10
DECLARE
  v_aud_type AUDITORIUM.AUDITORIUM_TYPE%TYPE := 'ЛК';
  v_desc VARCHAR2(100);
BEGIN
  -- Пример использования CASE для определения описания типа аудитории
  CASE v_aud_type
    WHEN 'ЛК' THEN
      v_desc := 'Лекционная аудитория';
    WHEN 'ЛБ' THEN
      v_desc := 'Лаборатория';
    WHEN 'СР' THEN
      v_desc := 'Семинарная аудитория';
    ELSE
      v_desc := 'Неизвестный тип аудитории';
  END CASE;

  DBMS_OUTPUT.PUT_LINE('Тип аудитории ' || v_aud_type || ': ' || v_desc);
END;
/

--11
DECLARE
  v_counter NUMBER := 1;
BEGIN
  DBMS_OUTPUT.PUT_LINE('Цикл LOOP: вывод чисел от 1 до 5');
  LOOP
    EXIT WHEN v_counter > 5;
    DBMS_OUTPUT.PUT_LINE('Число: ' || v_counter);
    v_counter := v_counter + 1;
  END LOOP;
END;
/

--12
DECLARE
  v_counter NUMBER := 1;
BEGIN
  DBMS_OUTPUT.PUT_LINE('Цикл WHILE: вывод чисел от 1 до 5');
  WHILE v_counter <= 5 LOOP
    DBMS_OUTPUT.PUT_LINE('Число: ' || v_counter);
    v_counter := v_counter + 1;
  END LOOP;
END;
/


--13
BEGIN
  DBMS_OUTPUT.PUT_LINE('Цикл FOR: вывод чисел от 1 до 5');
  FOR i IN 1..5 LOOP
    DBMS_OUTPUT.PUT_LINE('Число: ' || i);
  END LOOP;
END;
/
