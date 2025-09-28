//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Form basico de jugadores + grid; aplica reglas UI y mensajes

using System;
using System.Windows.Forms;
using Miticax.Entidades;

namespace Miticax.Presentacion
{
    public class FrmJugadores : Form
    {
        // Instancia de datos via helper para evitar metodos estaticos
        private readonly Miticax.Datos.JugadorDatos _jugadorDatos = UiServiciosHelper.JugadorDatos();

        private Label lblId; private TextBox txtId;
        private Label lblNombre; private TextBox txtNombre;
        private Label lblFecha; private DateTimePicker dtpFecha;
        private Button btnAgregar; private Button btnCerrar;
        private DataGridView grid;

        public FrmJugadores()
        {
            Text = "Jugadores - Registrar y Consultar";
            Width = 820;
            Height = 540;
            StartPosition = FormStartPosition.CenterParent;

            // Controles de entrada
            lblId = new Label() { Text = "IdJugador:", Left = 20, Top = 20, Width = 100 };
            txtId = new TextBox() { Left = 120, Top = 16, Width = 120, TabIndex = 0 };

            lblNombre = new Label() { Text = "Nombre:", Left = 260, Top = 20, Width = 80 };
            txtNombre = new TextBox() { Left = 330, Top = 16, Width = 200, TabIndex = 1 };

            lblFecha = new Label() { Text = "Fecha Nac.:", Left = 540, Top = 20, Width = 90 };
            dtpFecha = new DateTimePicker() { Left = 630, Top = 16, Width = 150, TabIndex = 2, Format = DateTimePickerFormat.Short };

            btnAgregar = new Button() { Text = "Agregar", Left = 500, Top = 56, Width = 120, TabIndex = 3 };
            btnCerrar = new Button() { Text = "Cerrar", Left = 630, Top = 56, Width = 120, TabIndex = 4 };

            AcceptButton = btnAgregar;
            CancelButton = btnCerrar;

            btnAgregar.Click += BtnAgregar_Click;
            btnCerrar.Click += (s, e) => Close();

            // Grid
            grid = new DataGridView();
            grid.Left = 20;
            grid.Top = 100;
            grid.Width = 760;
            grid.Height = 380;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoGenerateColumns = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Id", DataPropertyName = "IdJugador", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Nombre", DataPropertyName = "Nombre", Width = 180 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Fecha Nac.", DataPropertyName = "FechaNacimiento", Width = 110 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Nivel", DataPropertyName = "Nivel", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Cristales", DataPropertyName = "Cristales", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Batallas Ganadas", DataPropertyName = "BatallasGanadas", Width = 130 });

            Controls.AddRange(new Control[] { lblId, txtId, lblNombre, txtNombre, lblFecha, dtpFecha, btnAgregar, btnCerrar, grid });

            // Cargar datos iniciales
            Refrescar();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (!int.TryParse(txtId.Text.Trim(), out id) || id <= 0)
                {
                    MessageBox.Show("IdJugador invalido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("El nombre es requerido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var ent = new Miticax.Entidades.JugadorEntidad();
                ent.IdJugador = id;
                ent.Nombre = txtNombre.Text.Trim();
                ent.FechaNacimiento = dtpFecha.Value;

                // Llamar al servicio real (de instancia) construido en UiServiciosHelper
                var srv = UiServiciosHelper.JugadorService();
                string error;
                var resultado = srv.RegistrarJugador(ent, ent.FechaNacimiento, out error);

                bool exito = (bool)(resultado.GetType().GetProperty("Exito")?.GetValue(resultado) ?? false);
                string msg = UiServiciosHelper.ExtraerMensaje(resultado) ?? error;

                if (!exito)
                {
                    MessageBox.Show(string.IsNullOrWhiteSpace(msg) ? "Operacion no completada" : msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("El registro se ha ingresado correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtId.Clear(); txtNombre.Clear(); dtpFecha.Value = DateTime.Today;
                txtId.Focus();
                Refrescar();
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("No se pueden ingresar mas registros", "Limite", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al registrar el jugador.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Refrescar()
        {
            try
            {
                // Llama al snapshot mediante la instancia (no estatico)
                var arr = _jugadorDatos.GetAllSnapshot();
                grid.DataSource = arr;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al cargar jugadores.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
