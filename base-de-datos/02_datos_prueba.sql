/* =====================================================================
   NEXO Presupuesto - Sistema de Control de Presupuesto
   Script 02 datos de prueba
   Curso: Base de Datos I
   ---------------------------------------------------------------------
   Se ejecuta UNA sola vez, justo después del script 01
   Si lo necesitan repetir, ejecuten primero el script 01 (borra todo)
   Los datos cuadran con el mockup del panel de control:
   presupuesto aprobado 25.000.000, gastos 14.250.000, saldo 10.750.000
   ===================================================================== */

USE NexoPresupuesto;
GO

-- Usuarios (la clave se guarda con hash SHA-256)
-- Para probar: admin / Admin123   y   consulta / Consulta123
-- La contraseña no se guarda en texto plano, se guarda su hash
INSERT INTO Usuarios (Nombre, NombreUsuario, ClaveHash, Rol)
VALUES
('Administrador NEXO', 'admin',
    CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Admin123'), 2), 'Administrador'),
('Usuario de consulta', 'consulta',
    CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Consulta123'), 2), 'Consulta');

-- Departamentos (ids 1 a 5, en este orden)
INSERT INTO Departamentos (Nombre, Descripcion)
VALUES
('Gerencia', 'Dirección general de la empresa'),
('Administración y Finanzas', 'Registra los gastos y prepara los reportes'),
('Operaciones', 'Gastos de la operación diaria'),
('Tecnología', 'Equipo de cómputo y servicios tecnológicos'),
('Ventas', 'Publicidad y gestión comercial');

-- Categorías de gasto (ids 1 a 4, en este orden)
INSERT INTO Categorias (Nombre, Descripcion)
VALUES
('Operaciones', 'Alquiler, servicios y mantenimiento'),
('Tecnología', 'Equipo, licencias y servicios en la nube'),
('Administración', 'Papelería, capacitación y asesorías'),
('Ventas', 'Publicidad y ferias comerciales');

-- Período 2026
INSERT INTO Periodos (Anio, FechaInicio, FechaFin)
VALUES (2026, '2026-01-01', '2026-12-31');

-- Presupuestos: (período, departamento, categoría, monto aprobado)
-- Suman 25.000.000
INSERT INTO Presupuestos (IdPeriodo, IdDepartamento, IdCategoria, MontoAprobado)
VALUES
(1, 3, 1, 8000000),   -- id 1: Operaciones / Operaciones
(1, 4, 2, 6000000),   -- id 2: Tecnología / Tecnología
(1, 2, 3, 4000000),   -- id 3: Administración y Finanzas / Administración
(1, 1, 3, 1000000),   -- id 4: Gerencia / Administración
(1, 5, 4, 6000000);   -- id 5: Ventas / Ventas
GO

-- Transacciones activas (las registra el usuario 1; el estado queda 'Activa' por defecto)
INSERT INTO Transacciones (IdPresupuesto, IdUsuario, Fecha, Descripcion, Monto)
VALUES
-- Operaciones
(1, 1, '2026-01-31', 'Alquiler de oficina', 1800000),
(1, 1, '2026-05-10', 'Mantenimiento de instalaciones', 1350000),
(1, 1, '2026-08-18', 'Transporte y logística', 1017500),
(1, 1, '2026-09-22', 'Servicios públicos', 500000),
(1, 1, '2026-10-08', 'Pago de servicios públicos', 320000),
-- Tecnología
(2, 1, '2026-02-12', 'Licencias de software', 1200000),
(2, 1, '2026-04-20', 'Servicios en la nube', 925000),
(2, 1, '2026-07-15', 'Equipo de red', 587500),
(2, 1, '2026-10-09', 'Compra de equipo de cómputo', 850000),
-- Administración y Finanzas
(3, 1, '2026-06-12', 'Papelería e insumos', 765000),
(3, 1, '2026-09-08', 'Capacitación del personal', 1000000),
(3, 1, '2026-10-07', 'Materiales de oficina', 185000),
-- Gerencia
(4, 1, '2026-03-05', 'Honorarios de asesoría legal', 900000),
-- Ventas
(5, 1, '2026-03-18', 'Publicidad campaña Q1', 600000),
(5, 1, '2026-06-25', 'Publicidad campaña Q2', 800000),
(5, 1, '2026-08-12', 'Participación en feria comercial', 700000),
(5, 1, '2026-10-06', 'Publicidad campaña Q4', 750000);

-- Una transacción anulada (no cuenta en los gastos, pero se conserva)
INSERT INTO Transacciones
    (IdPresupuesto, IdUsuario, Fecha, Descripcion, Monto,
     Estado, MotivoAnulacion, FechaAnulacion, IdUsuarioAnula)
VALUES
(2, 1, '2026-10-05', 'Mantenimiento de equipos', 450000,
 'Anulada', 'Registro duplicado', '2026-10-06', 1);
GO

/* ---------------------------------------------------------------------
   Verificación
   Filas esperadas: Usuarios 2, Departamentos 5, Categorias 4,
   Periodos 1, Presupuestos 5, Transacciones 18
   --------------------------------------------------------------------- */
SELECT 'Usuarios' AS Tabla, COUNT(*) AS Filas FROM Usuarios
UNION ALL SELECT 'Departamentos', COUNT(*) FROM Departamentos
UNION ALL SELECT 'Categorias', COUNT(*) FROM Categorias
UNION ALL SELECT 'Periodos', COUNT(*) FROM Periodos
UNION ALL SELECT 'Presupuestos', COUNT(*) FROM Presupuestos
UNION ALL SELECT 'Transacciones', COUNT(*) FROM Transacciones;

-- Debe dar 25000000
SELECT SUM(MontoAprobado) AS PresupuestoAprobado FROM Presupuestos;

-- Debe dar 14250000 (solo las activas)
SELECT SUM(Monto) AS GastosRegistrados FROM Transacciones WHERE Estado = 'Activa';
GO