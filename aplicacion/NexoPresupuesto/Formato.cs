using System.Globalization;

namespace NexoPresupuesto;

// Ayuda para mostrar los montos como en el mockup: ₡25.000.000
public static class Formato
{
    // La cultura alemana usa el punto como separador de miles
    public static CultureInfo Cultura = new CultureInfo("de-DE");

    public static string Colones(decimal monto)
    {
        return "₡" + monto.ToString("#,##0", Cultura);
    }
}
