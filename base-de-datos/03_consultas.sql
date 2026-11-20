/* =====================================================================
   NEXO Presupuesto - Sistema de Control de Presupuesto
   Script 03 consultas del panel de control y del inicio de sesión
   Curso: Base de Datos I
   ---------------------------------------------------------------------
   Cada consulta es independiente: se puede seleccionar solo una y
   ejecutarla con F5. Los resultados esperados (con los datos del
   script 02) están anotados en los comentarios.
   ===================================================================== */

USE NexoPresupuesto;
GO

/* ---------------------------------------------------------------------
   Consulta 1: inicio de sesión
   Compara el hash de la clave escrita con el hash guardado.
   Si devuelve una fila, el usuario puede entrar; si no, no.
   Resultado esperado: 1 fila (Administrador NEXO, Administrador)
   --------------------------------------------------------------------- */
SELECT IdUsuario, Nombre, Rol
FROM Usuarios
WHERE NombreUsuario = 'admin'
  AND ClaveHash = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Admin123'), 2)
  AND Estado = 1;
GO

/* ---------------------------------------------------------------------
   Consulta 2: tarjetas del panel (resumen del período)
   Resultado esperado: 25000000 | 14250000 | 10750000 | 57.00
   --------------------------------------------------------------------- */
DECLARE @Anio INT = 2026;
DECLARE @Aprobado DECIMAL(18,2);
DECLARE @Gastado DECIMAL(18,2);

-- Total aprobado del período
SELECT @Aprobado = SUM(P.MontoAprobado)
FROM Presupuestos P
INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
WHERE PE.Anio = @Anio;

-- Total gastado: solo transacciones activas (las anuladas no cuentan)
SELECT @Gastado = ISNULL(SUM(T.Monto), 0)
FROM Transacciones T
INNER JOIN Presupuestos P ON P.IdPresupuesto = T.IdPresupuesto
INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
WHERE PE.Anio = @Anio
  AND T.Estado = 'Activa';

SELECT @Aprobado AS PresupuestoAprobado,
       @Gastado AS GastosRegistrados,
       @Aprobado - @Gastado AS SaldoDisponible,
       CAST(@Gastado * 100.0 / @Aprobado AS DECIMAL(5,2)) AS EjecucionPorcentaje;
GO

/* ---------------------------------------------------------------------
   Consulta 3: presupuesto contra gastos por categoría (gráfico de barras)
   Se usan dos subconsultas porque si se unen Presupuestos y Transacciones
   directamente, el monto aprobado se repetiría por cada transacción.
   Resultado esperado (4 filas):
   Administración 5000000 | 2850000 | 2150000
   Operaciones    8000000 | 4987500 | 3012500
   Tecnología     6000000 | 3562500 | 2437500
   Ventas         6000000 | 2850000 | 3150000
   --------------------------------------------------------------------- */
DECLARE @Anio INT = 2026;

SELECT C.Nombre AS Categoria,
       A.Aprobado,
       ISNULL(G.Gastado, 0) AS Gastado,
       A.Aprobado - ISNULL(G.Gastado, 0) AS Saldo
FROM Categorias C
INNER JOIN (
    SELECT P.IdCategoria, SUM(P.MontoAprobado) AS Aprobado
    FROM Presupuestos P
    INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
    WHERE PE.Anio = @Anio
    GROUP BY P.IdCategoria
) A ON A.IdCategoria = C.IdCategoria
LEFT JOIN (
    SELECT P.IdCategoria, SUM(T.Monto) AS Gastado
    FROM Transacciones T
    INNER JOIN Presupuestos P ON P.IdPresupuesto = T.IdPresupuesto
    INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
    WHERE PE.Anio = @Anio
      AND T.Estado = 'Activa'
    GROUP BY P.IdCategoria
) G ON G.IdCategoria = C.IdCategoria
ORDER BY C.Nombre;
GO

/* ---------------------------------------------------------------------
   Consulta 4: distribución de gastos por categoría (gráfico de dona)
   Resultado esperado (porcentajes 35, 25, 20 y 20):
   Operaciones 4987500 | 35.00
   Tecnología  3562500 | 25.00
   Administración 2850000 | 20.00
   Ventas      2850000 | 20.00
   --------------------------------------------------------------------- */
DECLARE @Anio INT = 2026;
DECLARE @Total DECIMAL(18,2);

SELECT @Total = SUM(T.Monto)
FROM Transacciones T
INNER JOIN Presupuestos P ON P.IdPresupuesto = T.IdPresupuesto
INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
WHERE PE.Anio = @Anio
  AND T.Estado = 'Activa';

SELECT C.Nombre AS Categoria,
       SUM(T.Monto) AS Gastado,
       CAST(SUM(T.Monto) * 100.0 / @Total AS DECIMAL(5,2)) AS Porcentaje
FROM Transacciones T
INNER JOIN Presupuestos P ON P.IdPresupuesto = T.IdPresupuesto
INNER JOIN Categorias C ON C.IdCategoria = P.IdCategoria
INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
WHERE PE.Anio = @Anio
  AND T.Estado = 'Activa'
GROUP BY C.Nombre
ORDER BY Gastado DESC, Categoria;
GO

/* ---------------------------------------------------------------------
   Consulta 5: presupuesto, gasto y ejecución por departamento
   Resultado esperado (5 filas):
   Administración y Finanzas 4000000 | 1950000 | 2050000 | 48.75
   Gerencia                  1000000 |  900000 |  100000 | 90.00
   Operaciones               8000000 | 4987500 | 3012500 | 62.34
   Tecnología                6000000 | 3562500 | 2437500 | 59.38
   Ventas                    6000000 | 2850000 | 3150000 | 47.50
   --------------------------------------------------------------------- */
DECLARE @Anio INT = 2026;

SELECT D.Nombre AS Departamento,
       A.Aprobado,
       ISNULL(G.Gastado, 0) AS Gastado,
       A.Aprobado - ISNULL(G.Gastado, 0) AS Saldo,
       CAST(ISNULL(G.Gastado, 0) * 100.0 / A.Aprobado AS DECIMAL(5,2)) AS EjecucionPorcentaje
FROM Departamentos D
INNER JOIN (
    SELECT P.IdDepartamento, SUM(P.MontoAprobado) AS Aprobado
    FROM Presupuestos P
    INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
    WHERE PE.Anio = @Anio
    GROUP BY P.IdDepartamento
) A ON A.IdDepartamento = D.IdDepartamento
LEFT JOIN (
    SELECT P.IdDepartamento, SUM(T.Monto) AS Gastado
    FROM Transacciones T
    INNER JOIN Presupuestos P ON P.IdPresupuesto = T.IdPresupuesto
    INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
    WHERE PE.Anio = @Anio
      AND T.Estado = 'Activa'
    GROUP BY P.IdDepartamento
) G ON G.IdDepartamento = D.IdDepartamento
ORDER BY D.Nombre;
GO

/* ---------------------------------------------------------------------
   Consulta 6: transacciones recientes (tabla del panel)
   Incluye las anuladas, como en el mockup.
   Resultado esperado (5 filas, de la más reciente a la más antigua):
   2026-10-09 Compra de equipo de cómputo  Tecnología     850000 Activa
   2026-10-08 Pago de servicios públicos   Operaciones    320000 Activa
   2026-10-07 Materiales de oficina        Administración 185000 Activa
   2026-10-06 Publicidad campaña Q4        Ventas         750000 Activa
   2026-10-05 Mantenimiento de equipos     Tecnología     450000 Anulada
   --------------------------------------------------------------------- */
SELECT TOP 5
       T.Fecha,
       T.Descripcion,
       C.Nombre AS Categoria,
       T.Monto,
       T.Estado
FROM Transacciones T
INNER JOIN Presupuestos P ON P.IdPresupuesto = T.IdPresupuesto
INNER JOIN Categorias C ON C.IdCategoria = P.IdCategoria
ORDER BY T.Fecha DESC, T.IdTransaccion DESC;
GO

/* ---------------------------------------------------------------------
   Consulta 7: transacciones anuladas (trazabilidad)
   Muestra el motivo, la fecha y quién la anuló.
   Resultado esperado: 1 fila (Mantenimiento de equipos, Registro
   duplicado, 2026-10-06, Administrador NEXO)
   --------------------------------------------------------------------- */
SELECT T.IdTransaccion,
       T.Descripcion,
       T.Monto,
       T.MotivoAnulacion,
       T.FechaAnulacion,
       U.Nombre AS AnuladaPor
FROM Transacciones T
INNER JOIN Usuarios U ON U.IdUsuario = T.IdUsuarioAnula
WHERE T.Estado = 'Anulada';
GO