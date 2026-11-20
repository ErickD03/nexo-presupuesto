using Microsoft.Data.SqlClient;

namespace NexoPresupuesto.Datos;

public static class UsuarioDatos
{
    // Revisa si el usuario y la clave son correctos.
    // Si lo son, guarda los datos en Sesion y devuelve true.
    public static bool ValidarLogin(string nombreUsuario, string clave)
    {
        // La clave nunca se compara en texto plano, solo su hash
        string hash = Seguridad.CalcularHash(clave);

        // Se usan parámetros (@usuario, @clave) para evitar inyección SQL
        string sql = @"SELECT IdUsuario, Nombre, Rol
                       FROM Usuarios
                       WHERE NombreUsuario = @usuario
                         AND ClaveHash = @clave
                         AND Estado = 1";

        using (SqlConnection conexion = ConexionBD.ObtenerConexion())
        using (SqlCommand comando = new SqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@usuario", nombreUsuario);
            comando.Parameters.AddWithValue("@clave", hash);

            conexion.Open();

            using (SqlDataReader lector = comando.ExecuteReader())
            {
                if (lector.Read())
                {
                    Sesion.IdUsuario = lector.GetInt32(0);
                    Sesion.Nombre = lector.GetString(1);
                    Sesion.Rol = lector.GetString(2);
                    return true;
                }
            }
        }

        return false;
    }
}
