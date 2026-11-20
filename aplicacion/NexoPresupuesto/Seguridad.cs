using System.Security.Cryptography;
using System.Text;

namespace NexoPresupuesto;

public static class Seguridad
{
    // Calcula el SHA-256 en hexadecimal (mayúsculas).
    // Da el mismo resultado que HASHBYTES('SHA2_256', ...) del script 02.
    public static string CalcularHash(string clave)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(clave);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
