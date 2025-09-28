//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Registro y ejecucion de batallas; actualiza ganador y bolsas de cristales

using System;
using System.Windows.Forms;
using Miticax.Datos;
using Miticax.Logica;
using Miticax.Entidades;

namespace Miticax.Presentacion
{
    public class FrmBatallas : Form
    {
        private Label lblJ1; private ComboBox cboJ1;
        private Label lblE1; private ComboBox cboE1;
        private Label lblJ2; private ComboBox cboJ2;
        private Label lblE2; private ComboBox cboE2;
        private Button btnRegistrar; private Button btnEjecutar; private Button btnCerrar;
        private DataGridView grid;

        public FrmBatallas()
        {
            Text = "Batallas - Configurar y Ejecutar";
            Width = 1000;
            Height = 600;

            lblJ1 = new Label() { Text = "Jugador 1:", Left = 20, Top = 20, Width = 80 };
            cboJ1 = new ComboBox() { Left = 100, Top = 16, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 0 };
            cboJ1.SelectedIndexChanged += (s, e) => CargarEquipos(cboJ1, cboE1);

            lblE1 = new Label() { Text = "Equipo 1:", Left = 360, Top = 20, Width = 80 };
            cboE1 = new ComboBox() { Left = 440, Top = 16, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 1 };

            lblJ2 = new Label() { Text = "Jugador 2:", Left = 20, Top = 56, Width = 80 };
            cboJ2 = new ComboBox() { Left = 100, Top = 52, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 2 };
            cboJ2.SelectedIndexChanged += (s, e) => CargarEquipos(cboJ2, cboE2);

            lblE2 = new Label() { Text = "Equipo 2:", Left = 360, Top = 56, Width = 80 };
            cboE2 = new ComboBox() { Left = 440, Top = 52, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 3 };

            btnRegistrar = new Button() { Text = "Registrar Batalla", Left = 660, Top = 15, Width = 150, TabIndex = 4 };
            btnEjecutar = new Button() { Text = "Ejecutar Batalla", Left = 820, Top = 15, Width = 150, TabIndex = 5 };
            btnCerrar = new Button() { Text = "Cerrar", Left = 820, Top = 50, Width = 150, TabIndex = 6 };

            AcceptButton = btnRegistrar;
            CancelButton = btnCerrar;

            btnRegistrar.Click += BtnRegistrar_Click;
            btnEjecutar.Click += BtnEjecutar_Click;
            btnCerrar.Click += (s, e) => Close();

            grid = new DataGridView();
            grid.Left = 20;
            grid.Top = 100;
            grid.Width = 950;
            grid.Height = 440;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoGenerateColumns = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdBatalla", DataPropertyName = "IdBatalla", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdJugador1", DataPropertyName = "IdJugador1", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdEquipo1", DataPropertyName = "IdEquipo1", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdJugador2", DataPropertyName = "IdJugador2", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdEquipo2", DataPropertyName = "IdEquipo2", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Ganador", DataPropertyName = "Ganador", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Fecha", DataPropertyName = "Fecha", Width = 140 });

            Controls.AddRange(new Control[] { lblJ1, cboJ1, lblE1, cboE1, lblJ2, cboJ2, lblE2, cboE2, btnRegistrar, btnEjecutar, btnCerrar, grid });

            CargarJugadores(cboJ1);
            CargarJugadores(cboJ2);
            Refrescar();
        }

        private void CargarJugadores(ComboBox combo)
        {
            try
            {
                var jugadores = JugadorDatos.GetAllSnapshot();
                combo.Items.Clear();
                for (int i = 0; i < jugadores.Length; i++)
                {
                    if (jugadores[i] == null) continue;
                    combo.Items.Add(jugadores[i].IdJugador + " - " + jugadores[i].Nombre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando jugadores.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEquipos(ComboBox comboJugador, ComboBox comboEquipo)
        {
            try
            {
                comboEquipo.Items.Clear();
                if (comboJugador.SelectedIndex < 0) return;
                int idJugador = ParseLeadingInt(comboJugador.SelectedItem.ToString());

                var equipos = EquipoDatos.GetByJugadorSnapshot(idJugador);
                for (int i = 0; i < equipos.Length; i++)
                {
                    if (equipos[i] == null) continue;
                    comboEquipo.Items.Add(equipos[i].IdEquipo + " - Equipo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando equipos.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboJ1.SelectedIndex < 0 || cboE1.SelectedIndex < 0 || cboJ2.SelectedIndex < 0 || cboE2.SelectedIndex < 0)
                {
                    MessageBox.Show("Debe seleccionar jugadores y equipos.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int j1 = ParseLeadingInt(cboJ1.SelectedItem.ToString());
                int e1 = ParseLeadingInt(cboE1.SelectedItem.ToString());
                int j2 = ParseLeadingInt(cboJ2.SelectedItem.ToString());
                int e2 = ParseLeadingInt(cboE2.SelectedItem.ToString());

                string error;
                var r = BatallaService.RegistrarBatalla(j1, e1, j2, e2, out error);
                if (!r.Exito)
                {
                    MessageBox.Show(r.Mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("El registro se ha ingresado correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Refrescar();
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("No se pueden ingresar mas registros", "Limite", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar batalla.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEjecutar_Click(object sender, EventArgs e)
        {
            try
            {
                // Ejecuta la ultima batalla registrada o la seleccionada en grid (si implementaste seleccion)
                int idBatalla = BatallaService.UltimaBatallaRegistradaId();
                string error;
                var r = BatallaService.EjecutarBatalla(idBatalla, out error);
                if (!r.Exito)
                {
                    MessageBox.Show(r.Mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("El registro se ha ingresado correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Refrescar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar batalla.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ParseLeadingInt(string s)
        {
            int guion = s.IndexOf(" - ");
            string num = guion > 0 ? s.Substring(0, guion) : s;
            int v; if (int.TryParse(num, out v)) return v;
            return 0;
        }

        private void Refrescar()
        {
            try
            {
                grid.DataSource = BatallaDatos.GetAllSnapshot();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar batallas.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
