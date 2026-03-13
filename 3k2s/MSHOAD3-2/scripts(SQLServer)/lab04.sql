-- 2 Вычисление итогов стоимости определенного вида ПО помесячно, за квартал, за полгода, за год.
SELECT 
    s.category AS 'Вид ПО',
    YEAR(l.purchase_date) AS 'Год',
    MONTH(l.purchase_date) AS 'Месяц',
    SUM(l.cost) AS 'Стоимость',
    SUM(SUM(l.cost)) OVER (PARTITION BY s.category, YEAR(l.purchase_date), DATEPART(quarter, l.purchase_date)) AS 'Итого за квартал',
    SUM(SUM(l.cost)) OVER (PARTITION BY s.category, YEAR(l.purchase_date), 
                           CASE WHEN MONTH(l.purchase_date) BETWEEN 1 AND 6 THEN 1 ELSE 2 END) AS 'Итого за полугодие',
    SUM(SUM(l.cost)) OVER (PARTITION BY s.category, YEAR(l.purchase_date)) AS 'Итого за год'
FROM Licenses l JOIN Software s ON l.software_id = s.software_id
    WHERE s.category = 'Office'
    GROUP BY s.category, YEAR(l.purchase_date), MONTH(l.purchase_date), DATEPART(quarter, l.purchase_date)
    ORDER BY s.category, YEAR(l.purchase_date), MONTH(l.purchase_date);



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
    WHERE l.purchase_date BETWEEN '2025-01-01' AND '2025-12-31'
        AND s.category = 'Office'
    GROUP BY s.category
)
SELECT 
    category AS [Вид ПО],
    licenses_count AS [Количество лицензий],
    licenses_cost AS [Стоимость лицензий],
    ROUND(licenses_count * 100.0 / total_licenses_count, 2) AS [Доля от общего кол-ва (%)],
    ROUND(licenses_cost * 100.0 / total_licenses_cost, 2) AS [Доля от общей стоимости (%)]
FROM LicenseStats;


-- 4 Функция ранжирования ROW_NUMBER() для разбиения результатов запроса на страницы (по 20 строк на каждую страницу).
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
) AS paged
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
) AS paged
WHERE row_num BETWEEN 21 AND 40;


-- 5 Функция ранжирования ROW_NUMBER() для удаления дубликатов
INSERT INTO Employees VALUES ('Alice Smith', 'alice_smith@repairbuild.com', 'Middle', 7);

WITH Duplicates AS (
    SELECT 
        employee_id,
        full_name,
        email,
        ROW_NUMBER() OVER (PARTITION BY full_name ORDER BY employee_id) AS rn
    FROM Employees
)
DELETE FROM Employees
WHERE employee_id IN (
    SELECT employee_id 
    FROM Duplicates 
    WHERE rn > 1
);

DECLARE @max_id INT;
SELECT @max_id = ISNULL(MAX(employee_id), 0) FROM Employees;
DBCC CHECKIDENT ('Employees', RESEED, @max_id);

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
DECLARE @six_months_ago DATE = DATEADD(MONTH, -6, GETDATE());

SELECT 
    v.vendor_name AS 'Вендор',
    YEAR(l.purchase_date) AS 'Год',
    MONTH(l.purchase_date) AS 'Месяц',
    SUM(l.cost) AS 'Сумма затрат'
FROM Vendors v
JOIN Software s ON v.vendor_id = s.vendor_id
JOIN Licenses l ON s.software_id = l.software_id
WHERE l.purchase_date >= @six_months_ago
GROUP BY v.vendor_name, YEAR(l.purchase_date), MONTH(l.purchase_date), FORMAT(l.purchase_date, 'yyyy-MM')
ORDER BY v.vendor_name, YEAR(l.purchase_date), MONTH(l.purchase_date);


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
    device_type AS 'Тип устройства',
    category AS 'Тип ПО',
    usage_count AS 'Количество использований'
FROM DeviceSoftwareUsage
WHERE rn = 1
ORDER BY device_type;

