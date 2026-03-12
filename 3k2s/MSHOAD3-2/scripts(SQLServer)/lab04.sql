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
FROM Licenses l
JOIN Software s ON l.software_id = s.software_id
WHERE s.category = 'Office'
GROUP BY s.category, YEAR(l.purchase_date), MONTH(l.purchase_date), DATEPART(quarter, l.purchase_date)
ORDER BY s.category, YEAR(l.purchase_date), MONTH(l.purchase_date);

-- 3 Вычисление итогов стоимости определенного вида ПО за период:
    -- количество и стоимость лицензий;
    -- сравнение их с общим количество лицензий (в %);
    -- сравнение их с общей стоимостью лицензий (в %).
DECLARE @start_date DATE = '2025-01-01';
DECLARE @end_date DATE = '2025-12-31';

WITH ServiceVolume AS (
    -- Объем услуг - количество назначений лицензий
    SELECT 
        COUNT(*) AS period_volume,
        (SELECT COUNT(*) FROM LicenseAssignments) AS total_volume,
        (SELECT MAX(period_count) FROM (
            SELECT COUNT(*) AS period_count 
            FROM LicenseAssignments 
            WHERE assigned_date BETWEEN @start_date AND @end_date
            GROUP BY license_id
        ) AS max_sub) AS max_per_license
    FROM LicenseAssignments
    WHERE assigned_date BETWEEN @start_date AND @end_date
)
SELECT 
    period_volume AS 'Объем услуг за период',
    total_volume AS 'Общий объем услуг',
    ROUND(CAST(period_volume AS FLOAT) / total_volume * 100, 2) AS 'Доля от общего объема (%)',
    max_per_license AS 'Максимальный объем (одна лицензия)',
    ROUND(CAST(period_volume AS FLOAT) / NULLIF(max_per_license, 0) * 100, 2) AS 'Отношение к максимуму (%)'
FROM ServiceVolume;

