-- TABLA PROYECTOS
CREATE TABLE Proyectos
(
    Id           INT IDENTITY (1,1) PRIMARY KEY, -- Incrementa automáticamente
    Codigo       VARCHAR(10) UNIQUE,             -- Correlativo (P-0001, P-0002, ...)
    Nombre       VARCHAR(255) NOT NULL,
    Municipio    VARCHAR(255) NOT NULL,
    Departamento VARCHAR(255) NOT NULL,
    FechaInicio  DATETIME     NOT NULL,
    FechaFin     DATETIME     NOT NULL,
    Active       BIT          NOT NULL
);

-- SPS PROYECTOS
-- OBTENER TODOS LOS PROYECTOS ACTIVOS
CREATE OR ALTER PROCEDURE sp_listarProyectos @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Proyectos WHERE Active = @Active;
END

-- OBTENER PROYECTOS POR CODIGO
CREATE OR ALTER PROCEDURE sp_listarProyectosPorCodigo 
    @Codigo VARCHAR(10),
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Proyectos
    WHERE Codigo = @Codigo
    AND Active = @Active;
END

-- OBTENER ULTIMO CODIGO GENERADO
CREATE OR ALTER PROCEDURE sp_listarUltimoCodigoGenerado
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 Codigo
    FROM Proyectos
    WHERE Active = 1
    ORDER BY Id DESC;
END

-- PROCEDIMIENTO MEJORADO PARA ELIMINAR PROYECTO Y SUS RUBROS ASOCIADOS
CREATE OR ALTER PROCEDURE sp_eliminarProyectoCompleto
    @Codigo VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdProyecto INT;
    DECLARE @Mensaje NVARCHAR(500);
    DECLARE @CantidadRubros INT;
    
    IF NOT EXISTS(SELECT 1 FROM Proyectos WHERE Codigo = @Codigo)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, 
               N'Proyecto no existe' AS [Message];
        RETURN;
    END;
    
    SELECT @IdProyecto = Id FROM Proyectos WHERE Codigo = @Codigo;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- 1. Inactivar todos los rubros asociados al proyecto
        UPDATE Rubros
        SET Active = 0
        WHERE IdProyecto = @IdProyecto;
        
        SET @CantidadRubros = @@ROWCOUNT;
        
        -- 2. Inactivar el proyecto
        UPDATE Proyectos
        SET Active = 0
        WHERE Codigo = @Codigo;
        
        COMMIT TRANSACTION;
        
        -- Mensaje de éxito
        SET @Mensaje = CONCAT(
            N'Proyecto eliminado exitosamente. ',
            'Se inactivaron ', CAST(@CantidadRubros AS VARCHAR), 
            ' rubros asociados.'
        );
        
        SELECT CAST(1 AS BIT) AS Success, 
               @Mensaje AS [Message];
               
    END TRY
    BEGIN CATCH
        -- Revertir la transacción en caso de error
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Mensaje de error detallado
        SET @Mensaje = CONCAT(
            N'ERROR: No se pudo completar la eliminación del proyecto y sus rubros asociados. ',
            ERROR_MESSAGE()
        );
        
        SELECT CAST(0 AS BIT) AS Success, 
               @Mensaje AS [Message];
    END CATCH
END
GO


-- ACTUALIZAR PROYECTO
CREATE OR ALTER PROCEDURE sp_editarProyecto
    @Codigo VARCHAR(10),
    @Nombre VARCHAR(255),
    @Municipio VARCHAR(255),
    @Departamento VARCHAR(255),
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS(SELECT 1 FROM Proyectos WHERE Codigo = @Codigo)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Proyecto no existe' AS [Message]; 
        RETURN;
    END;

    BEGIN TRY
        UPDATE Proyectos
        SET Nombre = @Nombre,
            Municipio = @Municipio,
            Departamento = @Departamento,
            FechaInicio = @FechaInicio,
            FechaFin = @FechaFin
        WHERE Codigo = @Codigo;
        
        -- Devolver todos los datos actualizados
        SELECT CAST(1 AS BIT) AS Success, 
               N'Proyecto editado' AS [Message],
               Id,
               Codigo,
               Nombre,
               Municipio,
               Departamento,
               FechaInicio,
               FechaFin
        FROM Proyectos 
        WHERE Codigo = @Codigo;
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, 
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END

-- GENERAR EL CODIGO DEL PROYECTO AUTOMATICAMENTE
CREATE OR ALTER PROCEDURE sp_GenerarCodigoProyecto
    @NuevoCodigo VARCHAR(10) OUTPUT
AS
BEGIN
    DECLARE @UltimoCodigo INT;

    SELECT @UltimoCodigo = MAX(CAST(SUBSTRING(Codigo, 3, LEN(Codigo)) AS INT))
    FROM Proyectos;

    IF @UltimoCodigo IS NULL
        SET @UltimoCodigo = 0;

    SET @UltimoCodigo = @UltimoCodigo + 1;
    SET @NuevoCodigo = 'P-' + RIGHT('0000' + CAST(@UltimoCodigo AS VARCHAR), 4);
END


-- CREAR PROYECTO
CREATE OR ALTER PROCEDURE sp_crearProyecto
    @Nombre VARCHAR(255),
    @Municipio VARCHAR(255),
    @Departamento VARCHAR(255),
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Codigo VARCHAR(10);

    -- Obtener el último código
    DECLARE @UltimoCodigo INT;
    SELECT @UltimoCodigo = MAX(CAST(SUBSTRING(Codigo, 3, LEN(Codigo)) AS INT))
    FROM Proyectos;

    IF @UltimoCodigo IS NULL
        SET @UltimoCodigo = 0;

    SET @UltimoCodigo = @UltimoCodigo + 1;
    SET @Codigo = 'P-' + RIGHT('0000' + CAST(@UltimoCodigo AS VARCHAR), 4);

    IF EXISTS(SELECT 1 FROM Proyectos WHERE Codigo = @Codigo)
        BEGIN
            SELECT CAST(0 AS BIT) AS Success,
                   N'El código del proyecto ya existe, no se puede repetir' AS [Message];
            RETURN;
        END

    BEGIN TRY
        INSERT INTO Proyectos (Codigo, Nombre, Municipio, Departamento, FechaInicio, FechaFin, Active)
        VALUES (@Codigo, @Nombre, @Municipio, @Departamento, @FechaInicio, @FechaFin, 1);

        -- Devolver todos los datos del proyecto creado
        SELECT CAST(1 AS BIT) AS Success,
               N'Proyecto creado' AS [Message],
               Id,
               Codigo,
               Nombre,
               Municipio,
               Departamento,
               FechaInicio,
               FechaFin
        FROM Proyectos
        WHERE Codigo = @Codigo;
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success,
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END





-- TABLA RUBROS
CREATE TABLE Rubros
(
    Id         INT IDENTITY (1,1) PRIMARY KEY, -- Incrementa automáticamente
    Codigo     VARCHAR(50) UNIQUE NOT NULL,    -- Código único por proyecto
    Nombre     VARCHAR(255)       NOT NULL,
    IdProyecto INT                NOT NULL,    -- FK a Proyectos
    Active     BIT                NOT NULL
    FOREIGN KEY (IdProyecto) REFERENCES Proyectos (Id) ON DELETE CASCADE
);

-- SPS Rubros 
-- OBTENER TODOS LOS RUBROS ACTIVOS (SOLO SI EL PROYECTO TAMBIÉN ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarRubros 
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT r.* 
    FROM Rubros r
    INNER JOIN Proyectos p ON r.IdProyecto = p.Id
    WHERE r.Active = @Active AND p.Active = 1;
END
GO

-- OBTENER RUBROS POR ID DE PROYECTO (SOLO SI EL PROYECTO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarRubrosPorProyecto 
    @IdProyecto INT,
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el proyecto existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Proyectos WHERE Id = @IdProyecto AND Active = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'El proyecto especificado no existe o está inactivo' AS [Message];
        RETURN;
    END;

    SELECT r.*
    FROM Rubros r
    WHERE r.IdProyecto = @IdProyecto
    AND r.Active = @Active;
END
GO

-- OBTENER RUBRO POR CÓDIGO (SOLO SI EL PROYECTO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarRubroPorCodigo 
    @Codigo VARCHAR(50),
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.*
    FROM Rubros r
    INNER JOIN Proyectos p ON r.IdProyecto = p.Id
    WHERE r.Codigo = @Codigo
    AND r.Active = @Active
    AND p.Active = 1;
END
GO

-- ELIMINAR RUBRO (BORRADO LÓGICO) - VERIFICAR QUE EL PROYECTO ESTÉ ACTIVO
CREATE OR ALTER PROCEDURE sp_eliminarRubro
    @Codigo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si el rubro existe y el proyecto está activo
    IF NOT EXISTS(
        SELECT 1 
        FROM Rubros r
        INNER JOIN Proyectos p ON r.IdProyecto = p.Id
        WHERE r.Codigo = @Codigo AND p.Active = 1
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Rubro no existe o el proyecto asociado está inactivo' AS [Message]; 
        RETURN;
    END;

    BEGIN TRY
        UPDATE Rubros
        SET Active = 0
        WHERE Codigo = @Codigo;

        SELECT CAST(1 AS BIT) AS Success, N'Rubro eliminado' AS [Message];
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- ACTUALIZAR RUBRO (SOLO SI EL PROYECTO ORIGINAL Y EL NUEVO ESTÁN ACTIVOS)
CREATE OR ALTER PROCEDURE sp_editarRubro
    @Codigo VARCHAR(50),
    @Nombre VARCHAR(255),
    @IdProyecto INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el rubro existe y el proyecto original está activo
    IF NOT EXISTS(
        SELECT 1 
        FROM Rubros r
        INNER JOIN Proyectos p ON r.IdProyecto = p.Id
        WHERE r.Codigo = @Codigo AND p.Active = 1
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Rubro no existe o el proyecto asociado está inactivo' AS [Message]; 
        RETURN;
    END;

    -- Verificar si el nuevo proyecto existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Proyectos WHERE Id = @IdProyecto AND Active = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'El proyecto especificado no existe o está inactivo' AS [Message]; 
        RETURN;
    END;

    BEGIN TRY
        UPDATE Rubros
        SET Nombre = @Nombre,
            IdProyecto = @IdProyecto
        WHERE Codigo = @Codigo;
        
        -- Devolver todos los datos actualizados
        SELECT CAST(1 AS BIT) AS Success, 
               N'Rubro editado' AS [Message],
               r.Id,
               r.Codigo,
               r.Nombre,
               r.IdProyecto,
               r.Active
        FROM Rubros r
        WHERE r.Codigo = @Codigo;
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, 
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- CREAR RUBRO (SOLO SI EL PROYECTO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_crearRubro
    @Nombre VARCHAR(255),
    @IdProyecto INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el proyecto existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Proyectos WHERE Id = @IdProyecto AND Active = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'El proyecto especificado no existe o está inactivo' AS [Message];
        RETURN;
    END

    -- Generar código único para el rubro (usando ID del proyecto + secuencia)
    DECLARE @Codigo VARCHAR(50);
    DECLARE @UltimoCodigo INT;
    
    SELECT @UltimoCodigo = MAX(CAST(SUBSTRING(Codigo, CHARINDEX('-', Codigo) + 1, LEN(Codigo)) AS INT))
    FROM Rubros
    WHERE IdProyecto = @IdProyecto AND Codigo LIKE 'R-' + CAST(@IdProyecto AS VARCHAR) + '-%';
    
    IF @UltimoCodigo IS NULL
        SET @UltimoCodigo = 0;

    SET @UltimoCodigo = @UltimoCodigo + 1;
    SET @Codigo = 'R-' + CAST(@IdProyecto AS VARCHAR) + '-' + RIGHT('000' + CAST(@UltimoCodigo AS VARCHAR), 3);

    -- Verificar si el código ya existe
    IF EXISTS(SELECT 1 FROM Rubros WHERE Codigo = @Codigo)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success,
               N'El código del rubro ya existe, no se puede repetir' AS [Message];
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Rubros (Codigo, Nombre, IdProyecto, Active)
        VALUES (@Codigo, @Nombre, @IdProyecto, 1);

        -- Devolver todos los datos del rubro creado
        SELECT CAST(1 AS BIT) AS Success,
               N'Rubro creado' AS [Message],
               Id,
               Codigo,
               Nombre,
               IdProyecto,
               Active
        FROM Rubros
        WHERE Codigo = @Codigo;
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success,
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- PROCEDIMIENTO PARA ELIMINAR RUBROS CUANDO SE INACTIVA UN PROYECTO
CREATE OR ALTER PROCEDURE sp_inactivarRubrosPorProyecto
    @IdProyecto INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE Rubros
        SET Active = 0
        WHERE IdProyecto = @IdProyecto;
        
        SELECT CAST(1 AS BIT) AS Success, 
               CONCAT('Se inactivaron ', CAST(@@ROWCOUNT AS VARCHAR), ' rubros') AS [Message];
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, 
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- OBTENER RUBROS CON DETALLES COMPLETOS (SOLO SI EL PROYECTO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarRubrosCompletos
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.Id,
        r.Codigo,
        r.Nombre,
        r.IdProyecto,
        r.Active,
        p.Codigo AS CodigoProyecto,
        p.Nombre AS NombreProyecto,
        p.Municipio,
        p.Departamento,
        p.FechaInicio,
        p.FechaFin
    FROM Rubros r
    INNER JOIN Proyectos p ON r.IdProyecto = p.Id AND p.Active = 1
    WHERE r.Active = @Active
    ORDER BY p.Nombre, r.Nombre;
END
GO






-- TABLA DONACIONES
CREATE TABLE Donaciones
(
    Id            INT IDENTITY (1,1) PRIMARY KEY, -- Incrementa automáticamente
    IdRubro       INT            NOT NULL,        -- FK a Rubros
    Monto         DECIMAL(18, 2) NOT NULL,        -- Monto donado
    FechaDonacion DATETIME       NOT NULL,        -- Fecha de la donación
    NombreDonante VARCHAR(255)   NOT NULL,        -- Nombre del donante
    Active        BIT            NOT NULL
    FOREIGN KEY (IdRubro) REFERENCES Rubros (Id) ON DELETE CASCADE
);

-- OBTENER TODAS LAS DONACIONES ACTIVAS (SOLO SI EL RUBRO TAMBIÉN ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarDonaciones 
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.* 
    FROM Donaciones d
    INNER JOIN Rubros r ON d.IdRubro = r.Id
    WHERE d.Active = @Active AND r.Active = 1;
END
GO

-- OBTENER DONACIONES POR ID DE RUBRO (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarDonacionesPorRubro 
    @IdRubro INT,
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el rubro existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Rubros WHERE Id = @IdRubro AND Active = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'El rubro especificado no existe o está inactivo' AS [Message];
        RETURN;
    END;

    SELECT d.*
    FROM Donaciones d
    WHERE d.IdRubro = @IdRubro
    AND d.Active = @Active;
END
GO

-- OBTENER DONACIÓN POR ID (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarDonacionPorId 
    @Id INT,
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT d.*
    FROM Donaciones d
    INNER JOIN Rubros r ON d.IdRubro = r.Id
    WHERE d.Id = @Id
    AND d.Active = @Active
    AND r.Active = 1;
END
GO

-- ELIMINAR DONACIÓN (BORRADO LÓGICO) - VERIFICAR QUE EL RUBRO ESTÉ ACTIVO
CREATE OR ALTER PROCEDURE sp_eliminarDonacion
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si la donación existe y el rubro está activo
    IF NOT EXISTS(
        SELECT 1 
        FROM Donaciones d
        INNER JOIN Rubros r ON d.IdRubro = r.Id
        WHERE d.Id = @Id AND r.Active = 1
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Donación no existe o el rubro asociado está inactivo' AS [Message]; 
        RETURN;
    END;

    BEGIN TRY
        UPDATE Donaciones
        SET Active = 0
        WHERE Id = @Id;

        SELECT CAST(1 AS BIT) AS Success, N'Donación eliminada' AS [Message];
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- ACTUALIZAR DONACIÓN (SOLO SI EL RUBRO ORIGINAL Y EL NUEVO ESTÁN ACTIVOS)
CREATE OR ALTER PROCEDURE sp_editarDonacion
    @Id INT,
    @IdRubro INT,
    @Monto DECIMAL(18, 2),
    @FechaDonacion DATETIME,
    @NombreDonante VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si la donación existe y el rubro original está activo
    IF NOT EXISTS(
        SELECT 1 
        FROM Donaciones d
        INNER JOIN Rubros r ON d.IdRubro = r.Id
        WHERE d.Id = @Id AND r.Active = 1
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Donación no existe o el rubro asociado está inactivo' AS [Message]; 
        RETURN;
    END;

    -- Verificar si el nuevo rubro existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Rubros WHERE Id = @IdRubro AND Active = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'El rubro especificado no existe o está inactivo' AS [Message]; 
        RETURN;
    END;

    BEGIN TRY
        UPDATE Donaciones
        SET IdRubro = @IdRubro,
            Monto = @Monto,
            FechaDonacion = @FechaDonacion,
            NombreDonante = @NombreDonante
        WHERE Id = @Id;
        
        -- Devolver todos los datos actualizados
        SELECT CAST(1 AS BIT) AS Success, 
               N'Donación editada' AS [Message],
               d.Id,
               d.IdRubro,
               d.Monto,
               d.FechaDonacion,
               d.NombreDonante,
               d.Active
        FROM Donaciones d
        WHERE d.Id = @Id;
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, 
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- CREAR DONACIÓN (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_crearDonacion
    @IdRubro INT,
    @Monto DECIMAL(18, 2),
    @FechaDonacion DATETIME,
    @NombreDonante VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el rubro existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Rubros WHERE Id = @IdRubro AND Active = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'El rubro especificado no existe o está inactivo' AS [Message];
        RETURN;
    END

    BEGIN TRY
        INSERT INTO Donaciones (IdRubro, Monto, FechaDonacion, NombreDonante, Active)
        VALUES (@IdRubro, @Monto, @FechaDonacion, @NombreDonante, 1);

        -- Devolver todos los datos de la donación creada
        SELECT CAST(1 AS BIT) AS Success,
               N'Donación creada' AS [Message],
               Id,
               IdRubro,
               Monto,
               FechaDonacion,
               NombreDonante,
               Active
        FROM Donaciones
        WHERE Id = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success,
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- OBTENER TOTAL DE DONACIONES POR RUBRO (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_obtenerTotalDonacionesPorRubro
    @IdRubro INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si el rubro existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Rubros WHERE Id = @IdRubro AND Active = 1)
    BEGIN
        SELECT 
            0 AS TotalDonaciones,
            0 AS CantidadDonaciones,
            CAST(0 AS BIT) AS RubroActivo;
        RETURN;
    END;
    
    SELECT 
        ISNULL(SUM(Monto), 0) AS TotalDonaciones,
        COUNT(*) AS CantidadDonaciones,
        CAST(1 AS BIT) AS RubroActivo
    FROM Donaciones 
    WHERE IdRubro = @IdRubro AND Active = 1;
END
GO

-- OBTENER DONACIONES POR DONANTE (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarDonacionesPorDonante
    @NombreDonante VARCHAR(255),
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT d.*
    FROM Donaciones d
    INNER JOIN Rubros r ON d.IdRubro = r.Id
    WHERE d.NombreDonante LIKE '%' + @NombreDonante + '%'
    AND d.Active = @Active
    AND r.Active = 1
    ORDER BY d.FechaDonacion DESC;
END
GO

-- OBTENER DONACIONES POR RANGO DE FECHAS (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarDonacionesPorRangoFechas
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT d.*
    FROM Donaciones d
    INNER JOIN Rubros r ON d.IdRubro = r.Id
    WHERE d.FechaDonacion BETWEEN @FechaInicio AND @FechaFin
    AND d.Active = @Active
    AND r.Active = 1
    ORDER BY d.FechaDonacion DESC;
END
GO

-- OBTENER REPORTE DE DONACIONES POR PROYECTO (SOLO SI PROYECTO Y RUBRO ESTÁN ACTIVOS)
CREATE OR ALTER PROCEDURE sp_obtenerReporteDonacionesPorProyecto
    @IdProyecto INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si el proyecto existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Proyectos WHERE Id = @IdProyecto AND Active = 1)
    BEGIN
        SELECT 
            '' AS NombreProyecto,
            '' AS CodigoRubro,
            '' AS NombreRubro,
            0 AS CantidadDonaciones,
            0 AS TotalDonado,
            CAST(0 AS BIT) AS ProyectoActivo;
        RETURN;
    END;
    
    SELECT 
        p.Nombre AS NombreProyecto,
        r.Codigo AS CodigoRubro,
        r.Nombre AS NombreRubro,
        COUNT(d.Id) AS CantidadDonaciones,
        ISNULL(SUM(d.Monto), 0) AS TotalDonado,
        CAST(1 AS BIT) AS ProyectoActivo
    FROM Proyectos p
    INNER JOIN Rubros r ON p.Id = r.IdProyecto AND r.Active = 1
    LEFT JOIN Donaciones d ON r.Id = d.IdRubro AND d.Active = 1
    WHERE p.Id = @IdProyecto 
    AND p.Active = 1 
    GROUP BY p.Nombre, r.Codigo, r.Nombre
    ORDER BY TotalDonado DESC;
END
GO

-- OBTENER TOP DONANTES (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_obtenerTopDonantes
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@Top)
        d.NombreDonante,
        COUNT(*) AS CantidadDonaciones,
        SUM(d.Monto) AS TotalDonado
    FROM Donaciones d
    INNER JOIN Rubros r ON d.IdRubro = r.Id
    WHERE d.Active = 1
    AND r.Active = 1
    GROUP BY d.NombreDonante
    ORDER BY TotalDonado DESC;
END
GO

-- OBTENER DONACIONES CON DETALLES COMPLETOS (SOLO SI PROYECTO Y RUBRO ESTÁN ACTIVOS)
CREATE OR ALTER PROCEDURE sp_listarDonacionesCompletas
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.Id,
        d.Monto,
        d.FechaDonacion,
        d.NombreDonante,
        d.Active,
        r.Codigo AS CodigoRubro,
        r.Nombre AS NombreRubro,
        p.Codigo AS CodigoProyecto,
        p.Nombre AS NombreProyecto
    FROM Donaciones d
    INNER JOIN Rubros r ON d.IdRubro = r.Id AND r.Active = 1
    INNER JOIN Proyectos p ON r.IdProyecto = p.Id AND p.Active = 1
    WHERE d.Active = @Active
    ORDER BY d.FechaDonacion DESC;
END
GO

-- PROCEDIMIENTO PARA ELIMINAR DONACIONES CUANDO SE INACTIVA UN RUBRO
CREATE OR ALTER PROCEDURE sp_inactivarDonacionesPorRubro
    @IdRubro INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE Donaciones
        SET Active = 0
        WHERE IdRubro = @IdRubro;
        
        SELECT CAST(1 AS BIT) AS Success, 
               CONCAT('Se inactivaron ', CAST(@@ROWCOUNT AS VARCHAR), ' donaciones') AS [Message];
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, 
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO





-- TABLA ORDENES DE COMPRA
CREATE TABLE OrdenesCompra
(
    Id         INT IDENTITY (1,1) PRIMARY KEY, -- Incrementa automáticamente
    IdRubro    INT            NOT NULL,        -- FK a Rubros
    Monto      DECIMAL(18, 2) NOT NULL,        -- Monto de la orden de compra
    FechaOrden DATETIME       NOT NULL,        -- Fecha de la orden de compra
    Active     BIT            NOT NULL
    FOREIGN KEY (IdRubro) REFERENCES Rubros (Id) ON DELETE CASCADE
);

-- SPS ORDENES DE COMPRA
-- OBTENER TODAS LAS ÓRDENES DE COMPRA ACTIVAS (SOLO SI EL RUBRO TAMBIÉN ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarOrdenesCompra 
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT oc.* 
    FROM OrdenesCompra oc
    INNER JOIN Rubros r ON oc.IdRubro = r.Id
    WHERE oc.Active = @Active AND r.Active = 1;
END
GO

-- OBTENER ÓRDENES DE COMPRA POR ID DE RUBRO (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarOrdenesCompraPorRubro 
    @IdRubro INT,
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el rubro existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Rubros WHERE Id = @IdRubro AND Active = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'El rubro especificado no existe o está inactivo' AS [Message];
        RETURN;
    END;

    SELECT oc.*
    FROM OrdenesCompra oc
    WHERE oc.IdRubro = @IdRubro
    AND oc.Active = @Active;
END
GO

-- OBTENER ORDEN DE COMPRA POR ID (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarOrdenCompraPorId 
    @Id INT,
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT oc.*
    FROM OrdenesCompra oc
    INNER JOIN Rubros r ON oc.IdRubro = r.Id
    WHERE oc.Id = @Id
    AND oc.Active = @Active
    AND r.Active = 1;
END
GO

-- ELIMINAR ORDEN DE COMPRA (BORRADO LÓGICO) - VERIFICAR QUE EL RUBRO ESTÉ ACTIVO
CREATE OR ALTER PROCEDURE sp_eliminarOrdenCompra
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si la orden de compra existe y el rubro está activo
    IF NOT EXISTS(
        SELECT 1 
        FROM OrdenesCompra oc
        INNER JOIN Rubros r ON oc.IdRubro = r.Id
        WHERE oc.Id = @Id AND r.Active = 1
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Orden de compra no existe o el rubro asociado está inactivo' AS [Message]; 
        RETURN;
    END;

    BEGIN TRY
        UPDATE OrdenesCompra
        SET Active = 0
        WHERE Id = @Id;

        SELECT CAST(1 AS BIT) AS Success, N'Orden de compra eliminada' AS [Message];
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- ACTUALIZAR ORDEN DE COMPRA (SOLO SI EL RUBRO ORIGINAL Y EL NUEVO ESTÁN ACTIVOS)
CREATE OR ALTER PROCEDURE sp_editarOrdenCompra
    @Id INT,
    @IdRubro INT,
    @Monto DECIMAL(18, 2),
    @FechaOrden DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si la orden de compra existe y el rubro original está activo
    IF NOT EXISTS(
        SELECT 1 
        FROM OrdenesCompra oc
        INNER JOIN Rubros r ON oc.IdRubro = r.Id
        WHERE oc.Id = @Id AND r.Active = 1
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Orden de compra no existe o el rubro asociado está inactivo' AS [Message]; 
        RETURN;
    END;

    -- Verificar si el nuevo rubro existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Rubros WHERE Id = @IdRubro AND Active = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'El rubro especificado no existe o está inactivo' AS [Message]; 
        RETURN;
    END;

    BEGIN TRY
        UPDATE OrdenesCompra
        SET IdRubro = @IdRubro,
            Monto = @Monto,
            FechaOrden = @FechaOrden
        WHERE Id = @Id;
        
        -- Devolver todos los datos actualizados
        SELECT CAST(1 AS BIT) AS Success, 
               N'Orden de compra editada' AS [Message],
               oc.Id,
               oc.IdRubro,
               oc.Monto,
               oc.FechaOrden,
               oc.Active
        FROM OrdenesCompra oc
        WHERE oc.Id = @Id;
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, 
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- CREAR ORDEN DE COMPRA (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_crearOrdenCompra
    @IdRubro INT,
    @Monto DECIMAL(18, 2),
    @FechaOrden DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el rubro existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Rubros WHERE Id = @IdRubro AND Active = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'El rubro especificado no existe o está inactivo' AS [Message];
        RETURN;
    END

    BEGIN TRY
        INSERT INTO OrdenesCompra (IdRubro, Monto, FechaOrden, Active)
        VALUES (@IdRubro, @Monto, @FechaOrden, 1);

        -- Devolver todos los datos de la orden de compra creada
        SELECT CAST(1 AS BIT) AS Success,
               N'Orden de compra creada' AS [Message],
               Id,
               IdRubro,
               Monto,
               FechaOrden,
               Active
        FROM OrdenesCompra
        WHERE Id = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success,
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- OBTENER TOTAL DE ÓRDENES DE COMPRA POR RUBRO (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_obtenerTotalOrdenesCompraPorRubro
    @IdRubro INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si el rubro existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Rubros WHERE Id = @IdRubro AND Active = 1)
    BEGIN
        SELECT 
            0 AS TotalOrdenesCompra,
            0 AS CantidadOrdenesCompra,
            CAST(0 AS BIT) AS RubroActivo;
        RETURN;
    END;
    
    SELECT 
        ISNULL(SUM(Monto), 0) AS TotalOrdenesCompra,
        COUNT(*) AS CantidadOrdenesCompra,
        CAST(1 AS BIT) AS RubroActivo
    FROM OrdenesCompra 
    WHERE IdRubro = @IdRubro AND Active = 1;
END
GO

-- OBTENER ÓRDENES DE COMPRA POR RANGO DE FECHAS (SOLO SI EL RUBRO ESTÁ ACTIVO)
CREATE OR ALTER PROCEDURE sp_listarOrdenesCompraPorRangoFechas
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT oc.*
    FROM OrdenesCompra oc
    INNER JOIN Rubros r ON oc.IdRubro = r.Id
    WHERE oc.FechaOrden BETWEEN @FechaInicio AND @FechaFin
    AND oc.Active = @Active
    AND r.Active = 1
    ORDER BY oc.FechaOrden DESC;
END
GO

-- OBTENER REPORTE DE ÓRDENES DE COMPRA POR PROYECTO (SOLO SI PROYECTO Y RUBRO ESTÁN ACTIVOS)
CREATE OR ALTER PROCEDURE sp_obtenerReporteOrdenesCompraPorProyecto
    @IdProyecto INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si el proyecto existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Proyectos WHERE Id = @IdProyecto AND Active = 1)
    BEGIN
        SELECT 
            '' AS NombreProyecto,
            '' AS CodigoRubro,
            '' AS NombreRubro,
            0 AS CantidadOrdenesCompra,
            0 AS TotalOrdenesCompra,
            CAST(0 AS BIT) AS ProyectoActivo;
        RETURN;
    END;
    
    SELECT 
        p.Nombre AS NombreProyecto,
        r.Codigo AS CodigoRubro,
        r.Nombre AS NombreRubro,
        COUNT(oc.Id) AS CantidadOrdenesCompra,
        ISNULL(SUM(oc.Monto), 0) AS TotalOrdenesCompra,
        CAST(1 AS BIT) AS ProyectoActivo
    FROM Proyectos p
    INNER JOIN Rubros r ON p.Id = r.IdProyecto AND r.Active = 1
    LEFT JOIN OrdenesCompra oc ON r.Id = oc.IdRubro AND oc.Active = 1
    WHERE p.Id = @IdProyecto 
    AND p.Active = 1 
    GROUP BY p.Nombre, r.Codigo, r.Nombre
    ORDER BY TotalOrdenesCompra DESC;
END
GO

-- OBTENER ÓRDENES DE COMPRA CON DETALLES COMPLETOS (SOLO SI PROYECTO Y RUBRO ESTÁN ACTIVOS)
CREATE OR ALTER PROCEDURE sp_listarOrdenesCompraCompletas
    @Active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        oc.Id,
        oc.Monto,
        oc.FechaOrden,
        oc.Active,
        r.Codigo AS CodigoRubro,
        r.Nombre AS NombreRubro,
        p.Codigo AS CodigoProyecto,
        p.Nombre AS NombreProyecto
    FROM OrdenesCompra oc
    INNER JOIN Rubros r ON oc.IdRubro = r.Id AND r.Active = 1
    INNER JOIN Proyectos p ON r.IdProyecto = p.Id AND p.Active = 1
    WHERE oc.Active = @Active
    ORDER BY oc.FechaOrden DESC;
END
GO

-- PROCEDIMIENTO PARA ELIMINAR ÓRDENES DE COMPRA CUANDO SE INACTIVA UN RUBRO
CREATE OR ALTER PROCEDURE sp_inactivarOrdenesCompraPorRubro
    @IdRubro INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE OrdenesCompra
        SET Active = 0
        WHERE IdRubro = @IdRubro;
        
        SELECT CAST(1 AS BIT) AS Success, 
               CONCAT('Se inactivaron ', CAST(@@ROWCOUNT AS VARCHAR), ' órdenes de compra') AS [Message];
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, 
               CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END
GO

-- OBTENER BALANCE DE RUBRO (DONACIONES - ÓRDENES DE COMPRA)
CREATE OR ALTER PROCEDURE sp_obtenerBalanceRubro
    @IdRubro INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si el rubro existe y está activo
    IF NOT EXISTS(SELECT 1 FROM Rubros WHERE Id = @IdRubro AND Active = 1)
    BEGIN
        SELECT 
            0 AS TotalDonaciones,
            0 AS TotalOrdenesCompra,
            0 AS Balance,
            CAST(0 AS BIT) AS RubroActivo;
        RETURN;
    END;
    
    DECLARE @TotalDonaciones DECIMAL(18,2);
    DECLARE @TotalOrdenesCompra DECIMAL(18,2);
    DECLARE @Balance DECIMAL(18,2);
    
    SELECT @TotalDonaciones = ISNULL(SUM(Monto), 0)
    FROM Donaciones 
    WHERE IdRubro = @IdRubro AND Active = 1;
    
    SELECT @TotalOrdenesCompra = ISNULL(SUM(Monto), 0)
    FROM OrdenesCompra 
    WHERE IdRubro = @IdRubro AND Active = 1;
    
    SET @Balance = @TotalDonaciones - @TotalOrdenesCompra;
    
    SELECT 
        @TotalDonaciones AS TotalDonaciones,
        @TotalOrdenesCompra AS TotalOrdenesCompra,
        @Balance AS Balance,
        CAST(1 AS BIT) AS RubroActivo;
END
GO



---------

-- POWER BI
-- Vista para proyectos y rubros
CREATE VIEW vw_ProyectosRubros AS
SELECT 
    p.*,
    r.Id AS RubroId,
    r.Codigo AS CodigoRubro,
    r.Nombre AS NombreRubro,
    r.Active AS RubroActivo
FROM Proyectos p
INNER JOIN Rubros r ON p.Id = r.IdProyecto
WHERE p.Active = 1 AND r.Active = 1;

-- Vista para donaciones completas
CREATE VIEW vw_DonacionesCompletas AS
SELECT 
    d.*,
    r.Codigo AS CodigoRubro,
    r.Nombre AS NombreRubro,
    p.Codigo AS CodigoProyecto,
    p.Nombre AS NombreProyecto,
    p.Municipio,
    p.Departamento
FROM Donaciones d
INNER JOIN Rubros r ON d.IdRubro = r.Id AND r.Active = 1
INNER JOIN Proyectos p ON r.IdProyecto = p.Id AND p.Active = 1
WHERE d.Active = 1;

-- Vista para órdenes de compra completas
CREATE VIEW vw_OrdenesCompraCompletas AS
SELECT 
    oc.*,
    r.Codigo AS CodigoRubro,
    r.Nombre AS NombreRubro,
    p.Codigo AS CodigoProyecto,
    p.Nombre AS NombreProyecto,
    p.Municipio,
    p.Departamento
FROM OrdenesCompra oc
INNER JOIN Rubros r ON oc.IdRubro = r.Id AND r.Active = 1
INNER JOIN Proyectos p ON r.IdProyecto = p.Id AND p.Active = 1
WHERE oc.Active = 1;

-- Vista para los Top 5 Donantes
CREATE VIEW vw_Top5Donantes AS
SELECT TOP 5
    d.NombreDonante,
    SUM(d.Monto) AS TotalDonado,
    COUNT(d.Id) AS CantidadDonaciones
FROM Donaciones d
INNER JOIN Rubros r ON d.IdRubro = r.Id AND r.Active = 1
INNER JOIN Proyectos p ON r.IdProyecto = p.Id AND p.Active = 1
WHERE d.Active = 1
GROUP BY d.NombreDonante
ORDER BY TotalDonado DESC;