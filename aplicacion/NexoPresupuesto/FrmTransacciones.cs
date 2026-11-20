using System.Data;
using System.Globalization;
using NexoPresupuesto.Datos;

namespace NexoPresupuesto;

// Pantalla para registrar transacciones y anularlas.
// Se arma por código, igual que el login y el panel.
public class FrmTransacciones : Form
{
    private const int AnioActual = 2026;

    // Colores de la identidad visual de NEXO
    private Color azulOscuro = ColorTranslator.FromHtml("#12304A");
    private Color turquesa = ColorTranslator.FromHtml("#12B8B0");
    private Color fondo = ColorTranslator.FromHtml("#F3F6FA");

    private ComboBox cmbPresupuesto = new ComboBox();
    private DateTimePicker dtpFecha = new DateTimePicker();
    private TextBox txtDescripcion = new TextBox();
    private TextBox txtMonto = new TextBox();
    private Button btnRegistrar = new Button();
    private Button btnAnular = new Button();
    private Label lblAviso = new Label();
    private DataGridView gridTransacciones = new DataGridView();

    public FrmTransacciones()
    {
        ConstruirPantalla();
        CargarDatos();
    }

    private void ConstruirPantalla()
    {
        // Ventana
        Text = "NEXO Presupuesto - Transacciones";
        ClientSize = new Size(1000, 620);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = fondo;
        Font = new Font("Segoe UI", 10);

        Label lblTitulo = new Label();
        lblTitulo.Text = "Transacciones";
        lblTitulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        lblTitulo.ForeColor = azulOscuro;
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(18, 10);

        Label lblNueva = CrearSubtitulo("Nueva transacción", 55);

        // Etiquetas del formulario
        Label lblPresupuesto = CrearEtiqueta("Presupuesto (departamento / categoría)", 20, 90);
        Label lblFecha = CrearEtiqueta("Fecha", 420, 90);
        Label lblDescripcion = CrearEtiqueta("Descripción", 560, 90);
        Label lblMonto = CrearEtiqueta("Monto (₡)", 805, 90);

        // Campos
        cmbPresupuesto.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPresupuesto.DropDownWidth = 500;
        cmbPresupuesto.Location = new Point(20, 115);
        cmbPresupuesto.Width = 380;

        dtpFecha.Format = DateTimePickerFormat.Short;
        dtpFecha.Value = DateTime.Today;
        dtpFecha.Location = new Point(420, 115);
        dtpFecha.Width = 125;

        txtDescripcion.MaxLength = 200;
        txtDescripcion.Location = new Point(560, 115);
        txtDescripcion.Width = 230;

        txtMonto.Location = new Point(805, 115);
        txtMonto.Width = 120;
        txtMonto.TextAlign = HorizontalAlignment.Right;

        // Botón registrar
        btnRegistrar.Text = "Registrar transacción";
        btnRegistrar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnRegistrar.BackColor = turquesa;
        btnRegistrar.ForeColor = Color.White;
        btnRegistrar.FlatStyle = FlatStyle.Flat;
        btnRegistrar.FlatAppearance.BorderSize = 0;
        btnRegistrar.Size = new Size(200, 36);
        btnRegistrar.Location = new Point(20, 158);
        btnRegistrar.Click += BtnRegistrar_Click;

        // Aviso si el rol no permite registrar ni anular
        lblAviso.ForeColor = Color.Firebrick;
        lblAviso.AutoSize = true;
        lblAviso.Location = new Point(240, 166);

        Label lblHistorial = CrearSubtitulo("Historial de transacciones", 210);

        ConfigurarGrid(gridTransacciones, 20, 240, 960, 320);

        // Botón anular
        btnAnular.Text = "Anular seleccionada";
        btnAnular.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnAnular.BackColor = Color.Firebrick;
        btnAnular.ForeColor = Color.White;
        btnAnular.FlatStyle = FlatStyle.Flat;
        btnAnular.FlatAppearance.BorderSize = 0;
        btnAnular.Size = new Size(200, 36);
        btnAnular.Location = new Point(20, 572);
        btnAnular.Click += BtnAnular_Click;

        Button btnCerrar = new Button();
        btnCerrar.Text = "Cerrar";
        btnCerrar.BackColor = azulOscuro;
        btnCerrar.ForeColor = Color.White;
        btnCerrar.FlatStyle = FlatStyle.Flat;
        btnCerrar.FlatAppearance.BorderSize = 0;
        btnCerrar.Size = new Size(200, 36);
        btnCerrar.Location = new Point(780, 572);
        btnCerrar.Click += BtnCerrar_Click;

        Controls.Add(lblTitulo);
        Controls.Add(lblNueva);
        Controls.Add(lblPresupuesto);
        Controls.Add(lblFecha);
        Controls.Add(lblDescripcion);
        Controls.Add(lblMonto);
        Controls.Add(cmbPresupuesto);
        Controls.Add(dtpFecha);
        Controls.Add(txtDescripcion);
        Controls.Add(txtMonto);
        Controls.Add(btnRegistrar);
        Controls.Add(lblAviso);
        Controls.Add(lblHistorial);
        Controls.Add(btnAnular);
        Controls.Add(btnCerrar);

        // Solo el administrador puede registrar y anular
        if (Sesion.Rol != "Administrador")
        {
            btnRegistrar.Enabled = false;
            btnAnular.Enabled = false;
            lblAviso.Text = "Su rol (" + Sesion.Rol + ") solo permite consultar las transacciones.";
        }
    }

    private Label CrearSubtitulo(string texto, int y)
    {
        Label lbl = new Label();
        lbl.Text = texto;
        lbl.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        lbl.ForeColor = azulOscuro;
        lbl.AutoSize = true;
        lbl.Location = new Point(20, y);
        return lbl;
    }

    private Label CrearEtiqueta(string texto, int x, int y)
    {
        Label lbl = new Label();
        lbl.Text = texto;
        lbl.ForeColor = Color.Gray;
        lbl.AutoSize = true;
        lbl.Location = new Point(x, y);
        return lbl;
    }

    // Deja la tabla de solo lectura y con los colores de NEXO
    private void ConfigurarGrid(DataGridView grid, int x, int y, int ancho, int alto)
    {
        grid.Location = new Point(x, y);
        grid.Size = new Size(ancho, alto);
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.MultiSelect = false;
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

    // Llena el combo de presupuestos y la tabla de transacciones
    private void CargarDatos()
    {
        try
        {
            // Se guarda la selección para no perderla al recargar
            object? seleccionado = cmbPresupuesto.SelectedValue;

            DataTable presupuestos = TransaccionDatos.ObtenerPresupuestos(AnioActual);
            presupuestos.Columns.Add("Texto", typeof(string));

            foreach (DataRow fila in presupuestos.Rows)
            {
                decimal saldo = Convert.ToDecimal(fila["Saldo"]);
                fila["Texto"] = fila["Presupuesto"] + "  (saldo " + Formato.Colones(saldo) + ")";
            }

            cmbPresupuesto.DisplayMember = "Texto";
            cmbPresupuesto.ValueMember = "IdPresupuesto";
            cmbPresupuesto.DataSource = presupuestos;

            if (seleccionado != null)
            {
                cmbPresupuesto.SelectedValue = seleccionado;
            }

            // Tabla de transacciones
            gridTransacciones.DataSource = TransaccionDatos.ObtenerTodas();
            gridTransacciones.Columns["IdTransaccion"].Visible = false;
            gridTransacciones.Columns["Descripcion"].HeaderText = "Descripción";
            gridTransacciones.Columns["Categoria"].HeaderText = "Categoría";
            gridTransacciones.Columns["MotivoAnulacion"].HeaderText = "Motivo de anulación";
            gridTransacciones.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
            gridTransacciones.Columns["Monto"].DefaultCellStyle.Format = "₡#,##0";
            gridTransacciones.Columns["Monto"].DefaultCellStyle.FormatProvider = Formato.Cultura;
            gridTransacciones.Columns["Monto"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Las anuladas se ven en rojo
            foreach (DataGridViewRow fila in gridTransacciones.Rows)
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

    private void BtnRegistrar_Click(object? sender, EventArgs e)
    {
        // 1. Validar lo que escribió el usuario
        if (cmbPresupuesto.SelectedValue == null)
        {
            MessageBox.Show("Seleccione un presupuesto.", "NEXO Presupuesto",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string descripcion = txtDescripcion.Text.Trim();
        if (descripcion == "")
        {
            MessageBox.Show("Escriba la descripción de la transacción.", "NEXO Presupuesto",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtDescripcion.Focus();
            return;
        }

        // Solo se aceptan montos enteros en colones (se ignoran los puntos y espacios)
        string textoMonto = txtMonto.Text.Replace(".", "").Replace(" ", "").Trim();
        decimal monto;
        if (!decimal.TryParse(textoMonto, NumberStyles.Integer, CultureInfo.InvariantCulture, out monto) || monto <= 0)
        {
            MessageBox.Show("Escriba el monto con números enteros mayores que cero, por ejemplo 850000.",
                "NEXO Presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtMonto.Focus();
            return;
        }

        if (dtpFecha.Value.Year != AnioActual)
        {
            MessageBox.Show("La fecha debe estar dentro del período " + AnioActual + ".",
                "NEXO Presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int idPresupuesto = Convert.ToInt32(cmbPresupuesto.SelectedValue);

        try
        {
            // 2. Validar que alcance el saldo (se consulta de nuevo a la base)
            decimal saldo = TransaccionDatos.ObtenerSaldo(idPresupuesto);
            if (monto > saldo)
            {
                MessageBox.Show("Saldo insuficiente. El saldo disponible de este presupuesto es "
                    + Formato.Colones(saldo) + " y la transacción es de " + Formato.Colones(monto) + ".",
                    "Alerta de presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Guardar
            TransaccionDatos.Registrar(idPresupuesto, dtpFecha.Value, descripcion, monto);

            MessageBox.Show("Transacción registrada correctamente.", "NEXO Presupuesto",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtDescripcion.Clear();
            txtMonto.Clear();
            CargarDatos();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo registrar la transacción: " + ex.Message,
                "NEXO Presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAnular_Click(object? sender, EventArgs e)
    {
        if (gridTransacciones.CurrentRow == null)
        {
            MessageBox.Show("Seleccione una transacción de la tabla.", "NEXO Presupuesto",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DataGridViewRow fila = gridTransacciones.CurrentRow;
        int idTransaccion = Convert.ToInt32(fila.Cells["IdTransaccion"].Value);
        string estado = Convert.ToString(fila.Cells["Estado"].Value) ?? "";
        string descripcion = Convert.ToString(fila.Cells["Descripcion"].Value) ?? "";

        if (estado == "Anulada")
        {
            MessageBox.Show("Esa transacción ya está anulada.", "NEXO Presupuesto",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Se pide el motivo; null significa que el usuario canceló
        string? motivo = PedirMotivo(descripcion);
        if (motivo == null)
        {
            return;
        }

        if (motivo == "")
        {
            MessageBox.Show("Para anular una transacción hay que escribir el motivo.",
                "NEXO Presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            bool anulada = TransaccionDatos.Anular(idTransaccion, motivo);

            if (anulada)
            {
                MessageBox.Show("Transacción anulada. Se conserva en el historial.",
                    "NEXO Presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo anular: la transacción ya estaba anulada.",
                    "NEXO Presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            CargarDatos();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo anular la transacción: " + ex.Message,
                "NEXO Presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Ventanita para escribir el motivo de la anulación.
    // Devuelve el texto, o null si el usuario cancela.
    private string? PedirMotivo(string descripcion)
    {
        Form dialogo = new Form();
        dialogo.Text = "Anular transacción";
        dialogo.ClientSize = new Size(440, 160);
        dialogo.StartPosition = FormStartPosition.CenterParent;
        dialogo.FormBorderStyle = FormBorderStyle.FixedDialog;
        dialogo.MaximizeBox = false;
        dialogo.MinimizeBox = false;

        Label lblTexto = new Label();
        lblTexto.Text = "Motivo de la anulación de \"" + descripcion + "\":";
        lblTexto.AutoSize = false;
        lblTexto.Size = new Size(410, 40);
        lblTexto.Location = new Point(15, 15);

        TextBox txtMotivo = new TextBox();
        txtMotivo.MaxLength = 200;
        txtMotivo.Location = new Point(15, 62);
        txtMotivo.Width = 410;

        Button btnAceptar = new Button();
        btnAceptar.Text = "Anular";
        btnAceptar.DialogResult = DialogResult.OK;
        btnAceptar.Location = new Point(245, 112);
        btnAceptar.Size = new Size(85, 32);

        Button btnCancelar = new Button();
        btnCancelar.Text = "Cancelar";
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(340, 112);
        btnCancelar.Size = new Size(85, 32);

        dialogo.AcceptButton = btnAceptar;
        dialogo.CancelButton = btnCancelar;
        dialogo.Controls.Add(lblTexto);
        dialogo.Controls.Add(txtMotivo);
        dialogo.Controls.Add(btnAceptar);
        dialogo.Controls.Add(btnCancelar);

        if (dialogo.ShowDialog(this) == DialogResult.OK)
        {
            return txtMotivo.Text.Trim();
        }

        return null;
    }

    private void BtnCerrar_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
