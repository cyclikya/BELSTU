-- Заполнение таблицы Grades тестовыми данными
INSERT INTO Grades (grade_id, student_id, subject_id, date, grade)
SELECT
    generate_series(1, 100000) AS grade_id,
    (random()*1000)::int + 1 AS student_id,
    (random()*20)::int + 1 AS subject_id,
    CURRENT_DATE - (random()*100)::int AS date,
    (random()*10)::int + 1 AS grade;
-- Выполнение запроса на выборку оценок 10 без индекса
EXPLAIN ANALYZE
SELECT * FROM Grades WHERE grade = 10;


------------------------------------------

-- Сортировка и фильтрация данных (например, по предметам, семестрам, оценкам).
SELECT * FROM view_student_grades WHERE subject_name='Высшая математика' ORDER BY date DESC;

-- Студент:
-- Просмотр своей успеваемости (таблица предметов и оценок по фамилии).
SELECT * FROM fn_get_student_grades(1);
-- Анализ успеваемости: средний балл по всем предметам
SELECT fn_get_student_avg(1) AS avg_grade;
-- Анализ успеваемости: средний балл по семестрам
SELECT * FROM fn_get_student_avg_by_sem(1);

-- Преподаватель:
-- Просмотр студентов, сгруппированных по факультетам и группам.
SELECT * FROM fn_get_teacher_students(2);
SELECT * FROM view_teacher_students WHERE teacher_id=1;
-- Добавление оценок (только по своим предметам).
begin;
CALL proc_add_grade(1,1,1,8,CURRENT_DATE);
CALL proc_update_grade(69,10);
SELECT * FROM grades;
rollback;
-- Анализ среднего балла по группам по преподавателю и предмету
SELECT * FROM fn_get_avg_by_group(5,5);
-- Отчеты по посещаемости по предмету (в %).
SELECT fn_get_attendance_pct(3) AS pct_present;

-- Администратор:
-- Управление студентами и преподавателями.
begin;
CALL proc_add_student(31,'RB0012',1);
CALL proc_update_student(31,2); --перевод в др группу
CALL proc_delete_student(31);
SELECT * from students;
CALL proc_add_teacher(32,'кандидаь наук','доцент','высшее');
CALL proc_update_teacher(11,'SenLecturer');
CALL proc_delete_teacher(11);
SELECT * from teachers;
rollback;
-- Назначение ролей пользователям.
CALL proc_grant_role('student_user','teacher_role');
CALL proc_revoke_role('student_user','teacher_role');
-- Отчеты по успеваемости (процент успеваемости по предметам и группам).
SELECT * FROM view_admin_performance;
-- Выявление студентов в зоне риска (средний балл < 6).
SELECT * FROM view_students_risk;

-- Отдел статистики:
-- Сводные отчеты по успеваемости группы.
SELECT * FROM view_stats_summary;
SELECT * FROM fn_group_summary(1);
-- Анализ динамики успеваемости/Hабота с данными за разные годы.
SELECT * FROM fn_performance_dynamics(2024,2025);

-- Импорт и экспорт JSON-данных:
BEGIN;
CALL proc_import_students('C:/unv/students.json');
CALL proc_export_students('C:/unv/students.json');
SELECT * FROM students;
ROLLBACK;
-------------------------------------------------------------