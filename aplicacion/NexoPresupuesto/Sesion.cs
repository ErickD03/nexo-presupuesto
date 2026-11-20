namespace NexoPresupuesto;

// Guarda quién inició sesión, para usarlo en las demás pantallas
public static class Sesion
{
    public static int IdUsuario { get; set; }
    public static string Nombre { get; set; } = "";
    public static string Rol { get; set; } = "";
}
