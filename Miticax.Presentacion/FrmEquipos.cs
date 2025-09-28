//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Registro de equipos: jugador + 3 criaturas de su inventario

using System;
using System.Windows.Forms;
using Miticax.Logica;
using Miticax.Datos;
using Miticax.Entidades;

namespace Miticax.Presentacion
{
    public class FrmEquipos : Form
    {
        private Label lblJugador; private ComboBox cboJugador;
        private Label lblC1; private ComboBox cboC1;
        private Label lblC2; private ComboBox cboC2;
        private Label lblC3; private ComboBox cboC3;
        private Button btnRegistrar; private Button btnCerrar;
        private DataGridView grid;

        public FrmEquipos()
        {
            Text = "Equipos - Registrar y Consultar";
            Width = 900;
            Height = 560;

            lblJugador = new Label() { Text = "Jugador:", Left = 20, Top = 20, Width = 70 };
            cboJugador = new ComboBox() { Left = 90, Top = 16, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 0 };
            cboJugador.SelectedIndexChanged += (s, e) => CargarCriaturasInventario();

            lblC1 = new Label() { Text = "Criatura 1:", Left = 350, Top = 20, Width = 80 };
            cboC1 = new ComboBox() { Left = 430, Top = 16, Width = 160, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 1 };

            lblC2 = new Label() { Text = "Criatura 2:", Left = 600, Top = 20, Width = 80 };
            cboC2 = new ComboBox() { Left = 680, Top = 16, Width = 160, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 2 };

            lblC3 = new Label() { Text = "Criatura 3:", Left = 350, Top = 56, Width = 80 };
            cboC3 = new ComboBox() { Left = 430, Top = 52, Width = 160, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 3 };

            btnRegistrar = new Button() { Text = "Registrar", Left = 680, Top = 52, Width = 160, TabIndex = 4 };
            btnCerrar = new Button() { Text = "Cerrar", Left = 20, Top = 52, Width = 120, TabIndex = 5 };

            AcceptButton = btnRegistrar;
            CancelButton = btnCerrar;

            btnRegistrar.Click += BtnRegistrar_Click;
            btnCerrar.Click += (s, e) => Close();

            grid = new DataGridView();
            grid.Left = 20;
            grid.Top = 100;
            grid.Width = 820;
            grid.Height = 400;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoGenerateColumns = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdEquipo", DataPropertyName = "IdEquipo", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdJugador", DataPropertyName = "IdJugador", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdCriatura1", DataPropertyName = "IdCriatura1", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdCriatura2", DataPropertyName = "IdCriatura2", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdCriatura3", DataPropertyName = "IdCriatura3", Width = 90 });

            Controls.AddRange(new Control[] { lblJugador, cboJugador, lblC1, cboC1, lblC2, cboC2, lblC3, cboC3, btnRegistrar, btnCerrar, grid });

            CargarJugadores();
            RefrescarGrid();
        }

        private void CargarJugadores()
        {
            try
            {
                var jugadores = JugadorDatos.GetAllSnapshot();
                cboJugador.Items.Clear();
                for (int i = 0; i < jugadores.Length; i++)
                {
                    if (jugadores[i] == null) continue;
                    cboJugador.Items.Add(jugadores[i].IdJugador + " - " + jugadores[i].Nombre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando jugadores.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarCriaturasInventario()
        {
            try
            {
                cboC1.Items.Clear(); cboC2.Items.Clear(); cboC3.Items.Clear();
                if (cboJugador.SelectedIndex < 0) return;

                int idJugador = ParseLeadingInt(cboJugador.SelectedItem.ToString());
                var inv = InventarioDatos.GetByJugadorSnapshot(idJugador);
                for (int i = 0; i < inv.Length; i++)
                {
                    if (inv[i] == null) continue;
                    string texto = inv[i].IdCriatura + " - Poder " + inv[i].Poder + " / Res " + inv[i].Resistencia;
                    cboC1.Items.Add(texto);
                    cboC2.Items.Add(texto);
                    cboC3.Items.Add(texto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando inventario del jugador.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboJugador.SelectedIndex < 0 || cboC1.SelectedIndex < 0 || cboC2.SelectedIndex < 0 || cboC3.SelectedIndex < 0)
                {
                    MessageBox.Show("Debe seleccionar jugador y 3 criaturas.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idJugador = ParseLeadingInt(cboJugador.SelectedItem.ToString());
                int c1 = ParseLeadingInt(cboC1.SelectedItem.ToString());
                int c2 = ParseLeadingInt(cboC2.SelectedItem.ToString());
                int c3 = ParseLeadingInt(cboC3.SelectedItem.ToString());

                string error;
                var r = EquipoService.Registrar(idJugador, c1, c2, c3, out error);
                if (!r.Exito)
                {
                    MessageBox.Show(r.Mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("El registro se ha ingresado correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                cboJugador.SelectedIndex = -1; cboC1.SelectedIndex = -1; cboC2.SelectedIndex = -1; cboC3.SelectedIndex = -1;
                cboJugador.Focus();
                RefrescarGrid();
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("No se pueden ingresar mas registros", "Limite", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar equipo.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ParseLeadingInt(string s)
        {
            int guion = s.IndexOf(" - ");
            string num = guion > 0 ? s.Substring(0, guion) : s;
            int v; if (int.TryParse(num, out v)) return v;
            return 0;
        }

        private void RefrescarGrid()
        {
            try
            {
                var arr = EquipoDatos.GetAllSnapshot();
                grid.DataSource = arr;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar equipos.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
