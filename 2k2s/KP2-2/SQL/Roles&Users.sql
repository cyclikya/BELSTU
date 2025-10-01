-- Создание ролей
CREATE ROLE student_role;
CREATE ROLE teacher_role;
CREATE ROLE admin_role;
CREATE ROLE analytics_role;

-- пользователи
CREATE USER student_user WITH PASSWORD 'student_pass';
CREATE USER teacher_user WITH PASSWORD 'teacher_pass';
CREATE USER admin_user   WITH PASSWORD 'admin_pass';
CREATE USER analytics_user WITH PASSWORD 'analytics_pass';

-- привязка ролей к пользователям
GRANT student_role   TO student_user;
GRANT teacher_role   TO teacher_user;
GRANT admin_role     TO admin_user;
GRANT analytics_role TO analytics_user;

-- студент: только чтение своих view-функций
GRANT SELECT ON view_student_grades, view_student_avg, view_student_avg_sem, view_students_risk TO student_role;
GRANT EXECUTE ON FUNCTION fn_get_student_grades(INT), fn_get_student_avg(INT), fn_get_student_avg_by_sem(INT) TO student_role;

-- преподаватель: чтение + добавление/обновление оценок и отчётов по своим предметам
GRANT SELECT ON view_teacher_students,view_student_grades, view_avg_by_group_subject, view_attendance_pct TO teacher_role;
GRANT EXECUTE ON PROCEDURE proc_add_grade(INT,INT,INT,INT,DATE), proc_update_grade(INT,INT) TO teacher_role;
GRANT EXECUTE ON FUNCTION fn_get_teacher_students(INT), fn_get_avg_by_group(INT,INT), fn_get_attendance_pct(INT,INT) TO teacher_role;

-- администратор: полный доступ ко всему
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO admin_role;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO admin_role;
GRANT EXECUTE ON ALL PROCEDURES IN SCHEMA public TO admin_role;

-- аналитик: доступ только к отчётным view и функциям
GRANT SELECT ON view_admin_performance, view_stats_summary, view_stats_dynamics TO analytics_role;
GRANT EXECUTE ON FUNCTION fn_students_at_risk(), fn_group_summary(INT), fn_performance_dynamics(INT,INT), fn_get_reports(TEXT) TO analytics_role;