namespace NexoPresupuesto;

// Pantalla principal (por ahora solo un saludo; el panel se arma en el paso 5)
public class FrmPrincipal : Form
{
    public FrmPrincipal()
    {
        Text = "NEXO Presupuesto";
        Size = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterScreen;

        Label lblBienvenida = new Label();
        lblBienvenida.Text = "Bienvenido, " + Sesion.Nombre + " (" + Sesion.Rol + ")";
        lblBienvenida.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        lblBienvenida.AutoSize = true;
        lblBienvenida.Location = new Point(30, 30);

        Controls.Add(lblBienvenida);
    }
}
