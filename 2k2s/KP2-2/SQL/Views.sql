-- Успеваемость студента
CREATE OR REPLACE VIEW view_student_grades AS
SELECT s.student_id, a.last_name, a.first_name,
       sub.subject_name, g.grade, g.date
  FROM Grades g
  JOIN Students s ON g.student_id=s.student_id
  JOIN Accounts a ON s.account_id=a.account_id
  JOIN Subjects sub ON g.subject_id=sub.subject_id;

-- Средний балл студента
CREATE OR REPLACE VIEW view_student_avg AS
SELECT student_id, AVG(grade)::NUMERIC AS avg_grade
  FROM Grades GROUP BY student_id;

-- Средний балл по семестрам
CREATE OR REPLACE VIEW view_student_avg_sem AS
SELECT student_id,
       CASE WHEN EXTRACT(MONTH FROM date)<=6 THEN 'Spring' ELSE 'Fall' END AS semester,
       AVG(grade)::NUMERIC AS avg_grade
  FROM Grades GROUP BY student_id, CASE WHEN EXTRACT(MONTH FROM date)<=6 THEN 'Spring' ELSE 'Fall' END;

-- Список студентов преподавателя (группировка по факультету и группе)
CREATE OR REPLACE VIEW view_teacher_students AS
SELECT t.teacher_id, f.faculty_name, sg.group_name, s.student_id, a.last_name, a.first_name
  FROM Grades g
  JOIN Teachers t ON g.teacher_id=t.teacher_id
  JOIN Students s ON g.student_id=s.student_id
  JOIN Accounts a ON s.account_id=a.account_id
  JOIN StudyGroups sg ON s.group_id=sg.group_id
  JOIN Faculties f ON sg.faculty_id=f.faculty_id
 ORDER BY f.faculty_name, sg.group_name;

-- Средний балл группы по предмету (для преподавателя)
CREATE OR REPLACE VIEW view_avg_by_group_subject AS
SELECT t.teacher_id, sub.subject_name, sg.group_name, AVG(g.grade)::NUMERIC AS avg_grade
  FROM Grades g
  JOIN Subjects sub ON g.subject_id=sub.subject_id
  JOIN Students s ON g.student_id=s.student_id
  JOIN StudyGroups sg ON s.group_id=sg.group_id
  JOIN Teachers t ON g.teacher_id=t.teacher_id
 GROUP BY t.teacher_id, sub.subject_name, sg.group_name;

-- Отчёт по посещаемости группы (в %)
CREATE OR REPLACE VIEW view_attendance_pct AS
SELECT t.teacher_id, sub.subject_name, sg.group_name,
       100.0 * SUM(CASE WHEN att.status='присутствовал' THEN 1 ELSE 0 END)::NUMERIC
           / COUNT(*) AS pct_present
  FROM Attendance att
  JOIN Subjects sub ON att.subject_id=sub.subject_id
  JOIN Students s ON att.student_id=s.student_id
  JOIN StudyGroups sg ON s.group_id=sg.group_id
  JOIN Teachers t ON sub.faculty_id=sg.faculty_id
 GROUP BY t.teacher_id, sub.subject_name, sg.group_name;

-- Репорт по успеваемости по группам/предметам (админ)
CREATE OR REPLACE VIEW view_admin_performance AS
SELECT f.faculty_name, sg.group_name, sub.subject_name,
       AVG(g.grade)::NUMERIC AS avg_grade
  FROM Grades g
  JOIN Students s ON g.student_id=s.student_id
  JOIN StudyGroups sg ON s.group_id=sg.group_id
  JOIN Faculties f ON sg.faculty_id=f.faculty_id
  JOIN Subjects sub ON g.subject_id=sub.subject_id
 GROUP BY f.faculty_name, sg.group_name, sub.subject_name;

-- Студенты в зоне риска
CREATE OR REPLACE VIEW view_students_risk AS
SELECT s.student_id, a.last_name, a.first_name, AVG(g.grade)::NUMERIC AS avg_grade
  FROM Grades g
  JOIN Students s ON g.student_id=s.student_id
  JOIN Accounts a ON s.account_id=a.account_id
 GROUP BY s.student_id,a.last_name,a.first_name
HAVING AVG(g.grade)<6;

-- Сводный отчёт отдела статистики по группам
CREATE OR REPLACE VIEW view_stats_summary AS
SELECT 
    sg.group_id, 
    sg.group_name,
    json_agg(	 json_build_object(
            	'year', year_data.year,
            	'avg_grade', year_data.avg_grade
        )
    ) AS dynamics
FROM (
    SELECT 
        sg.group_id,
        EXTRACT(YEAR FROM g.date) AS year,
        AVG(g.grade) AS avg_grade
    FROM Grades g
    JOIN Students s ON g.student_id = s.student_id
    JOIN StudyGroups sg ON s.group_id = sg.group_id
    GROUP BY sg.group_id, EXTRACT(YEAR FROM g.date)
) AS year_data
JOIN StudyGroups sg ON year_data.group_id = sg.group_id
GROUP BY sg.group_id, sg.group_name;

-- Отчёт динамики успеваемости
CREATE OR REPLACE VIEW view_stats_dynamics AS
SELECT EXTRACT(YEAR FROM g.date)::INT AS year,
       AVG(g.grade)::NUMERIC AS avg_grade
  FROM Grades g
 GROUP BY year ORDER BY year;