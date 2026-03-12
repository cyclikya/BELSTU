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


-- 3 Вычисление итогов стоимости определенного вида ПО за период:
    -- количество и стоимость лицензий;
    -- сравнение их с общим количество лицензий (в %);
    -- сравнение их с общей стоимостью лицензий (в %).
