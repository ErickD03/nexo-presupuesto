using System.Data;
using Microsoft.Data.SqlClient;

namespace NexoPresupuesto.Datos;

// Acceso a datos para registrar, consultar y anular transacciones
public static class TransaccionDatos
{
    // Presupuestos del período con su saldo (para el combo del formulario)
    public static DataTable ObtenerPresupuestos(int anio)
    {
        string sql = @"SELECT P.IdPresupuesto,
                              D.Nombre + ' / ' + C.Nombre AS Presupuesto,
                              P.MontoAprobado,
                              ISNULL(G.Gastado, 0) AS Gastado,
                              P.MontoAprobado - ISNULL(G.Gastado, 0) AS Saldo
                       FROM Presupuestos P
                       INNER JOIN Periodos PE ON PE.IdPeriodo = P.IdPeriodo
                       INNER JOIN Departamentos D ON D.IdDepartamento = P.IdDepartamento
                       INNER JOIN Categorias C ON C.IdCategoria = P.IdCategoria
                       LEFT JOIN (
                           SELECT IdPresupuesto, SUM(Monto) AS Gastado
                           FROM Transacciones
                           WHERE Estado = 'Activa'
                           GROUP BY IdPresupuesto
                       ) G ON G.IdPresupuesto = P.IdPresupuesto
                       WHERE PE.Anio = @anio
                       ORDER BY D.Nombre, C.Nombre";

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

    // Saldo actual de un presupuesto: lo aprobado menos las transacciones activas
    public static decimal ObtenerSaldo(int idPresupuesto)
    {
        string sql = @"SELECT P.MontoAprobado - ISNULL(
                           (SELECT SUM(T.Monto)
                            FROM Transacciones T
                            WHERE T.IdPresupuesto = P.IdPresupuesto
                              AND T.Estado = 'Activa'), 0)
                       FROM Presupuestos P
                       WHERE P.IdPresupuesto = @id";

        using (SqlConnection conexion = ConexionBD.ObtenerConexion())
        using (SqlCommand comando = new SqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@id", idPresupuesto);
            conexion.Open();

            object? resultado = comando.ExecuteScalar();
            if (resultado == null || resultado == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToDecimal(resultado);
        }
    }

    // Todas las transacciones (activas y anuladas), de la más reciente a la más antigua
    public static DataTable ObtenerTodas()
    {
        string sql = @"SELECT T.IdTransaccion,
                              T.Fecha,
                              T.Descripcion,
                              C.Nombre AS Categoria,
                              D.Nombre AS Departamento,
                              T.Monto,
                              T.Estado,
                              T.MotivoAnulacion
                       FROM Transacciones T
                       INNER JOIN Presupuestos P ON P.IdPresupuesto = T.IdPresupuesto
                       INNER JOIN Categorias C ON C.IdCategoria = P.IdCategoria
                       INNER JOIN Departamentos D ON D.IdDepartamento = P.IdDepartamento
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

    // Registra una transacción nueva. El estado queda 'Activa' por defecto
    // y el usuario es el que tiene la sesión abierta.
    public static void Registrar(int idPresupuesto, DateTime fecha, string descripcion, decimal monto)
    {
        string sql = @"INSERT INTO Transacciones (IdPresupuesto, IdUsuario, Fecha, Descripcion, Monto)
                       VALUES (@presupuesto, @usuario, @fecha, @descripcion, @monto)";

        using (SqlConnection conexion = ConexionBD.ObtenerConexion())
        using (SqlCommand comando = new SqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@presupuesto", idPresupuesto);
            comando.Parameters.AddWithValue("@usuario", Sesion.IdUsuario);
            comando.Parameters.Add("@fecha", SqlDbType.Date).Value = fecha.Date;
            comando.Parameters.AddWithValue("@descripcion", descripcion);
            comando.Parameters.AddWithValue("@monto", monto);

            conexion.Open();
            comando.ExecuteNonQuery();
        }
    }

    // Anula una transacción: NO se borra, se guarda el motivo, la fecha y el usuario.
    // Devuelve false si no se pudo anular (por ejemplo, si ya estaba anulada).
    public static bool Anular(int idTransaccion, string motivo)
    {
        string sql = @"UPDATE Transacciones
                       SET Estado = 'Anulada',
                           MotivoAnulacion = @motivo,
                           FechaAnulacion = @fecha,
                           IdUsuarioAnula = @usuario
                       WHERE IdTransaccion = @id
                         AND Estado = 'Activa'";

        using (SqlConnection conexion = ConexionBD.ObtenerConexion())
        using (SqlCommand comando = new SqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@motivo", motivo);
            comando.Parameters.Add("@fecha", SqlDbType.Date).Value = DateTime.Today;
            comando.Parameters.AddWithValue("@usuario", Sesion.IdUsuario);
            comando.Parameters.AddWithValue("@id", idTransaccion);

            conexion.Open();
            int filas = comando.ExecuteNonQuery();
            return filas > 0;
        }
    }
}
