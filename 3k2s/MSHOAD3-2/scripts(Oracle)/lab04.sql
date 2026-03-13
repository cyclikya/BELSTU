-- 2 Вычисление итогов стоимости определенного вида ПО помесячно, за квартал, за полгода, за год.
SELECT 
    s.category AS "Вид ПО",
    EXTRACT(YEAR FROM l.purchase_date) AS "Год",
    EXTRACT(MONTH FROM l.purchase_date) AS "Месяц",
    SUM(l.cost) AS "Стоимость",
    SUM(SUM(l.cost)) OVER (PARTITION BY s.category, EXTRACT(YEAR FROM l.purchase_date), 
                           TO_CHAR(l.purchase_date, 'Q')) AS "Итого за квартал",
    SUM(SUM(l.cost)) OVER (PARTITION BY s.category, EXTRACT(YEAR FROM l.purchase_date),
                           CASE WHEN EXTRACT(MONTH FROM l.purchase_date) BETWEEN 1 AND 6 THEN 1 ELSE 2 END) AS "Итого за полугодие",
    SUM(SUM(l.cost)) OVER (PARTITION BY s.category, EXTRACT(YEAR FROM l.purchase_date)) AS "Итого за год"
FROM Licenses l
JOIN Software s ON l.software_id = s.software_id
WHERE s.category = 'Office'
GROUP BY s.category, EXTRACT(YEAR FROM l.purchase_date), EXTRACT(MONTH FROM l.purchase_date),
         TO_CHAR(l.purchase_date, 'Q')
ORDER BY s.category, EXTRACT(YEAR FROM l.purchase_date), EXTRACT(MONTH FROM l.purchase_date);

------------------------------------------------------------------------- не работает
-- 3 Вычисление итогов стоимости определенного вида ПО за период:
    -- количество и стоимость лицензий;
    -- сравнение их с общим количество лицензий (в %);
    -- сравнение их с общей стоимостью лицензий (в %).
WITH LicenseStats AS (
    SELECT 
        s.category,
        COUNT(l.license_id) AS licenses_count,
        SUM(l.cost) AS licenses_cost,
        (SELECT COUNT(*) FROM Licenses) AS total_licenses_count,
        (SELECT SUM(cost) FROM Licenses) AS total_licenses_cost
    FROM Licenses l
    JOIN Software s ON l.software_id = s.software_id
    WHERE l.purchase_date BETWEEN DATE '2025-01-01' AND DATE '2025-12-31'
        AND s.category = 'Office'
    GROUP BY s.category
)
SELECT 
    category AS "Вид ПО",
    licenses_count AS "Количество лицензий",
    licenses_cost AS "Стоимость лицензий",
    ROUND(licenses_count / total_licenses_count * 100, 2) AS "Доля от общего кол-ва (%)",
    ROUND(licenses_cost / total_licenses_cost * 100, 2) AS "Доля от общей стоимости (%)"
FROM LicenseStats;


-- 4 Функции ранжирования ROW_NUMBER() для разбиения результатов запроса на страницы (по 20 строк на каждую страницу).
-- Страница 1 (строки 1-20)
SELECT * FROM (
    SELECT 
        e.employee_id,
        e.full_name,
        e.email,
        d.department_name,
        ROW_NUMBER() OVER (ORDER BY e.employee_id) AS row_num
    FROM Employees e
    LEFT JOIN Departments d ON e.department_id = d.department_id
)
WHERE row_num BETWEEN 1 AND 20;

-- Страница 2 (строки 21-40)
SELECT * FROM (
    SELECT 
        e.employee_id,
        e.full_name,
        e.email,
        d.department_name,
        ROW_NUMBER() OVER (ORDER BY e.employee_id) AS row_num
    FROM Employees e
    LEFT JOIN Departments d ON e.department_id = d.department_id
)
WHERE row_num BETWEEN 21 AND 40;


-- 5 Функция ранжирования ROW_NUMBER() для удаления дубликатов
INSERT INTO Employees VALUES (35, 'Alice Smith', 'alice_smith@repairbuild.com', 'Middle', 7);

DELETE FROM Employees
WHERE employee_id IN (
    SELECT employee_id
    FROM (
        SELECT 
            employee_id,
            ROW_NUMBER() OVER (PARTITION BY full_name ORDER BY employee_id) AS rn
        FROM Employees
    )
    WHERE rn > 1
);

WITH Duplicates AS (
    SELECT 
        employee_id,
        full_name,
        email,
        ROW_NUMBER() OVER (PARTITION BY full_name ORDER BY employee_id) AS rn
    FROM Employees
)
SELECT * FROM Duplicates
WHERE rn > 1
ORDER BY full_name;


-- 6 Вернуть для каждого вендора суммы затраченных на лицензирование средств за последние 6 месяцев помесячно.
SELECT 
    v.vendor_name AS "Вендор",
    EXTRACT(YEAR FROM l.purchase_date) AS "Год",
    EXTRACT(MONTH FROM l.purchase_date) AS "Месяц",
    SUM(l.cost) AS "Сумма затрат"
FROM Vendors v
JOIN Software s ON v.vendor_id = s.vendor_id
JOIN Licenses l ON s.software_id = l.software_id
WHERE l.purchase_date >= ADD_MONTHS(SYSDATE, -6)
GROUP BY v.vendor_name, EXTRACT(YEAR FROM l.purchase_date), 
         EXTRACT(MONTH FROM l.purchase_date), TO_CHAR(l.purchase_date, 'YYYY-MM')
ORDER BY v.vendor_name, EXTRACT(YEAR FROM l.purchase_date), EXTRACT(MONTH FROM l.purchase_date);


-- 7 Какой тип программного обеспечения использовался наибольшее число раз для устройств определенного вида? Вернуть для всех видов.
WITH DeviceSoftwareUsage AS (
    SELECT 
        la.device_type,
        s.category,
        COUNT(*) AS usage_count,
        ROW_NUMBER() OVER (PARTITION BY la.device_type ORDER BY COUNT(*) DESC) AS rn
    FROM LicenseAssignments la
    JOIN Licenses l ON la.license_id = l.license_id
    JOIN Software s ON l.software_id = s.software_id
    WHERE la.device_type IS NOT NULL
    GROUP BY la.device_type, s.category
)
SELECT 
    device_type AS "Тип устройства",
    category AS "Тип ПО",
    usage_count AS "Количество использований"
FROM DeviceSoftwareUsage
WHERE rn = 1
ORDER BY device_type;