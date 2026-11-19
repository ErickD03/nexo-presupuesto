/* =====================================================================
   NEXO Presupuesto - Sistema de Control de Presupuesto
   Script 01 para la creación de la base de datos y de las tablas
   Curso: Base de Datos I
   Motor: Microsoft SQL Server 2019 o superior
   ---------------------------------------------------------------------
   IMPORTANTE: si se vuelve a ejecutar, borra las tablas y las crea de nuevo
   (se pierden los datos). Utilizar solo mientras se está desarrollando.
   ===================================================================== */

IF DB_ID('NexoPresupuesto') IS NULL
    CREATE DATABASE NexoPresupuesto;
GO

USE NexoPresupuesto;
GO

/* ---------------------------------------------------------------------
   Borrar tablas anteriores (primero las hijas, después las padres)
   --------------------------------------------------------------------- */
DROP TABLE IF EXISTS Transacciones;
DROP TABLE IF EXISTS Presupuestos;
DROP TABLE IF EXISTS Periodos;
DROP TABLE IF EXISTS Categorias;
DROP TABLE IF EXISTS Departamentos;
DROP TABLE IF EXISTS Usuarios;
GO

/* ---------------------------------------------------------------------
   Tablas de catálogo
   --------------------------------------------------------------------- */

-- Usuarios del sistema. La clave se guarda como hash, nunca en texto plano.
CREATE TABLE Usuarios (
    IdUsuario     INT IDENTITY(1,1) NOT NULL,
    Nombre        VARCHAR(100) NOT NULL,
    NombreUsuario VARCHAR(50)  NOT NULL,
    ClaveHash     VARCHAR(64)  NOT NULL,
    Rol           VARCHAR(20)  NOT NULL,
    Estado        BIT          NOT NULL DEFAULT 1,
    CONSTRAINT PK_Usuarios PRIMARY KEY (IdUsuario),
    CONSTRAINT UQ_Usuarios_NombreUsuario UNIQUE (NombreUsuario),
    CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('Administrador', 'Consulta'))
);
GO

-- Departamentos de la empresa (Gerencia, Operaciones, etc.)
CREATE TABLE Departamentos (
    IdDepartamento INT IDENTITY(1,1) NOT NULL,
    Nombre         VARCHAR(80)  NOT NULL,
    Descripcion    VARCHAR(200) NULL,
    Estado         BIT          NOT NULL DEFAULT 1,
    CONSTRAINT PK_Departamentos PRIMARY KEY (IdDepartamento),
    CONSTRAINT UQ_Departamentos_Nombre UNIQUE (Nombre)
);
GO

-- Categorías de gasto (Tecnología, Servicios públicos, etc.)
CREATE TABLE Categorias (
    IdCategoria INT IDENTITY(1,1) NOT NULL,
    Nombre      VARCHAR(80)  NOT NULL,
    Descripcion VARCHAR(200) NULL,
    Estado      BIT          NOT NULL DEFAULT 1,
    CONSTRAINT PK_Categorias PRIMARY KEY (IdCategoria),
    CONSTRAINT UQ_Categorias_Nombre UNIQUE (Nombre)
);
GO

-- Períodos presupuestarios (por ejemplo, el año 2026)
CREATE TABLE Periodos (
    IdPeriodo   INT IDENTITY(1,1) NOT NULL,
    Anio        INT  NOT NULL,
    FechaInicio DATE NOT NULL,
    FechaFin    DATE NOT NULL,
    Estado      BIT  NOT NULL DEFAULT 1,
    CONSTRAINT PK_Periodos PRIMARY KEY (IdPeriodo),
    CONSTRAINT UQ_Periodos_Anio UNIQUE (Anio),
    CONSTRAINT CK_Periodos_Fechas CHECK (FechaFin > FechaInicio)
);
GO

/* ---------------------------------------------------------------------
   Presupuestos: une un período, un departamento y una categoría
   --------------------------------------------------------------------- */
CREATE TABLE Presupuestos (
    IdPresupuesto  INT IDENTITY(1,1) NOT NULL,
    IdPeriodo      INT           NOT NULL,
    IdDepartamento INT           NOT NULL,
    IdCategoria    INT           NOT NULL,
    MontoAprobado  DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_Presupuestos PRIMARY KEY (IdPresupuesto),
    CONSTRAINT FK_Presupuestos_Periodos
        FOREIGN KEY (IdPeriodo) REFERENCES Periodos (IdPeriodo),
    CONSTRAINT FK_Presupuestos_Departamentos
        FOREIGN KEY (IdDepartamento) REFERENCES Departamentos (IdDepartamento),
    CONSTRAINT FK_Presupuestos_Categorias
        FOREIGN KEY (IdCategoria) REFERENCES Categorias (IdCategoria),
    -- Evita dos presupuestos para la misma combinación
    CONSTRAINT UQ_Presupuestos_Combinacion UNIQUE (IdPeriodo, IdDepartamento, IdCategoria),
    CONSTRAINT CK_Presupuestos_Monto CHECK (MontoAprobado > 0)
);
GO

/* ---------------------------------------------------------------------
   Transacciones: los gastos registrados contra un presupuesto.
   Una transacción anulada NO se borra: se conserva el motivo, la fecha
   y el usuario que la anuló. Esos tres datos quedan NULL mientras la
   transacción está activa (la aplicación se encarga de llenarlos).
   --------------------------------------------------------------------- */
CREATE TABLE Transacciones (
    IdTransaccion   INT IDENTITY(1,1) NOT NULL,
    IdPresupuesto   INT           NOT NULL,
    IdUsuario       INT           NOT NULL,   -- quién la registró
    Fecha           DATE          NOT NULL,
    Descripcion     VARCHAR(200)  NOT NULL,
    Monto           DECIMAL(18,2) NOT NULL,
    Estado          VARCHAR(10)   NOT NULL DEFAULT 'Activa',
    MotivoAnulacion VARCHAR(200)  NULL,
    FechaAnulacion  DATE          NULL,
    IdUsuarioAnula  INT           NULL,       -- quién la anuló
    CONSTRAINT PK_Transacciones PRIMARY KEY (IdTransaccion),
    CONSTRAINT FK_Transacciones_Presupuestos
        FOREIGN KEY (IdPresupuesto) REFERENCES Presupuestos (IdPresupuesto),
    CONSTRAINT FK_Transacciones_UsuarioRegistra
        FOREIGN KEY (IdUsuario) REFERENCES Usuarios (IdUsuario),
    CONSTRAINT FK_Transacciones_UsuarioAnula
        FOREIGN KEY (IdUsuarioAnula) REFERENCES Usuarios (IdUsuario),
    CONSTRAINT CK_Transacciones_Monto CHECK (Monto > 0),
    CONSTRAINT CK_Transacciones_Estado CHECK (Estado IN ('Activa', 'Anulada'))
);
GO

/* ---------------------------------------------------------------------
   Verificación: deben salir 6 tablas y 6 llaves foráneas
   --------------------------------------------------------------------- */
SELECT TABLE_NAME AS Tabla
FROM INFORMATION_SCHEMA.TABLES
ORDER BY TABLE_NAME;

SELECT CONSTRAINT_NAME AS LlaveForanea, TABLE_NAME AS Tabla
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'
ORDER BY TABLE_NAME, CONSTRAINT_NAME;
GO