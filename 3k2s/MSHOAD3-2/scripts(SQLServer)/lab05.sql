SELECT * FROM countries;
SELECT * FROM places;
SELECT * FROM rivers;

-- 6 Тип пространственных данных
SELECT DISTINCT geom.STGeometryType() AS GeometryType
FROM dbo.countries;

SELECT DISTINCT geom.STGeometryType() AS GeometryType
FROM dbo.rivers;

SELECT DISTINCT geom.STGeometryType() AS GeometryType
FROM dbo.places;


-- 7 SRID
SELECT DISTINCT geom.STSrid AS SRID
FROM dbo.countries;

SELECT DISTINCT geom.STSrid AS SRID
FROM dbo.rivers;

SELECT DISTINCT geom.STSrid AS SRID
FROM dbo.places;

-- 8 атрибутивные столбцы
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'countries' 
  AND DATA_TYPE NOT IN ('geometry', 'geography');

SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'rivers' 
  AND DATA_TYPE NOT IN ('geometry', 'geography');

SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'places' 
  AND DATA_TYPE NOT IN ('geometry', 'geography');

-- 9 в формате WKT
SELECT TOP 3 
    NAME,
    geom.STAsText() AS WKT_Geometry
FROM dbo.countries;

SELECT TOP 3 
    name,
    geom.STAsText() AS WKT_Geometry
FROM dbo.rivers;

SELECT TOP 3 
    name,
    geom.STAsText() AS WKT_Geometry
FROM dbo.places;


-- 10
-- 10.1 Нахождение пересечения пространственных объектов STIntersection() и STIntersects()
DECLARE @russia geometry;

SELECT @russia = geom 
FROM dbo.countries 
WHERE NAME = 'Russia';

SELECT 
    c.NAME,
    c.geom.STIntersection(@russia).STAsText() AS IntersectionWKT
FROM dbo.countries c
WHERE c.geom.STIntersects(@russia) = 1
  AND c.NAME != 'Russia';


-- 10.2 Нахождение объединения пространственных объектов STUnion()
DECLARE @russia geometry;

SELECT @russia = geom 
FROM dbo.countries 
WHERE NAME = 'Russia';

SELECT 
    r.name AS RiverName,
    r.geom.STIntersection(@russia).STAsText() AS IntersectionWKT
FROM dbo.rivers r
WHERE r.geom.STIntersects(@russia) = 1;


-- 10.3 Нахождение вложенности пространственных объектов STContains() и STWithin()
DECLARE @france geometry;
DECLARE @paris geometry;

SELECT @france = geom FROM dbo.countries WHERE NAME = 'France';
SELECT @paris = geom FROM dbo.places WHERE name = 'Paris';

SELECT 
    @france.STContains(@paris) AS FranceContainsParis,
    @paris.STWithin(@france) AS ParisWithinFrance;


-- 10.4 Упрощение пространственного объекта Reduce()
DECLARE @russia geometry;

SELECT @russia = geom 
FROM dbo.countries 
WHERE NAME = 'Russia';

SELECT 
    -- Исходное количество точек
    @russia.STNumPoints() AS OriginalPoints,
    
    -- Упрощение с допуском 1 градус
    @russia.Reduce(1).STNumPoints() AS SimplifiedPoints_1,
    @russia.Reduce(1).STAsText() AS SimplifiedWKT_1,
    
    -- Упрощение с допуском 5 градусов (более грубое)
    @russia.Reduce(5).STNumPoints() AS SimplifiedPoints_5,
    @russia.Reduce(5).STAsText() AS SimplifiedWKT_5;


-- 10.5 Нахождение координат вершин пространственного объектов STPointN()
-- Координаты вершин первой страны
DECLARE @geom geometry;

SELECT @geom = geom 
FROM dbo.countries 
WHERE NAME = 'Italy';

-- Количество точек
SELECT @geom.STNumPoints() AS NumPoints;

-- Получить координаты каждой вершины
-- Используем рекурсивный CTE для перебора всех точек
;WITH Points AS (
    SELECT 1 AS PointNum
    UNION ALL
    SELECT PointNum + 1 
    FROM Points 
    WHERE PointNum < @geom.STNumPoints()
)
SELECT 
    PointNum,
    @geom.STPointN(PointNum).STX AS Longitude,
    @geom.STPointN(PointNum).STY AS Latitude
FROM Points
OPTION (MAXRECURSION 10000);


-- 10.6 Нахождение размерности пространственных объектов STDimension() 
SELECT DISTINCT Dim FROM (
    SELECT geom.STDimension() AS Dim FROM dbo.countries
) t;

SELECT DISTINCT Dim FROM (
    SELECT geom.STDimension() AS Dim FROM dbo.rivers
) t;

SELECT DISTINCT Dim FROM (
    SELECT geom.STDimension() AS Dim FROM dbo.places
) t;


-- 10.7 Нахождение длины и площади пространственных объектов STLength() и STArea() 
SELECT TOP 5
    NAME,
    geom.STArea() AS Area_degrees,
    geom.STLength() AS Perimeter_degrees
FROM dbo.countries
ORDER BY geom.STArea() DESC;


-- 10.8 Нахождение расстояния между пространственными объектами STDistance(другая_геометрия) 
DECLARE @moscow geometry;
DECLARE @paris geometry;

SELECT @moscow = geom FROM dbo.places WHERE name = 'Moscow';
SELECT @paris = geom FROM dbo.places WHERE name = 'Paris';

SELECT @moscow.STDistance(@paris) AS Distance_degrees;


-- 11	Создайте пространственный объект в виде точки (1) /линии (2) /полигона (3).
CREATE TABLE dbo.my_objects (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100),
    description NVARCHAR(255),
    geom geometry
);

SELECT * FROM dbo.my_objects;
SELECT id, name, description,
    geom.STAsText() AS WKT,
    geom.STGeometryType() AS Type,
    geom.STX AS Longitude,
    geom.STY AS Latitude
FROM dbo.my_objects;

-- 11.1
DECLARE @myPoint geometry;
SET @myPoint = geometry::STGeomFromText('POINT (23.8286 53.6775)', 4326);


INSERT INTO dbo.my_objects (name, description, geom)
VALUES (
    N'Grodno', 
    N'Гродно, Беларусь — точка',
    geometry::STGeomFromText('POINT (23.8286 53.6775)', 4326)
);

-- 11.2
INSERT INTO dbo.my_objects (name, description, geom)
VALUES (
    N'Moscow-SPb Route', 
    N'Маршрут Москва — Санкт-Петербург — линия',
    geometry::STGeomFromText(
        'LINESTRING (37.6 55.75, 35.9 56.86, 31.28 58.52, 30.32 59.93)', 
        4326
    )
);

-- 11.3
INSERT INTO dbo.my_objects (name, description, geom)
VALUES (
    N'Minsk Area', 
    N'Условная область вокруг Минска — полигон',
    geometry::STGeomFromText(
        'POLYGON ((27.3 53.7, 27.9 53.7, 27.9 54.0, 27.3 54.0, 27.3 53.7))', 
        4326
    )
);


-- 12	Найдите, в какие пространственные объекты попадают созданные вами объекты.
-- 12.1
DECLARE @myPoint geometry;
SELECT @myPoint = geom FROM dbo.my_objects WHERE name = 'Grodno';

SELECT 
    c.NAME AS CountryName
FROM dbo.countries c
WHERE c.geom.STContains(@myPoint) = 1;

-- 12.2
DECLARE @myLine geometry;
SELECT @myLine = geom FROM dbo.my_objects WHERE name = 'Moscow-SPb Route';

SELECT 
    c.NAME AS CountryName
FROM dbo.countries c
WHERE c.geom.STIntersects(@myLine) = 1;

-- 12.3
DECLARE @myPoly geometry;
SELECT @myPoly = geom FROM dbo.my_objects WHERE name = 'Minsk Area';

SELECT 
    p.name AS CityName,
    p.geom.STAsText() AS Location
FROM dbo.places p
WHERE @myPoly.STContains(p.geom) = 1;
