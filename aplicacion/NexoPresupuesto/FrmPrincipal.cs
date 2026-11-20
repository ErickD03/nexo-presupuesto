using System.Data;
using NexoPresupuesto.Datos;

namespace NexoPresupuesto;

// Panel de control: tarjetas con los totales, presupuesto por categoría
// y transacciones recientes. Se arma por código, igual que el login.
public class FrmPrincipal : Form
{
    private const int AnioActual = 2026;

    // Colores de la identidad visual de NEXO
    private Color azulOscuro = ColorTranslator.FromHtml("#12304A");
    private Color turquesa = ColorTranslator.FromHtml("#12B8B0");
    private Color fondo = ColorTranslator.FromHtml("#F3F6FA");

    // Controles que se llenan con datos de la base
    private Label lblAprobado = new Label();
    private Label lblGastado = new Label();
    private Label lblSaldo = new Label();
    private Label lblEjecucion = new Label();
    private ProgressBar barraEjecucion = new ProgressBar();
    private DataGridView gridCategorias = new DataGridView();
    private DataGridView gridRecientes = new DataGridView();

    public FrmPrincipal()
    {
        ConstruirPantalla();
        CargarDatos();
    }

    private void ConstruirPantalla()
    {
        // Ventana
        Text = "NEXO Presupuesto";
        ClientSize = new Size(1100, 680);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = fondo;
        Font = new Font("Segoe UI", 10);

        ConstruirMenuLateral();

        // Título del panel
        Label lblTitulo = new Label();
        lblTitulo.Text = "Panel de control";
        lblTitulo.Font = new Font("Segoe UI", 22, FontStyle.Bold);
        lblTitulo.ForeColor = azulOscuro;
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(228, 18);

        Label lblPeriodo = new Label();
        lblPeriodo.Text = "Resumen presupuestario · Período " + AnioActual;
        lblPeriodo.ForeColor = Color.Gray;
        lblPeriodo.AutoSize = true;
        lblPeriodo.Location = new Point(232, 64);

        Controls.Add(lblTitulo);
        Controls.Add(lblPeriodo);

        // Tarjetas
        Controls.Add(CrearTarjeta("Presupuesto aprobado", lblAprobado, 230));
        Controls.Add(CrearTarjeta("Gastos registrados", lblGastado, 445));
        Controls.Add(CrearTarjeta("Saldo disponible", lblSaldo, 660));

        // La tarjeta de ejecución lleva además una barra de progreso
        Panel tarjetaEjecucion = CrearTarjeta("Ejecución presupuestaria", lblEjecucion, 875);
        barraEjecucion.Size = new Size(170, 8);
        barraEjecucion.Location = new Point(12, 74);
        tarjetaEjecucion.Controls.Add(barraEjecucion);
        Controls.Add(tarjetaEjecucion);

        // Presupuesto contra gastos por categoría
        Label lblCategorias = CrearSubtitulo("Presupuesto vs gastos por categoría", 215);
        Controls.Add(lblCategorias);
        ConfigurarGrid(gridCategorias, 230, 245, 840, 150);

        // Transacciones recientes
        Label lblRecientes = CrearSubtitulo("Transacciones recientes", 415);
        Controls.Add(lblRecientes);
        ConfigurarGrid(gridRecientes, 230, 445, 840, 180);
    }

    private void ConstruirMenuLateral()
    {
        Panel menu = new Panel();
        menu.BackColor = azulOscuro;
        menu.Dock = DockStyle.Left;
        menu.Width = 200;

        Label lblNexo = new Label();
        lblNexo.Text = "NEXO";
        lblNexo.Font = new Font("Segoe UI", 22, FontStyle.Bold);
        lblNexo.ForeColor = Color.White;
        lblNexo.AutoSize = true;
        lblNexo.Location = new Point(18, 20);

        Label lblPresupuesto = new Label();
        lblPresupuesto.Text = "PRESUPUESTO";
        lblPresupuesto.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        lblPresupuesto.ForeColor = turquesa;
        lblPresupuesto.AutoSize = true;
        lblPresupuesto.Location = new Point(22, 66);

        // Opción del menú (por ahora solo Inicio)
        Button btnInicio = new Button();
        btnInicio.Text = "Inicio";
        btnInicio.BackColor = turquesa;
        btnInicio.ForeColor = Color.White;
        btnInicio.FlatStyle = FlatStyle.Flat;
        btnInicio.FlatAppearance.BorderSize = 0;
        btnInicio.TextAlign = ContentAlignment.MiddleLeft;
        btnInicio.Padding = new Padding(10, 0, 0, 0);
        btnInicio.Size = new Size(170, 38);
        btnInicio.Location = new Point(15, 120);

        // Quién tiene la sesión abierta
        Label lblUsuario = new Label();
        lblUsuario.Text = Sesion.Nombre + Environment.NewLine + Sesion.Rol;
        lblUsuario.ForeColor = Color.White;
        lblUsuario.AutoSize = false;
        lblUsuario.Size = new Size(170, 45);
        lblUsuario.Location = new Point(15, 560);

        Button btnSalir = new Button();
        btnSalir.Text = "Salir";
        btnSalir.ForeColor = Color.White;
        btnSalir.FlatStyle = FlatStyle.Flat;
        btnSalir.FlatAppearance.BorderColor = Color.White;
        btnSalir.Size = new Size(170, 36);
        btnSalir.Location = new Point(15, 615);
        btnSalir.Click += BtnSalir_Click;

        menu.Controls.Add(lblNexo);
        menu.Controls.Add(lblPresupuesto);
        menu.Controls.Add(btnInicio);
        menu.Controls.Add(lblUsuario);
        menu.Controls.Add(btnSalir);
        Controls.Add(menu);
    }

    // Crea una tarjeta blanca con un título y un valor
    private Panel CrearTarjeta(string titulo, Label lblValor, int x)
    {
        Panel tarjeta = new Panel();
        tarjeta.BackColor = Color.White;
        tarjeta.Size = new Size(195, 90);
        tarjeta.Location = new Point(x, 100);

        Label lblTituloTarjeta = new Label();
        lblTituloTarjeta.Text = titulo;
        lblTituloTarjeta.ForeColor = Color.Gray;
        lblTituloTarjeta.AutoSize = true;
        lblTituloTarjeta.Location = new Point(12, 12);

        lblValor.Text = "...";
        lblValor.Font = new Font("Segoe UI", 15, FontStyle.Bold);
        lblValor.ForeColor = azulOscuro;
        lblValor.AutoSize = true;
        lblValor.Location = new Point(12, 38);

        tarjeta.Controls.Add(lblTituloTarjeta);
        tarjeta.Controls.Add(lblValor);
        return tarjeta;
    }

    private Label CrearSubtitulo(string texto, int y)
    {
        Label lbl = new Label();
        lbl.Text = texto;
        lbl.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        lbl.ForeColor = azulOscuro;
        lbl.AutoSize = true;
        lbl.Location = new Point(230, y);
        return lbl;
    }

    // Deja las tablas de solo lectura y con los colores de NEXO
    private void ConfigurarGrid(DataGridView grid, int x, int y, int ancho, int alto)
    {
        grid.Location = new Point(x, y);
        grid.Size = new Size(ancho, alto);
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.RowTemplate.Height = 28;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersHeight = 32;
        grid.ColumnHeadersDefaultCellStyle.BackColor = azulOscuro;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.DefaultCellStyle.SelectionBackColor = turquesa;
        grid.DefaultCellStyle.SelectionForeColor = Color.White;
        Controls.Add(grid);
    }

    // Pide los datos a la base y los muestra
    private void CargarDatos()
    {
        try
        {
            // Tarjetas
            ResumenPanel resumen = PanelDatos.ObtenerResumen(AnioActual);
            lblAprobado.Text = Formato.Colones(resumen.Aprobado);
            lblGastado.Text = Formato.Colones(resumen.Gastado);
            lblSaldo.Text = Formato.Colones(resumen.Saldo);
            lblEjecucion.Text = resumen.Ejecucion.ToString("0", Formato.Cultura) + " %";

            int porcentaje = (int)Math.Round(resumen.Ejecucion);
            if (porcentaje > 100)
            {
                porcentaje = 100;
            }
            barraEjecucion.Value = porcentaje;

            // Presupuesto contra gastos por categoría
            gridCategorias.DataSource = PanelDatos.ObtenerPorCategoria(AnioActual);
            gridCategorias.Columns["Categoria"].HeaderText = "Categoría";
            DarFormatoColones(gridCategorias, "Aprobado", "Gastado", "Saldo");

            // Transacciones recientes
            gridRecientes.DataSource = PanelDatos.ObtenerRecientes();
            gridRecientes.Columns["Descripcion"].HeaderText = "Descripción";
            gridRecientes.Columns["Categoria"].HeaderText = "Categoría";
            gridRecientes.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
            DarFormatoColones(gridRecientes, "Monto");

            // Las anuladas se ven en rojo
            foreach (DataGridViewRow fila in gridRecientes.Rows)
            {
                string estado = Convert.ToString(fila.Cells["Estado"].Value) ?? "";
                if (estado == "Anulada")
                {
                    fila.DefaultCellStyle.ForeColor = Color.Firebrick;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudieron cargar los datos: " + ex.Message,
                "NEXO Presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Muestra las columnas indicadas como colones, alineadas a la derecha
    private void DarFormatoColones(DataGridView grid, params string[] columnas)
    {
        foreach (string nombre in columnas)
        {
            grid.Columns[nombre].DefaultCellStyle.Format = "₡#,##0";
            grid.Columns[nombre].DefaultCellStyle.FormatProvider = Formato.Cultura;
            grid.Columns[nombre].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
    }

    private void BtnSalir_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
