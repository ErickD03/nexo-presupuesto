using Microsoft.Data.SqlClient;

namespace NexoPresupuesto.Datos;

public static class ConexionBD
{
    // Servidor local con autenticación de Windows.
    // Si su SQL Server es una instancia con nombre, cambien localhost por
    // localhost\SQLEXPRESS (o el nombre que tenga).
    private const string Cadena =
        "Server=localhost;Database=NexoPresupuesto;Integrated Security=True;TrustServerCertificate=True;";

    // Devuelve una conexión nueva. Quien la use debe abrirla y cerrarla.
    public static SqlConnection ObtenerConexion()
    {
        return new SqlConnection(Cadena);
    }

    // Prueba si se puede conectar. Si falla, devuelve el mensaje del error.
    public static bool ProbarConexion(out string mensaje)
    {
        try
        {
            using (SqlConnection conexion = ObtenerConexion())
            {
                conexion.Open();
            }

            mensaje = "Conexión correcta";
            return true;
        }
        catch (Exception ex)
        {
            mensaje = ex.Message;
            return false;
        }
    }
}
