-- Добавление оценки
CREATE OR REPLACE PROCEDURE proc_add_grade(
  p_student_id INT, p_subject_id INT, p_teacher_id INT, p_grade INT, p_date DATE)
LANGUAGE plpgsql AS $$
BEGIN
  INSERT INTO Grades(student_id,subject_id,teacher_id,grade,date)
  VALUES(p_student_id,p_subject_id,p_teacher_id,p_grade,p_date);
END; $$;

-- Обновление оценки
CREATE OR REPLACE PROCEDURE proc_update_grade(
  p_grade_id INT, p_new_grade INT)
LANGUAGE plpgsql AS $$
BEGIN
  UPDATE Grades SET grade = p_new_grade WHERE grade_id = p_grade_id;
END; $$;

-- Удаление оценки
CREATE OR REPLACE PROCEDURE proc_delete_grade(p_grade_id INT)
LANGUAGE plpgsql AS $$
BEGIN
  DELETE FROM Grades WHERE grade_id = p_grade_id;
END; $$;

-- Добавление посещаемости
CREATE OR REPLACE PROCEDURE proc_add_attendance(
  p_student_id INT, p_subject_id INT, p_date DATE, p_status VARCHAR)
LANGUAGE plpgsql AS $$
BEGIN
  INSERT INTO Attendance(student_id,subject_id,attendance_date,status)
  VALUES(p_student_id,p_subject_id,p_date,p_status);
END; $$;
CALL proc_add_attendance(1,1, CURRENT_DATE, 'присутствовал');

-- Обновление статуса посещаемости
CREATE OR REPLACE PROCEDURE proc_update_attendance(
  p_att_id INT, p_new_status VARCHAR)
LANGUAGE plpgsql AS $$
BEGIN
  UPDATE Attendance SET status = p_new_status WHERE attendance_id = p_att_id;
END; $$;
CALL proc_update_attendance(1, 'присутствовал');

-- Удаление записи посещаемости
CREATE OR REPLACE PROCEDURE proc_delete_attendance(p_att_id INT)
LANGUAGE plpgsql AS $$
BEGIN
  DELETE FROM Attendance WHERE attendance_id = p_att_id;
END; $$;
CALL proc_update_attendance(1);

-- Добавление студента
CREATE OR REPLACE PROCEDURE proc_add_student(
  p_account_id INT, p_record_book VARCHAR, p_group_id INT)
LANGUAGE plpgsql AS $$
BEGIN
  INSERT INTO Students(account_id,record_book_id,group_id)
    VALUES(p_account_id,p_record_book,p_group_id);
END; $$;

-- Обновление данных студента
CREATE OR REPLACE PROCEDURE proc_update_student(
  p_student_id INT, p_group_id INT)
LANGUAGE plpgsql AS $$
BEGIN
  UPDATE Students SET group_id = p_group_id WHERE student_id = p_student_id;
END; $$;

-- Удаление студента
CREATE OR REPLACE PROCEDURE proc_delete_student(p_student_id INT)
LANGUAGE plpgsql AS $$
BEGIN
  DELETE FROM Students WHERE student_id = p_student_id;
END; $$;

-- Добавление преподавателя
CREATE OR REPLACE PROCEDURE proc_add_teacher(
  p_account_id INT, p_degree VARCHAR, p_rank VARCHAR, p_level VARCHAR)
LANGUAGE plpgsql AS $$
BEGIN
  INSERT INTO Teachers(account_id,academic_degree,academic_rank,education_level)
    VALUES(p_account_id,p_degree,p_rank,p_level);
END; $$;

-- Обновление преподавателя
CREATE OR REPLACE PROCEDURE proc_update_teacher(
  p_teacher_id INT, p_rank VARCHAR)
LANGUAGE plpgsql AS $$
BEGIN
  UPDATE Teachers SET academic_rank = p_rank WHERE teacher_id = p_teacher_id;
END; $$;

-- Удаление преподавателя
CREATE OR REPLACE PROCEDURE proc_delete_teacher(p_teacher_id INT)
LANGUAGE plpgsql AS $$
BEGIN
  DELETE FROM Teachers WHERE teacher_id = p_teacher_id;
END; $$;

-- Назначение роли пользователю
CREATE OR REPLACE PROCEDURE proc_grant_role(
  p_username TEXT, p_rolename TEXT)
LANGUAGE plpgsql AS $$
BEGIN
  EXECUTE format('GRANT %I TO %I', p_rolename, p_username);
END; $$;

-- Отзыв роли
CREATE OR REPLACE PROCEDURE proc_revoke_role(
  p_username TEXT, p_rolename TEXT)
LANGUAGE plpgsql AS $$
BEGIN
  EXECUTE format('REVOKE %I FROM %I', p_rolename, p_username);
END; $$;

-- Создание отчёта
CREATE OR REPLACE PROCEDURE proc_create_report(
  p_type VARCHAR, p_data JSONB, p_emp_id INT)
LANGUAGE plpgsql AS $$
BEGIN
  INSERT INTO Reports(report_type,report_data,employee_id,created_at)
    VALUES(p_type,p_data,p_emp_id,NOW());
END; $$;
CALL proc_create_report(
  'manual_report', 
  '{"message": "This is a test report", "value": 42}'::jsonb, 
  1
);

-- Генерация отчёта успеваемости студента
CREATE OR REPLACE PROCEDURE proc_student_report(p_stud_id INT, p_empl_id INT)
LANGUAGE plpgsql AS $$
DECLARE
  v_report_data JSONB;
BEGIN
  SELECT json_build_object(
      'student_id', p_stud_id,
      'avg_grade', AVG(grade)
    )::JSONB
  INTO v_report_data
  FROM Grades 
  WHERE student_id = p_stud_id;
  CALL proc_create_report(
    'Успеваемость',
    v_report_data,
    p_empl_id
  );
END; $$;
CALL proc_student_report(1, 1);
SELECT * FROM Reports WHERE report_type = 'Успеваемость';

-- Генерация отчёта для отдела статистики
CREATE OR REPLACE PROCEDURE proc_stats_report(p_year INT, p_empl_id INT)
LANGUAGE plpgsql AS $$
DECLARE
    report_data JSONB;
BEGIN
    -- Сначала получаем агрегированные данные по группам
    WITH group_stats AS (
        SELECT 
            s.group_id,
            AVG(g.grade) AS avg_grade
        FROM Grades g
        JOIN Students s ON g.student_id = s.student_id
        WHERE EXTRACT(YEAR FROM g.date) = p_year
        GROUP BY s.group_id
    )
    -- Затем преобразуем в JSON
    SELECT json_agg(json_build_object(
        'group_id', group_id,
        'avg_grade', avg_grade
    ))::JSONB
    INTO report_data
    FROM group_stats;
    
    -- Вызываем процедуру создания отчета
    CALL proc_create_report(
        'Годовой отчет',
        report_data,
        p_empl_id
    );
END; $$;
CALL proc_stats_report(2024, 1);
SELECT * FROM Reports WHERE report_type = 'Годовой отчет';