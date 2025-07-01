-- =====================
-- Триггеры для Grades
-- =====================

-- Проверка диапазона оценки (1–10)
CREATE OR REPLACE FUNCTION check_grade_range()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.grade < 1 OR NEW.grade > 10 THEN
    RAISE EXCEPTION 'Оценка % вне допустимого диапазона 1–10', NEW.grade;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_check_grade_range
  BEFORE INSERT OR UPDATE ON Grades
  FOR EACH ROW EXECUTE FUNCTION check_grade_range();

-- Оценка может выставляться только в день занятий (INSERT)
CREATE OR REPLACE FUNCTION check_grade_on_class_day()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF TG_OP = 'INSERT' AND NEW.date <> CURRENT_DATE THEN
    RAISE EXCEPTION 'Дата выставления оценки % должна быть сегодняшней (%).', NEW.date, CURRENT_DATE;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_check_grade_on_class_day
  BEFORE INSERT ON Grades
  FOR EACH ROW EXECUTE FUNCTION check_grade_on_class_day();

-- Контроль доступа преподавателя — только свои предметы
CREATE OR REPLACE FUNCTION teacher_access_control()
RETURNS trigger LANGUAGE plpgsql AS $$
DECLARE
  my_tid INT;
BEGIN
  IF has_role('teacher_role') THEN
    SELECT t.teacher_id INTO my_tid
      FROM Teachers t
      JOIN Accounts a ON t.account_id = a.account_id
     WHERE a.email = session_user;
    IF NEW.teacher_id IS DISTINCT FROM my_tid THEN
      RAISE EXCEPTION 'Преподаватель % не может менять оценки другого преподавателя', session_user;
    END IF;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_teacher_access
  BEFORE INSERT OR UPDATE ON Grades
  FOR EACH ROW EXECUTE FUNCTION teacher_access_control();

-- Уникальность записи оценки (student, subject, date)
CREATE OR REPLACE FUNCTION unique_grade_record()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM Grades
     WHERE student_id = NEW.student_id
       AND subject_id = NEW.subject_id
       AND date       = NEW.date
       AND (TG_OP = 'INSERT' OR grade_id <> NEW.grade_id)
  ) THEN
    RAISE EXCEPTION 'Дублирование оценки (студент %, предмет %, дата %)', 
      NEW.student_id, NEW.subject_id, NEW.date;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_unique_grade
  BEFORE INSERT OR UPDATE ON Grades
  FOR EACH ROW EXECUTE FUNCTION unique_grade_record();

-- Проверка обязательных полей в Grades
CREATE OR REPLACE FUNCTION check_not_null_grade_fields()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.student_id IS NULL OR NEW.subject_id IS NULL OR NEW.teacher_id IS NULL THEN
    RAISE EXCEPTION 'Обязательные поля student_id, subject_id, teacher_id должны быть заполнены';
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_not_null_grade
  BEFORE INSERT OR UPDATE ON Grades
  FOR EACH ROW EXECUTE FUNCTION check_not_null_grade_fields();

-- Логирование изменений оценок
CREATE OR REPLACE FUNCTION log_grade_changes()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  INSERT INTO Reports(report_type, report_data, employee_id, created_at)
    VALUES (
      'grade_change',
      jsonb_build_object('op',TG_OP,'old',row_to_json(OLD),'new',row_to_json(NEW)),
      NULL,
      NOW()
    );
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_log_grade
  AFTER INSERT OR UPDATE ON Grades
  FOR EACH ROW EXECUTE FUNCTION log_grade_changes();

-- ========================
-- Триггеры для Attendance
-- ========================

-- Уникальность записи посещаемости (student, subject, date)
CREATE OR REPLACE FUNCTION unique_attendance_record()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM Attendance
     WHERE student_id      = NEW.student_id
       AND subject_id      = NEW.subject_id
       AND attendance_date = NEW.attendance_date
       AND (TG_OP = 'INSERT' OR attendance_id <> NEW.attendance_id)
  ) THEN
    RAISE EXCEPTION 'Дублирование посещаемости (студент %, предмет %, дата %)',
      NEW.student_id, NEW.subject_id, NEW.attendance_date;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_unique_attendance
  BEFORE INSERT OR UPDATE ON Attendance
  FOR EACH ROW EXECUTE FUNCTION unique_attendance_record();

-- Запрет посещения в будущем
CREATE OR REPLACE FUNCTION check_attendance_date_not_future()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.attendance_date > CURRENT_DATE THEN
    RAISE EXCEPTION 'Дата посещения % не может быть больше текущей', NEW.attendance_date;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_check_attendance_date_not_future
  BEFORE INSERT OR UPDATE ON Attendance
  FOR EACH ROW EXECUTE FUNCTION check_attendance_date_not_future();

-- 2.10 Логирование изменений посещаемости
CREATE OR REPLACE FUNCTION log_attendance_changes()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  INSERT INTO Reports(report_type, report_data, employee_id, created_at)
    VALUES (
      'attendance_change',
      jsonb_build_object('op',TG_OP,'old',row_to_json(OLD),'new',row_to_json(NEW)),
      NULL,
      NOW()
    );
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_log_attendance
  AFTER INSERT OR UPDATE ON Attendance
  FOR EACH ROW EXECUTE FUNCTION log_attendance_changes();

-- =======================
-- Триггеры для Students
-- =======================

-- Уникальность номера зачетной книжки
CREATE OR REPLACE FUNCTION check_unique_record_book()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM Students
     WHERE record_book_id = NEW.record_book_id
       AND (TG_OP = 'INSERT' OR student_id <> NEW.student_id)
  ) THEN
    RAISE EXCEPTION 'Повтор номера зачетной книжки %', NEW.record_book_id;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_unique_record_book
  BEFORE INSERT OR UPDATE ON Students
  FOR EACH ROW EXECUTE FUNCTION check_unique_record_book();

-- Проверка обязательных полей Students
CREATE OR REPLACE FUNCTION check_not_null_student_fields()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.account_id IS NULL OR NEW.group_id IS NULL OR NEW.record_book_id IS NULL THEN
    RAISE EXCEPTION 'В Students должны быть заполнены account_id, group_id, record_book_id';
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_not_null_student
  BEFORE INSERT OR UPDATE ON Students
  FOR EACH ROW EXECUTE FUNCTION check_not_null_student_fields();


-- ========================
-- Триггеры для Teachers
-- ========================

-- Проверка обязательных полей Teachers
CREATE OR REPLACE FUNCTION check_not_null_teacher_fields()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.account_id IS NULL OR NEW.academic_degree IS NULL 
     OR NEW.academic_rank IS NULL OR NEW.education_level IS NULL THEN
    RAISE EXCEPTION 'В Teachers должны быть заполнены account_id, academic_degree, academic_rank, education_level';
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_not_null_teacher
  BEFORE INSERT OR UPDATE ON Teachers
  FOR EACH ROW EXECUTE FUNCTION check_not_null_teacher_fields();

-- Уникальность привязки к аккаунту (один преподаватель на один account_id)
CREATE OR REPLACE FUNCTION unique_teacher_account()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM Teachers
     WHERE account_id = NEW.account_id
       AND (TG_OP = 'INSERT' OR teacher_id <> NEW.teacher_id)
  ) THEN
    RAISE EXCEPTION 'Аккаунт % уже привязан к другому преподавателю', NEW.account_id;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_unique_teacher_account
  BEFORE INSERT OR UPDATE ON Teachers
  FOR EACH ROW EXECUTE FUNCTION unique_teacher_account();

-- =========================
-- Триггеры для Employees
-- =========================

-- Проверка обязательных полей Employees
CREATE OR REPLACE FUNCTION check_not_null_employee_fields()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.account_id IS NULL OR NEW.position IS NULL 
     OR NEW.employment_date IS NULL OR NEW.faculty_id IS NULL THEN
    RAISE EXCEPTION 'В Employees должны быть заполнены account_id, position, employment_date, faculty_id';
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_not_null_employee
  BEFORE INSERT OR UPDATE ON Employees
  FOR EACH ROW EXECUTE FUNCTION check_not_null_employee_fields();

-- Уникальность привязки к аккаунту (один сотрудник на один account_id)
CREATE OR REPLACE FUNCTION unique_employee_account()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM Employees
     WHERE account_id = NEW.account_id
       AND (TG_OP = 'INSERT' OR employee_id <> NEW.employee_id)
  ) THEN
    RAISE EXCEPTION 'Аккаунт % уже привязан к другому сотруднику', NEW.account_id;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_unique_employee_account
  BEFORE INSERT OR UPDATE ON Employees
  FOR EACH ROW EXECUTE FUNCTION unique_employee_account();

-- Запрет даты найма в будущем
CREATE OR REPLACE FUNCTION check_employee_date_not_future()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.employment_date > CURRENT_DATE THEN
    RAISE EXCEPTION 'Дата найма % не может быть в будущем', NEW.employment_date;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_check_employee_date_not_future
  BEFORE INSERT OR UPDATE ON Employees
  FOR EACH ROW EXECUTE FUNCTION check_employee_date_not_future();

-- =========================
-- Триггеры для Subjects
-- =========================

-- Проверка обязательных полей Subjects
CREATE OR REPLACE FUNCTION check_not_null_subject_fields()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.subject_name IS NULL OR NEW.hours IS NULL 
     OR NEW.form_of_assessment IS NULL OR NEW.faculty_id IS NULL THEN
    RAISE EXCEPTION 'В Subjects должны быть заполнены subject_name, hours, form_of_assessment, faculty_id';
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_not_null_subject
  BEFORE INSERT OR UPDATE ON Subjects
  FOR EACH ROW EXECUTE FUNCTION check_not_null_subject_fields();

-- Проверка положительного количества часов
CREATE OR REPLACE FUNCTION check_subject_hours_positive()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.hours <= 0 THEN
    RAISE EXCEPTION 'Количество часов % должно быть положительным', NEW.hours;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_check_subject_hours_positive
  BEFORE INSERT OR UPDATE ON Subjects
  FOR EACH ROW EXECUTE FUNCTION check_subject_hours_positive();

-- Валидация формы аттестации
CREATE OR REPLACE FUNCTION check_subject_form_valid()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.form_of_assessment NOT IN ('экзамен','зачет') THEN
    RAISE EXCEPTION 'Неподдерживаемая форма аттестации: %', NEW.form_of_assessment;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_check_subject_form_valid
  BEFORE INSERT OR UPDATE ON Subjects
  FOR EACH ROW EXECUTE FUNCTION check_subject_form_valid();

-- Уникальность названия предмета в рамках факультета
CREATE OR REPLACE FUNCTION unique_subject_name_per_faculty()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM Subjects
     WHERE subject_name = NEW.subject_name
       AND faculty_id   = NEW.faculty_id
       AND (TG_OP = 'INSERT' OR subject_id <> NEW.subject_id)
  ) THEN
    RAISE EXCEPTION 'Предмет % уже существует на факультете %', NEW.subject_name, NEW.faculty_id;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_unique_subject_name
  BEFORE INSERT OR UPDATE ON Subjects
  FOR EACH ROW EXECUTE FUNCTION unique_subject_name_per_faculty();

-- =========================
-- Триггеры для Faculties
-- =========================

-- Проверка обязательных полей Faculties
CREATE OR REPLACE FUNCTION check_not_null_faculty_fields()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.faculty_name IS NULL OR NEW.description IS NULL THEN
    RAISE EXCEPTION 'В Faculties должны быть заполнены faculty_name, description';
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_not_null_faculty
  BEFORE INSERT OR UPDATE ON Faculties
  FOR EACH ROW EXECUTE FUNCTION check_not_null_faculty_fields();

-- Уникальность названия факультета
CREATE OR REPLACE FUNCTION unique_faculty_name()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM Faculties
     WHERE faculty_name = NEW.faculty_name
       AND (TG_OP = 'INSERT' OR faculty_id <> NEW.faculty_id)
  ) THEN
    RAISE EXCEPTION 'Факультет % уже существует', NEW.faculty_name;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_unique_faculty_name
  BEFORE INSERT OR UPDATE ON Faculties
  FOR EACH ROW EXECUTE FUNCTION unique_faculty_name();

-- =========================
-- Триггеры для Accounts
-- =========================

-- Проверка обязательных полей Accounts
CREATE OR REPLACE FUNCTION check_not_null_account_fields()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.last_name IS NULL OR NEW.first_name IS NULL THEN
    RAISE EXCEPTION 'В Accounts должны быть заполнены last_name, first_name';
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_not_null_account
  BEFORE INSERT OR UPDATE ON Accounts
  FOR EACH ROW EXECUTE FUNCTION check_not_null_account_fields();

-- Валидация формата email
CREATE OR REPLACE FUNCTION check_email_format()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.email !~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' THEN
    RAISE EXCEPTION 'Неверный формат email: %', NEW.email;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_check_email_format
  BEFORE INSERT OR UPDATE ON Accounts
  FOR EACH ROW EXECUTE FUNCTION check_email_format();


-- ===========================
-- Триггеры для StudyGroups
-- ===========================

-- Проверка обязательных полей StudyGroups
CREATE OR REPLACE FUNCTION check_not_null_group_fields()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.group_name IS NULL OR NEW.start_year IS NULL OR NEW.faculty_id IS NULL THEN
    RAISE EXCEPTION 'В StudyGroups должны быть заполнены group_name, start_year, faculty_id';
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_not_null_group
  BEFORE INSERT OR UPDATE ON StudyGroups
  FOR EACH ROW EXECUTE FUNCTION check_not_null_group_fields();

-- Уникальность группы в рамках факультета
CREATE OR REPLACE FUNCTION unique_group_name_per_faculty()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM StudyGroups
     WHERE group_name = NEW.group_name
       AND faculty_id = NEW.faculty_id
       AND (TG_OP = 'INSERT' OR group_id <> NEW.group_id)
  ) THEN
    RAISE EXCEPTION 'Группа % уже существует на факультете %', NEW.group_name, NEW.faculty_id;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_unique_group_name
  BEFORE INSERT OR UPDATE ON StudyGroups
  FOR EACH ROW EXECUTE FUNCTION unique_group_name_per_faculty();

-- Запрет будущего года начала группы
CREATE OR REPLACE FUNCTION check_group_year_not_future()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.start_year > EXTRACT(YEAR FROM CURRENT_DATE) THEN
    RAISE EXCEPTION 'Год начала % не может быть больше текущего', NEW.start_year;
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_check_group_year_not_future
  BEFORE INSERT OR UPDATE ON StudyGroups
  FOR EACH ROW EXECUTE FUNCTION check_group_year_not_future();

-- =========================
-- Триггеры для Reports
-- =========================

-- Проверка обязательных полей Reports
CREATE OR REPLACE FUNCTION check_not_null_report_fields()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.report_type IS NULL OR NEW.report_data IS NULL OR NEW.employee_id IS NULL THEN
    RAISE EXCEPTION 'В Reports должны быть заполнены report_type, report_data, employee_id';
  END IF;
  RETURN NEW;
END; $$;
CREATE TRIGGER trg_not_null_report
  BEFORE INSERT OR UPDATE ON Reports
  FOR EACH ROW EXECUTE FUNCTION check_not_null_report_fields();
