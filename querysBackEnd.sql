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

-- ELIMINAR PROYECTO
CREATE OR ALTER PROCEDURE sp_eliminarProyecto
    @Codigo VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS(SELECT 1 FROM Proyectos WHERE Codigo = @Codigo)
    BEGIN
        SELECT CAST(0 AS BIT) AS Success, N'Proyecto no existe' AS [Message]; RETURN;
    END;

    BEGIN TRY
        UPDATE Proyectos
        SET Active = 0
        WHERE Codigo = @Codigo;

        SELECT CAST(1 AS BIT) AS Success, N'Proyecto eliminado' AS [Message];
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS BIT) AS Success, CONCAT(N'ERROR ENCONTRADO: ', ERROR_MESSAGE()) AS [Message];
    END CATCH
END

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