-- Consulta para obtener el porcentaje de ejecución de fondos por proyecto
SELECT
    p.Codigo AS CodigoProyecto,
    p.Nombre AS NombreProyecto,
    ISNULL(SUM(oc.Monto), 0) AS TotalComprado,
    ISNULL(SUM(d.Monto), 0) AS TotalDonado,
    CASE
        WHEN ISNULL(SUM(d.Monto), 0) = 0 THEN 0
        ELSE (ISNULL(SUM(oc.Monto), 0) / ISNULL(SUM(d.Monto), 0)) * 100
        END AS PorcentajeEjecucion
FROM Proyectos p
         INNER JOIN Rubros r ON p.Id = r.IdProyecto AND r.Active = 1
         LEFT JOIN OrdenesCompra oc ON r.Id = oc.IdRubro AND oc.Active = 1
         LEFT JOIN Donaciones d ON r.Id = d.IdRubro AND d.Active = 1
WHERE p.Active = 1
GROUP BY p.Codigo, p.Nombre
ORDER BY p.Nombre;


-- Consulta para obtener la disponibilidad de fondos en cada rubro del proyecto "X"
SELECT
    r.Codigo AS CodigoRubro,
    r.Nombre AS NombreRubro,
    ISNULL(SUM(d.Monto), 0) AS TotalDonado,
    ISNULL(SUM(oc.Monto), 0) AS TotalComprado,
    ISNULL(SUM(d.Monto), 0) - ISNULL(SUM(oc.Monto), 0) AS DisponibilidadDeFondos
FROM Rubros r
         INNER JOIN Proyectos p ON r.IdProyecto = p.Id
         LEFT JOIN Donaciones d ON r.Id = d.IdRubro AND d.Active = 1
         LEFT JOIN OrdenesCompra oc ON r.Id = oc.IdRubro AND oc.Active = 1
WHERE p.Codigo = 'P-0005'
  AND p.Active = 1
GROUP BY r.Codigo, r.Nombre
ORDER BY r.Nombre;