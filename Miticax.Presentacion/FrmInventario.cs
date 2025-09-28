//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Compra de criaturas al inventario del jugador; valida cristales y refresca grid

using System;
using System.Windows.Forms;

namespace Miticax.Presentacion
{
    public class FrmInventario : Form
    {
        // Instancias de capa Datos via helper (evita CS0120)
        private readonly Miticax.Datos.JugadorDatos _jugadorDatos = UiServiciosHelper.JugadorDatos();
        private readonly Miticax.Datos.CriaturaDatos _criaturaDatos = UiServiciosHelper.CriaturaDatos();
        private readonly Miticax.Datos.InventarioDatos _inventarioDatos = UiServiciosHelper.InventarioDatos();

        private Label lblJugador; private ComboBox cboJugador;
        private Label lblCriatura; private ComboBox cboCriatura;
        private Button btnComprar; private Button btnCerrar;
        private DataGridView grid;

        public FrmInventario()
        {
            Text = "Inventario (compras)";
            Width = 820;
            Height = 540;

            lblJugador = new Label() { Text = "Jugador:", Left = 20, Top = 20, Width = 80 };
            cboJugador = new ComboBox() { Left = 90, Top = 16, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 0 };

            lblCriatura = new Label() { Text = "Criatura:", Left = 360, Top = 20, Width = 80 };
            cboCriatura = new ComboBox() { Left = 430, Top = 16, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 1 };

            btnComprar = new Button() { Text = "Comprar", Left = 680, Top = 15, Width = 100, TabIndex = 2 };
            btnCerrar = new Button() { Text = "Cerrar", Left = 680, Top = 50, Width = 100, TabIndex = 3 };

            AcceptButton = btnComprar;
            CancelButton = btnCerrar;

            btnComprar.Click += BtnComprar_Click;
            btnCerrar.Click += (s, e) => Close();

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

            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdJugador", DataPropertyName = "IdJugador", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IdCriatura", DataPropertyName = "IdCriatura", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Poder", DataPropertyName = "Poder", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Resistencia", DataPropertyName = "Resistencia", Width = 100 });

            Controls.AddRange(new Control[] { lblJugador, cboJugador, lblCriatura, cboCriatura, btnComprar, btnCerrar, grid });

            CargarCombos();
            RefrescarGrid();
        }

        private void CargarCombos()
        {
            try
            {
                // Jugadores
                var jugadores = _jugadorDatos.GetAllSnapshot();
                cboJugador.Items.Clear();
                for (int i = 0; i < jugadores.Length; i++)
                {
                    if (jugadores[i] == null) continue;
                    cboJugador.Items.Add(jugadores[i].IdJugador + " - " + jugadores[i].Nombre);
                }

                // Criaturas del catalogo
                var criaturas = _criaturaDatos.GetAllSnapshot();
                cboCriatura.Items.Clear();
                for (int i = 0; i < criaturas.Length; i++)
                {
                    if (criaturas[i] == null) continue;
                    cboCriatura.Items.Add(criaturas[i].IdCriatura + " - " + criaturas[i].Nombre + " (Costo " + criaturas[i].Costo + ")");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando combos.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnComprar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboJugador.SelectedIndex < 0 || cboCriatura.SelectedIndex < 0)
                {
                    MessageBox.Show("Debe seleccionar jugador y criatura.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idJugador = ParseLeadingInt(cboJugador.SelectedItem.ToString());
                int idCriatura = ParseLeadingInt(cboCriatura.SelectedItem.ToString());

                // Invocar InventarioService.ComprarCriatura por reflexion (soporta firmas comunes)
                bool exito = false;
                string msg = null;

                var t = Type.GetType("Miticax.Logica.InventarioService, Miticax.Logica");
                object ro = null;
                if (t != null)
                {
                    // Preferido: ComprarCriatura(int,int,out string)
                    var m1 = t.GetMethod("ComprarCriatura", new Type[] { typeof(int), typeof(int), typeof(string).MakeByRefType() });
                    if (m1 != null)
                    {
                        object[] pars = new object[] { idJugador, idCriatura, null };
                        ro = m1.Invoke(null, pars);
                        msg = UiServiciosHelper.ExtraerMensaje(ro) ?? (pars[2] as string);
                    }
                    else
                    {
                        // Alternativa: ComprarCriatura(int,int)
                        var m2 = t.GetMethod("ComprarCriatura", new Type[] { typeof(int), typeof(int) });
                        if (m2 != null) ro = m2.Invoke(null, new object[] { idJugador, idCriatura });
                    }
                }

                if (ro != null)
                {
                    var pEx = ro.GetType().GetProperty("Exito");
                    if (pEx != null) exito = (bool)(pEx.GetValue(ro) ?? false);
                    msg = UiServiciosHelper.ExtraerMensaje(ro) ?? msg;
                }

                if (!exito)
                {
                    // Mensaje exacto si lo provee la capa logica (incluye el de cristales insuficientes)
                    MessageBox.Show(string.IsNullOrWhiteSpace(msg) ? "Operacion no completada" : msg, "Operacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                MessageBox.Show("El registro se ha ingresado correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Requisito: limpiar campos tras agregar
                cboJugador.SelectedIndex = -1;
                cboCriatura.SelectedIndex = -1;
                cboJugador.Focus();

                RefrescarGrid();
            }
            catch (IndexOutOfRangeException)
            {
                // Limite de arreglo alcanzado
                MessageBox.Show("No se pueden ingresar mas registros", "Limite", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al comprar la criatura.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ParseLeadingInt(string s)
        {
            // Obtiene el entero al inicio (antes de " - ")
            int guion = s.IndexOf(" - ");
            string num = guion > 0 ? s.Substring(0, guion) : s;
            int val; if (int.TryParse(num, out val)) return val;
            return 0;
        }

        private void RefrescarGrid()
        {
            try
            {
                var inv = _inventarioDatos.GetAllSnapshot();
                grid.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al cargar inventario.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
