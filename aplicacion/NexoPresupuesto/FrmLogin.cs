using NexoPresupuesto.Datos;

namespace NexoPresupuesto;

// Pantalla de inicio de sesión. Se arma por código para no depender del diseñador.
public class FrmLogin : Form
{
    private TextBox txtUsuario = new TextBox();
    private TextBox txtClave = new TextBox();
    private Button btnIngresar = new Button();
    private Label lblMensaje = new Label();

    // Colores de la identidad visual de NEXO
    private Color azulOscuro = ColorTranslator.FromHtml("#12304A");
    private Color turquesa = ColorTranslator.FromHtml("#12B8B0");

    public FrmLogin()
    {
        ConstruirPantalla();
    }

    private void ConstruirPantalla()
    {
        // Ventana
        Text = "NEXO Presupuesto - Inicio de sesión";
        ClientSize = new Size(380, 460);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = azulOscuro;
        Font = new Font("Segoe UI", 10);

        // Títulos
        Label lblTitulo = new Label();
        lblTitulo.Text = "NEXO";
        lblTitulo.Font = new Font("Segoe UI", 32, FontStyle.Bold);
        lblTitulo.ForeColor = Color.White;
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(36, 25);

        Label lblSubtitulo = new Label();
        lblSubtitulo.Text = "PRESUPUESTO";
        lblSubtitulo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        lblSubtitulo.ForeColor = turquesa;
        lblSubtitulo.AutoSize = true;
        lblSubtitulo.Location = new Point(42, 90);

        Label lblSesion = new Label();
        lblSesion.Text = "Inicio de sesión";
        lblSesion.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        lblSesion.ForeColor = Color.White;
        lblSesion.AutoSize = true;
        lblSesion.Location = new Point(40, 150);

        // Campos de texto
        txtUsuario.PlaceholderText = "Nombre de usuario";
        txtUsuario.Font = new Font("Segoe UI", 11);
        txtUsuario.Width = 300;
        txtUsuario.Location = new Point(40, 205);

        txtClave.PlaceholderText = "Contraseña";
        txtClave.Font = new Font("Segoe UI", 11);
        txtClave.UseSystemPasswordChar = true;
        txtClave.Width = 300;
        txtClave.Location = new Point(40, 255);

        // Botón
        btnIngresar.Text = "INICIAR SESIÓN";
        btnIngresar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnIngresar.BackColor = turquesa;
        btnIngresar.ForeColor = Color.White;
        btnIngresar.FlatStyle = FlatStyle.Flat;
        btnIngresar.FlatAppearance.BorderSize = 0;
        btnIngresar.Size = new Size(300, 42);
        btnIngresar.Location = new Point(40, 315);
        btnIngresar.Click += BtnIngresar_Click;

        // Mensajes de error
        lblMensaje.ForeColor = Color.Salmon;
        lblMensaje.AutoSize = false;
        lblMensaje.Size = new Size(300, 70);
        lblMensaje.Location = new Point(40, 370);

        // Enter = presionar el botón
        AcceptButton = btnIngresar;

        Controls.Add(lblTitulo);
        Controls.Add(lblSubtitulo);
        Controls.Add(lblSesion);
        Controls.Add(txtUsuario);
        Controls.Add(txtClave);
        Controls.Add(btnIngresar);
        Controls.Add(lblMensaje);
    }

    private void BtnIngresar_Click(object? sender, EventArgs e)
    {
        lblMensaje.Text = "";

        string usuario = txtUsuario.Text.Trim();
        string clave = txtClave.Text;

        if (usuario == "" || clave == "")
        {
            lblMensaje.Text = "Escriba el usuario y la contraseña.";
            return;
        }

        try
        {
            if (UsuarioDatos.ValidarLogin(usuario, clave))
            {
                // Cierra el login y le avisa a Program que todo salió bien
                DialogResult = DialogResult.OK;
            }
            else
            {
                lblMensaje.Text = "Usuario o contraseña incorrectos.";
                txtClave.Clear();
                txtClave.Focus();
            }
        }
        catch (Exception ex)
        {
            lblMensaje.Text = "No se pudo conectar a la base de datos: " + ex.Message;
        }
    }
}
