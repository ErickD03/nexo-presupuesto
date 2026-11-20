using System.Data;
using Microsoft.Data.SqlClient;

namespace NexoPresupuesto.Datos;

// Guarda los totales que se muestran en las tarjetas del panel
public class ResumenPanel
{
    public decimal Aprobado { get; set; }
    public decimal Gastado { get; set; }

    public decimal Saldo
    {
        get { return Aprobado - Gastado; }
    }

    public decimal Ejecucion
    {
        get
        {
            if (Aprobado == 0)
            {
                return 0;
            }
            return Gastado * 100 / Aprobado;
        }
    }
}

// Consultas del panel (son las consultas 2, 3 y 6 del script 03)
public static class PanelDatos
{
    // Totales del período para las tarjetas
    public static ResumenPanel ObtenerResumen(int anio)
    {
        ResumenPanel resumen = new ResumenPanel();

        string sqlAprobado = @"SELECT ISNULL(SUM(P.MontoAprobado), 0)
                               FROM Presupuestos P
                               INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
                               WHERE PE.Anio = @anio";

        // Solo cuentan las transacciones activas (las anuladas no)
        string sqlGastado = @"SELECT ISNULL(SUM(T.Monto), 0)
                              FROM Transacciones T
                              INNER JOIN Presupuestos P ON P.IdPresupuesto = T.IdPresupuesto
                              INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
                              WHERE PE.Anio = @anio
                                AND T.Estado = 'Activa'";

        using (SqlConnection conexion = ConexionBD.ObtenerConexion())
        {
            conexion.Open();

            using (SqlCommand comando = new SqlCommand(sqlAprobado, conexion))
            {
                comando.Parameters.AddWithValue("@anio", anio);
                resumen.Aprobado = Convert.ToDecimal(comando.ExecuteScalar());
            }

            using (SqlCommand comando = new SqlCommand(sqlGastado, conexion))
            {
                comando.Parameters.AddWithValue("@anio", anio);
                resumen.Gastado = Convert.ToDecimal(comando.ExecuteScalar());
            }
        }

        return resumen;
    }

    // Presupuesto contra gastos por categoría
    public static DataTable ObtenerPorCategoria(int anio)
    {
        string sql = @"SELECT C.Nombre AS Categoria,
                              A.Aprobado,
                              ISNULL(G.Gastado, 0) AS Gastado,
                              A.Aprobado - ISNULL(G.Gastado, 0) AS Saldo
                       FROM Categorias C
                       INNER JOIN (
                           SELECT P.IdCategoria, SUM(P.MontoAprobado) AS Aprobado
                           FROM Presupuestos P
                           INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
                           WHERE PE.Anio = @anio
                           GROUP BY P.IdCategoria
                       ) A ON A.IdCategoria = C.IdCategoria
                       LEFT JOIN (
                           SELECT P.IdCategoria, SUM(T.Monto) AS Gastado
                           FROM Transacciones T
                           INNER JOIN Presupuestos P ON P.IdPresupuesto = T.IdPresupuesto
                           INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
                           WHERE PE.Anio = @anio
                             AND T.Estado = 'Activa'
                           GROUP BY P.IdCategoria
                       ) G ON G.IdCategoria = C.IdCategoria
                       ORDER BY C.Nombre";

        DataTable tabla = new DataTable();

        using (SqlConnection conexion = ConexionBD.ObtenerConexion())
        using (SqlCommand comando = new SqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@anio", anio);
            SqlDataAdapter adaptador = new SqlDataAdapter(comando);
            adaptador.Fill(tabla);
        }

        return tabla;
    }

    // Las 5 transacciones más recientes (incluye las anuladas)
    public static DataTable ObtenerRecientes()
    {
        string sql = @"SELECT TOP 5
                              T.Fecha,
                              T.Descripcion,
                              C.Nombre AS Categoria,
                              T.Monto,
                              T.Estado
                       FROM Transacciones T
                       INNER JOIN Presupuestos P ON P.IdPresupuesto = T.IdPresupuesto
                       INNER JOIN Categorias C ON C.IdCategoria = P.IdCategoria
                       ORDER BY T.Fecha DESC, T.IdTransaccion DESC";

        DataTable tabla = new DataTable();

        using (SqlConnection conexion = ConexionBD.ObtenerConexion())
        using (SqlCommand comando = new SqlCommand(sql, conexion))
        {
            SqlDataAdapter adaptador = new SqlDataAdapter(comando);
            adaptador.Fill(tabla);
        }

        return tabla;
    }
}
