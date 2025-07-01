-- Список оценок студента
CREATE OR REPLACE FUNCTION fn_get_student_grades(p_student INT)
RETURNS TABLE(subject VARCHAR, grade INT, date DATE) LANGUAGE sql AS $$
  SELECT sub.subject_name, g.grade, g.date
    FROM Grades g
    JOIN Subjects sub ON g.subject_id=sub.subject_id
   WHERE g.student_id = p_student
   ORDER BY g.date;
$$;
SELECT * FROM fn_get_student_grades(1);

-- Средняя оценка студента
CREATE OR REPLACE FUNCTION fn_get_student_avg(p_student INT)
RETURNS NUMERIC LANGUAGE sql AS $$
  SELECT AVG(grade)::NUMERIC FROM Grades WHERE student_id = p_student;
$$;
SELECT fn_get_student_avg(1) AS avg_grade;

-- Средняя оценка студента по семестрам
CREATE OR REPLACE FUNCTION fn_get_student_avg_by_sem(p_student INT)
RETURNS TABLE(semester TEXT, avg_grade NUMERIC) LANGUAGE sql AS $$
  SELECT 
    CASE WHEN EXTRACT(MONTH FROM date)<=6 THEN 'Second sem' ELSE 'First sem' END,
    AVG(grade)
  FROM Grades
  WHERE student_id = p_student
  GROUP BY 1;
$$;
SELECT * FROM fn_get_student_avg_by_sem(1);

-- Список студентов преподавателя
CREATE OR REPLACE FUNCTION fn_get_teacher_students(p_teacher INT)
RETURNS TABLE(student_id INT, last_name VARCHAR, first_name VARCHAR) LANGUAGE sql AS $$
  SELECT s.student_id, a.last_name, a.first_name
    FROM Grades g
    JOIN Students s ON g.student_id = s.student_id
    JOIN Accounts a ON s.account_id = a.account_id
   WHERE g.teacher_id = p_teacher
   GROUP BY s.student_id,a.last_name,a.first_name;
$$;
SELECT * FROM fn_get_teacher_students(2);

-- Средний балл по группе по предмету (для преподавателя)
CREATE OR REPLACE FUNCTION fn_get_avg_by_group(
  p_teacher INT, p_subject INT)
RETURNS TABLE(group_name VARCHAR, avg_grade NUMERIC) LANGUAGE sql AS $$
  SELECT sg.group_name, AVG(g.grade)
    FROM Grades g
    JOIN Students s ON g.student_id = s.student_id
    JOIN StudyGroups sg ON s.group_id = sg.group_id
   WHERE g.teacher_id=p_teacher AND g.subject_id=p_subject
   GROUP BY sg.group_name;
$$;
SELECT * FROM fn_get_avg_by_group(5,5);

-- !Процент посещаемости группы по предмету
CREATE OR REPLACE FUNCTION fn_get_attendance_pct(
  p_subject INT
) RETURNS NUMERIC LANGUAGE sql AS $$
  SELECT 
    CASE 
      WHEN COUNT(*) > 0 THEN 
        ROUND(
          COUNT(CASE WHEN status = 'присутствовал' THEN 1 END)::NUMERIC / 
          COUNT(*) * 100, 
          2
        )
      ELSE 0 
    END
  FROM Attendance
  WHERE subject_id = p_subject;
$$;
SELECT fn_get_attendance_pct(1) AS pct_present;

-- Список студентов в зоне риска
CREATE OR REPLACE FUNCTION fn_students_at_risk()
RETURNS TABLE(student_id INT, avg_grade NUMERIC) LANGUAGE sql AS $$
  SELECT s.student_id, AVG(g.grade)
    FROM Grades g
    JOIN Students s ON g.student_id=s.student_id
   GROUP BY s.student_id
  HAVING AVG(g.grade)<6;
$$;
SELECT * FROM fn_students_at_risk();

-- Сводка успеваемости по группе
CREATE OR REPLACE FUNCTION fn_group_summary(p_group INT)
RETURNS JSONB LANGUAGE sql AS $$
  SELECT jsonb_build_object(
    'group', sg.group_name,
    'avg_grade', AVG(g.grade)
  )
    FROM Grades g
    JOIN Students s ON g.student_id=s.student_id
    JOIN StudyGroups sg ON s.group_id=sg.group_id
   WHERE sg.group_id=p_group
   GROUP BY sg.group_name;
$$;
SELECT * FROM fn_group_summary(1);

-- Динамика успеваемости за годы
CREATE OR REPLACE FUNCTION fn_performance_dynamics(p_start INT, p_end INT)
RETURNS TABLE(year INT, avg_grade NUMERIC) LANGUAGE sql AS $$
  SELECT EXTRACT(YEAR FROM g.date)::INT, AVG(g.grade)
    FROM Grades g
   WHERE EXTRACT(YEAR FROM g.date) BETWEEN p_start AND p_end
   GROUP BY 1 ORDER BY 1;
$$;
SELECT * FROM fn_performance_dynamics(2024,2025);

-- Импорт в виде функции
CREATE OR REPLACE FUNCTION fn_import_students(p_path TEXT)
RETURNS VOID LANGUAGE plpgsql AS $$
BEGIN
  CALL proc_import_students(p_path);
END; $$;

-- Экспорт в виде функции
CREATE OR REPLACE FUNCTION fn_export_students(p_path TEXT)
RETURNS VOID LANGUAGE plpgsql AS $$
BEGIN
  CALL proc_export_students(p_path);
END; $$;

-- Получение отчёта по типу!!!!!!!!!!!!!!!!!!!!
CREATE OR REPLACE FUNCTION fn_get_reports(p_type TEXT)
RETURNS TABLE(report_id INT, data JSONB, created TIMESTAMP) LANGUAGE sql AS $$
  SELECT report_id, report_data, created_at
    FROM Reports WHERE report_type=p_type;
$$;
SELECT * FROM fn_get_reports(report_type);

-- Получение списка групп
CREATE OR REPLACE FUNCTION fn_list_groups()
RETURNS TABLE(group_id INT, group_name VARCHAR) LANGUAGE sql AS $$
  SELECT group_id, group_name FROM StudyGroups;
$$;
SELECT * FROM fn_list_groups();

-- Список преподавателей
CREATE OR REPLACE FUNCTION fn_list_teachers()
RETURNS TABLE(teacher_id INT, last_name VARCHAR, first_name VARCHAR) LANGUAGE sql AS $$
  SELECT t.teacher_id, a.last_name, a.first_name
    FROM Teachers t
    JOIN Accounts a ON t.account_id=a.account_id;
$$;
SELECT * FROM fn_list_teachers();

-- Список студентов
CREATE OR REPLACE FUNCTION fn_list_students()
RETURNS TABLE(student_id INT, last_name VARCHAR, first_name VARCHAR) LANGUAGE sql AS $$
  SELECT s.student_id, a.last_name, a.first_name
    FROM Students s
    JOIN Accounts a ON s.account_id=a.account_id;
$$;
SELECT * FROM fn_list_students();
