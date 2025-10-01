-- Процедура импорта студентов
CREATE OR REPLACE PROCEDURE proc_import_students(p_path text)
LANGUAGE plpgsql
AS $$
DECLARE
  json_data json;
  student_record json; -- переименовали переменную
BEGIN
  -- Читаем JSON файл
  json_data := pg_read_file(p_path)::json;
  -- Вставляем/обновляем аккаунты (используем алиас для элемента массива)
  INSERT INTO Accounts (
    account_id,
    last_name,
    first_name,
    middle_name,
    address,
    phone_number,
    email
  )
  SELECT 
    (account_data->>'account_id')::INT, -- используем алиас account_data
    account_data->>'last_name',
    account_data->>'first_name',
    NULLIF(account_data->>'middle_name', ''),
    NULLIF(account_data->>'address', ''),
    NULLIF(account_data->>'phone_number', ''),
    account_data->>'email'
  FROM json_array_elements(json_data) AS account_data -- алиас здесь
  ON CONFLICT (account_id) DO UPDATE SET
    last_name = EXCLUDED.last_name,
    first_name = EXCLUDED.first_name,
    middle_name = EXCLUDED.middle_name,
    address = EXCLUDED.address,
    phone_number = EXCLUDED.phone_number,
    email = EXCLUDED.email;
  
  -- Вставляем/обновляем студентов
  FOR student_record IN SELECT * FROM json_array_elements(json_data)
  LOOP
    -- Проверяем обязательные поля
    IF student_record->>'student_id' IS NULL OR 
       student_record->>'record_book_id' IS NULL OR 
       student_record->>'group_id' IS NULL THEN
      RAISE WARNING 'Пропущена запись с отсутствующими обязательными полями: %', student_record;
      CONTINUE;
    END IF;
    
    INSERT INTO Students (
      student_id,
      account_id,
      record_book_id,
      group_id
    )
    VALUES (
      (student_record->>'student_id')::INT,
      (student_record->>'account_id')::INT,
      student_record->>'record_book_id',
      (student_record->>'group_id')::INT
    )
    ON CONFLICT (student_id) DO UPDATE SET
      account_id = EXCLUDED.account_id,
      record_book_id = EXCLUDED.record_book_id,
      group_id = EXCLUDED.group_id;
  END LOOP;
END;
$$;

-- Процедура экспорта студентов
CREATE OR REPLACE PROCEDURE proc_export_students(p_path TEXT)
LANGUAGE plpgsql
AS $$
DECLARE
  js TEXT;
BEGIN
  -- Собираем данные студентов с информацией об аккаунтах
  SELECT json_agg(json_build_object(
    'student_id', s.student_id,
    'account_id', s.account_id,
    'record_book_id', s.record_book_id,
    'group_id', s.group_id,
    'last_name', a.last_name,
    'first_name', a.first_name,
    'middle_name', a.middle_name,
    'address', a.address,
    'phone_number', a.phone_number,
    'email', a.email
  ))::TEXT INTO js
  FROM Students s
  JOIN Accounts a ON s.account_id = a.account_id;
    EXECUTE format('COPY (SELECT %L) TO %L', js, p_path);
END;
$$;