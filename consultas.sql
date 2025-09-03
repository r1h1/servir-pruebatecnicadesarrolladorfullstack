-- (a) El porcentaje de ejecución de fondos para cada proyecto registrado. La ejecución se
-- calcula como el monto total ejecutado (gastado) dentro del total de fondos recibidos por
-- medio de donaciones.

SELECT 
    P.Nombre AS Proyecto, 
    SUM(O.Monto) AS Ejecutado, 
    D.MontoTotal AS TotalRecibido,
    (SUM(O.Monto) / D.MontoTotal) * 100 AS PorcentajeEjecucion
FROM Proyectos P
JOIN Donaciones D ON D.IdProyecto = P.Id
JOIN OrdenesCompra O ON O.IdRubro = D.IdRubro
GROUP BY P.Nombre, D.MontoTotal;



--- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---


-- (b) La disponibilidad de fondos en cada rubro del proyecto “X”, de modo que se muestren
-- todos los rubros del proyecto (incluyendo los que pueden no tener ninguna donación
-- recibida o ninguna orden de compra emitida)

SELECT 
    R.Nombre AS Rubro, 
    COALESCE(SUM(D.Monto), 0) AS DonacionesRecibidas,
    COALESCE(SUM(O.Monto), 0) AS GastosRealizados,
    (COALESCE(SUM(D.Monto), 0) - COALESCE(SUM(O.Monto), 0)) AS Disponibilidad
FROM Rubros R
LEFT JOIN Donaciones D ON D.IdRubro = R.Id
LEFT JOIN OrdenesCompra O ON O.IdRubro = R.Id
WHERE R.IdProyecto = 'X'  -- Proyecto específico
GROUP BY R.Nombre;