//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Consulta de Top 10 jugadores por batallas ganadas

using System;
using System.Windows.Forms;

namespace Miticax.Presentacion
{
    public class FrmTop10 : Form
    {
        private DataGridView grid;
        private Button btnRefrescar; private Button btnCerrar;

        public FrmTop10()
        {
            Text = "Top 10 Ganadores";
            Width = 720;
            Height = 520;

            btnRefrescar = new Button() { Text = "Refrescar", Left = 20, Top = 16, Width = 120, TabIndex = 0 };
            btnCerrar = new Button() { Text = "Cerrar", Left = 150, Top = 16, Width = 120, TabIndex = 1 };

            AcceptButton = btnRefrescar;
            CancelButton = btnCerrar;

            btnRefrescar.Click += (s, e) => Cargar();
            btnCerrar.Click += (s, e) => Close();

            grid = new DataGridView();
            grid.Left = 20;
            grid.Top = 60;
            grid.Width = 660;
            grid.Height = 400;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoGenerateColumns = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdJugador", DataPropertyName = "IdJugador", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Nombre", DataPropertyName = "Nombre", Width = 160 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Fecha Nacimiento", DataPropertyName = "FechaNacimiento", Width = 130 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Nivel", DataPropertyName = "Nivel", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Cristales", DataPropertyName = "Cristales", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Batallas Ganadas", DataPropertyName = "BatallasGanadas", Width = 120 });

            Controls.AddRange(new Control[] { btnRefrescar, btnCerrar, grid });

            // Carga inicial
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                // Usa helper que intenta Top10(), Top100() o Top(10) por reflexion
                var arr = UiServiciosHelper.TopRanking10();
                grid.DataSource = arr;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar Top 10.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
