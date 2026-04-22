/* Задание 1.
   MODEL: план количества лицензий по "классам (помещениям)"
   В текущей БД отдельной таблицы классов нет,
   поэтому используем Departments как аналог помещений.
*/

WITH dept_base AS (
    SELECT
        d.department_id,
        d.department_name,
        COUNT(DISTINCT e.employee_id) AS employee_count,
        COUNT(CASE WHEN UPPER(la.status) = 'ACTIVE' THEN 1 END) AS current_license_count
    FROM Departments d
    LEFT JOIN Employees e
        ON e.department_id = d.department_id
    LEFT JOIN LicenseAssignments la
        ON la.employee_id = e.employee_id
    GROUP BY d.department_id, d.department_name
),
months AS (
    SELECT LEVEL - 1 AS month_no
    FROM dual
    CONNECT BY LEVEL <= 13
),
src AS (
    SELECT
        b.department_id,
        b.department_name,
        m.month_no,
        CASE
            WHEN m.month_no = 0 THEN b.current_license_count
            ELSE 0
        END AS planned_license_count,
        b.employee_count
    FROM dept_base b
    CROSS JOIN months m
)
SELECT
    department_id,
    department_name,
    TO_CHAR(ADD_MONTHS(DATE '2025-12-01', month_no), 'MM.YYYY') AS plan_month,
    planned_license_count
FROM src
MODEL
    PARTITION BY (department_id, department_name)
    DIMENSION BY (month_no)
    MEASURES (
        planned_license_count,
        employee_count
    )
    RULES SEQUENTIAL ORDER (
        planned_license_count[FOR month_no FROM 1 TO 12 INCREMENT 1] =
            GREATEST(
                ROUND(
                    planned_license_count[CV(month_no) - 1] * 1.03 * 0.98
                ),
                0
            )
    )
ORDER BY department_id, plan_month;





/* Задание 2.
   MATCH_RECOGNIZE: рост, падение, рост стоимости лицензий
   для каждого вида ПО
*/
WITH license_costs AS (
    SELECT
        s.software_id,
        s.name AS software_name,
        l.license_id,
        l.license_key,
        l.purchase_date,
        l.cost
    FROM Software s
    JOIN Licenses l
        ON l.software_id = s.software_id
)
SELECT
    software_name,
    start_date,
    start_cost,
    rise_date,
    rise_cost,
    fall_date,
    fall_cost,
    rise2_date,
    rise2_cost
FROM license_costs
MATCH_RECOGNIZE (
    PARTITION BY software_name
    ORDER BY purchase_date
    MEASURES
        FIRST(start_row.purchase_date) AS start_date,
        FIRST(start_row.cost)          AS start_cost,
        FIRST(rise1.purchase_date)     AS rise_date,
        FIRST(rise1.cost)              AS rise_cost,
        FIRST(fall1.purchase_date)     AS fall_date,
        FIRST(fall1.cost)              AS fall_cost,
        FIRST(rise2.purchase_date)     AS rise2_date,
        FIRST(rise2.cost)              AS rise2_cost
    ONE ROW PER MATCH
    PATTERN (start_row rise1 fall1 rise2)
    DEFINE
        rise1 AS rise1.cost > PREV(rise1.cost),
        fall1 AS fall1.cost < PREV(fall1.cost),
        rise2 AS rise2.cost > PREV(rise2.cost)
)
ORDER BY software_name, start_date;