//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Consulta de rondas por IdBatalla

using System;
using System.Windows.Forms;

namespace Miticax.Presentacion
{
    public class FrmRondas : Form
    {
        // Instancia de Datos via helper (metodos de instancia)
        private readonly Miticax.Datos.RondaDatos _rondaDatos = UiServiciosHelper.RondaDatos();

        private Label lblBatalla; private TextBox txtBatalla;
        private Button btnBuscar; private Button btnCerrar;
        private DataGridView grid;

        public FrmRondas()
        {
            Text = "Rondas - Consulta por Batalla";
            Width = 820;
            Height = 520;

            lblBatalla = new Label() { Text = "IdBatalla:", Left = 20, Top = 20, Width = 80 };
            txtBatalla = new TextBox() { Left = 100, Top = 16, Width = 120, TabIndex = 0 };

            btnBuscar = new Button() { Text = "Buscar", Left = 240, Top = 15, Width = 100, TabIndex = 1 };
            btnCerrar = new Button() { Text = "Cerrar", Left = 350, Top = 15, Width = 100, TabIndex = 2 };

            AcceptButton = btnBuscar;
            CancelButton = btnCerrar;

            btnBuscar.Click += BtnBuscar_Click;
            btnCerrar.Click += (s, e) => Close();

            grid = new DataGridView();
            grid.Left = 20;
            grid.Top = 60;
            grid.Width = 760;
            grid.Height = 400;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoGenerateColumns = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Ronda", DataPropertyName = "IdRonda", Width = 70 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdBatalla", DataPropertyName = "IdBatalla", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdJugador1", DataPropertyName = "IdJugador1", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdCriatura1", DataPropertyName = "IdCriatura1", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdJugador2", DataPropertyName = "IdJugador2", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdCriatura2", DataPropertyName = "IdCriatura2", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "GanadorRonda", DataPropertyName = "GanadorRonda", Width = 120 });

            Controls.AddRange(new Control[] { lblBatalla, txtBatalla, btnBuscar, btnCerrar, grid });
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (!int.TryParse(txtBatalla.Text.Trim(), out id) || id <= 0)
                {
                    MessageBox.Show("IdBatalla invalido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tomamos todas las rondas y filtramos por IdBatalla (sin LINQ)
                var todas = _rondaDatos.GetAllSnapshot();
                var filtradas = UiServiciosHelper.FiltrarPorCampoIgual(todas, "IdBatalla", id);
                grid.DataSource = filtradas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar rondas.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
