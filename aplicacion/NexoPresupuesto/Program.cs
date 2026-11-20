namespace NexoPresupuesto;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Primero se muestra el login; si entra bien, se abre la pantalla principal
        FrmLogin login = new FrmLogin();
        if (login.ShowDialog() == DialogResult.OK)
        {
            Application.Run(new FrmPrincipal());
        }
    }
}
