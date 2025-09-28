//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Formulario de menu principal para navegar a todas las opciones

using System;
using System.Windows.Forms;

namespace Miticax.Presentacion
{
    public class FrmMenuPrincipal : Form
    {
        private Button btnCriaturas;
        private Button btnJugadores;
        private Button btnInventario;
        private Button btnEquipos;
        private Button btnBatallas;
        private Button btnRondas;
        private Button btnTop10;
        private FlowLayoutPanel panel;

        public FrmMenuPrincipal()
        {
            // Configuracion basica de ventana
            Text = "MiticaX - Menu Principal";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 520;
            Height = 360;

            FormBorderStyle = FormBorderStyle.FixedDialog; // evita resize
            MaximizeBox = false; MinimizeBox = false;      // limpia la UI
            KeyPreview = true;                             // permitir Esc para cerrar si lo deseas
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };


            // Panel contenedor
            panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            panel.AutoScroll = true;
            Controls.Add(panel);

            // Crear botones
            btnCriaturas = CrearBoton("Criaturas", (s, e) => Abrir(new FrmCriaturas()));
            btnJugadores = CrearBoton("Jugadores", (s, e) => Abrir(new FrmJugadores()));
            btnInventario = CrearBoton("Inventario (compras)", (s, e) => Abrir(new FrmInventario()));
            btnEquipos = CrearBoton("Equipos", (s, e) => Abrir(new FrmEquipos()));
            btnBatallas = CrearBoton("Batallas (configurar y ejecutar)", (s, e) => Abrir(new FrmBatallas()));
            btnRondas = CrearBoton("Rondas (consulta)", (s, e) => Abrir(new FrmRondas()));
            btnTop10 = CrearBoton("Top 10", (s, e) => Abrir(new FrmTop10()));

            // fijar TabIndex en el orden de creación
            btnCriaturas.TabIndex = 0;
            btnJugadores.TabIndex = 1;
            btnInventario.TabIndex = 2;
            btnEquipos.TabIndex = 3;
            btnBatallas.TabIndex = 4;
            btnRondas.TabIndex = 5;
            btnTop10.TabIndex = 6;

            // Agregar al panel
            panel.Controls.AddRange(new Control[]
            {
                btnCriaturas, btnJugadores, btnInventario, btnEquipos, btnBatallas, btnRondas, btnTop10
            });
        }

        private Button CrearBoton(string texto, EventHandler onClick)
        {
            var b = new Button();
            b.Text = texto;
            b.Width = 440;
            b.Height = 40;
            b.Margin = new Padding(5);
            b.Click += onClick;
            return b;
        }

        private void Abrir(Form frm)
        {
            // Abrir como dialogo modal, con try/catch para robustez
            try
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al abrir la ventana.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                frm.Dispose();
            }
        }
    }
}
