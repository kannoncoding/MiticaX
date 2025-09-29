//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Form criaturas + grid; aplica reglas UI, mensajes y mapeo de descripciones

using System;
using System.Windows.Forms;
using Miticax.Entidades;
using Miticax.Logica;

namespace Miticax.Presentacion
{
    public class FrmCriaturas : Form
    {
        // Instancia de datos via helper
        private readonly Miticax.Datos.CriaturaDatos _criaturaDatos = UiServiciosHelper.CriaturaDatos();

        // Controles
        private Label lblId; private TextBox txtId;
        private Label lblNombre; private TextBox txtNombre;
        private Label lblTipo; private ComboBox cboTipo;
        private Label lblNivel; private ComboBox cboNivel;
        private Label lblPoder; private NumericUpDown nudPoder;
        private Label lblResistencia; private NumericUpDown nudResistencia;
        private Label lblCosto; private NumericUpDown nudCosto;

        private Button btnAgregar; private Button btnCerrar;
        private DataGridView grid;

        public FrmCriaturas()
        {
            // Configuracion general del formulario
            Text = "Criaturas - Registrar y Consultar";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;

            // Crear controles de entrada
            // ----- Fila 1: Id, Nombre, Tipo -----
            lblId = new Label() { Text = "IdCriatura:", Left = 20, Top = 20, Width = 100 };
            txtId = new TextBox() { Left = 120, Top = 20, Width = 120, TabIndex = 0 };

            lblNombre = new Label() { Text = "Nombre:", Left = 260, Top = 20, Width = 80 };
            txtNombre = new TextBox() { Left = 340, Top = 20, Width = 160, TabIndex = 1 };

            lblTipo = new Label() { Text = "Tipo:", Left = 520, Top = 20, Width = 60 };
            cboTipo = new ComboBox()
            {
                Left = 600,
                Top = 20,
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 2
            };
            cboTipo.Items.AddRange(new object[] { "agua", "tierra", "aire", "fuego" });

            // ----- Fila 2: Nivel, Poder, Resistencia, Costo -----
            lblNivel = new Label() { Text = "Nivel:", Left = 20, Top = 60, Width = 60 };
            cboNivel = new ComboBox()
            {
                Left = 90,
                Top = 60,
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 3
            };
            cboNivel.Items.AddRange(new object[] { "01-Iniciado", "02-Aprendiz", "03-Estudiante", "04-Avanzado", "05-Maestro" });

            lblPoder = new Label() { Text = "Poder:", Left = 270, Top = 60, Width = 60 };
            nudPoder = new NumericUpDown() { Left = 330, Top = 60, Width = 80, Minimum = 0, Maximum = 100000, TabIndex = 4 };

            lblResistencia = new Label() { Text = "Resistencia:", Left = 430, Top = 60, Width = 90 };
            nudResistencia = new NumericUpDown() { Left = 525, Top = 60, Width = 80, Minimum = 0, Maximum = 100000, TabIndex = 5 };

            lblCosto = new Label() { Text = "Costo (Cristales):", Left = 620, Top = 60, Width = 120 };
            nudCosto = new NumericUpDown() { Left = 745, Top = 60, Width = 80, Minimum = 0, Maximum = 1000000, TabIndex = 6 };

            // ----- Botones (misma fila 2, a la derecha) -----
            btnAgregar = new Button() { Text = "Agregar", Left = 840, Top = 58, Width = 100, TabIndex = 7 };
            btnCerrar = new Button() { Text = "Cerrar", Left = 945, Top = 58, Width = 100, TabIndex = 8 };

            AcceptButton = btnAgregar;
            CancelButton = btnCerrar;

            btnAgregar.Click += BtnAgregar_Click;
            btnCerrar.Click += (s, e) => Close();

            // DataGridView
            grid = new DataGridView
            {
                Left = 20,
                Top = 100,
                Width = 1025,
                Height = 420,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false
            };

            // asegurar anclaje arriba-izquierda para todos
            lblId.Anchor = txtId.Anchor = lblNombre.Anchor = txtNombre.Anchor =
            lblTipo.Anchor = cboTipo.Anchor = lblNivel.Anchor = cboNivel.Anchor =
            lblPoder.Anchor = nudPoder.Anchor = lblResistencia.Anchor = nudResistencia.Anchor =
            lblCosto.Anchor = nudCosto.Anchor = btnAgregar.Anchor = btnCerrar.Anchor =
            AnchorStyles.Top | AnchorStyles.Left;

            // Columnas manuales
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Id", DataPropertyName = "IdCriatura", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Nombre", DataPropertyName = "Nombre", Width = 140 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Tipo", DataPropertyName = "TipoTexto", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Nivel", DataPropertyName = "NivelTexto", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Poder", DataPropertyName = "Poder", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Resistencia", DataPropertyName = "Resistencia", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Costo", DataPropertyName = "Costo", Width = 80 });

            Controls.AddRange(new Control[]{
                lblId, txtId, lblNombre, txtNombre, lblTipo, cboTipo, lblNivel, cboNivel,
                lblPoder, nudPoder, lblResistencia, nudResistencia, lblCosto, nudCosto,
                btnAgregar, btnCerrar, grid
            });

            // Cargar datos iniciales al grid
            RefrescarGrid();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (!int.TryParse(txtId.Text.Trim(), out id) || id <= 0)
                {
                    MessageBox.Show("IdCriatura invalido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("El nombre es requerido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // costo desde NumericUpDown
                int costo = (int)nudCosto.Value;

                // Validar combos
                if (cboTipo.SelectedIndex < 0)
                {
                    MessageBox.Show("Debe seleccionar el tipo.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cboNivel.SelectedIndex < 0)
                {
                    MessageBox.Show("Debe seleccionar el nivel.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Entidad completa
                var ent = new Miticax.Entidades.CriaturaEntidad
                {
                    IdCriatura = id,
                    Nombre = txtNombre.Text.Trim(),
                    Costo = costo,
                    Poder = (int)nudPoder.Value,
                    Resistencia = (int)nudResistencia.Value,
                    Tipo = cboTipo.SelectedItem.ToString().Trim(),
                    Nivel = cboNivel.SelectedIndex + 1   // 1..5
                };

                var srv = UiServiciosHelper.CriaturaService();
                string error;
                var resultado = srv.RegistrarCriatura(ent, out error);

                bool exito = resultado.Exito;
                string msg = UiServiciosHelper.ExtraerMensaje(resultado) ?? error;

                if (!exito)
                {
                    MessageBox.Show(string.IsNullOrWhiteSpace(msg) ? "Operacion no completada" : msg,
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("El registro se ha ingresado correctamente",
                                "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // limpiar y refrescar
                txtId.Clear();
                txtNombre.Clear();
                nudCosto.Value = nudCosto.Minimum;
                nudPoder.Value = nudPoder.Minimum;
                nudResistencia.Value = nudResistencia.Minimum;
                cboTipo.SelectedIndex = -1;
                cboNivel.SelectedIndex = -1;
                txtId.Focus();

                RefrescarGrid();
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("No se pueden ingresar mas registros", "Limite", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al registrar la criatura.\n" + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // ---- helpers locales para costo ----
        private int LeerCostoDesdeUi()
        {
            // 1) buscar TextBox llamado "txtCosto"
            var arr = this.Controls.Find("txtCosto", true); // busca en anidados
            for (int i = 0; i < arr.Length; i++)
            {
                var tb = arr[i] as TextBox;
                if (tb != null)
                {
                    int c;
                    if (int.TryParse(tb.Text.Trim(), out c) && c >= 0) return c;
                    return -1;
                }
            }
            // 2) buscar NumericUpDown llamado "nudCosto"
            arr = this.Controls.Find("nudCosto", true);
            for (int i = 0; i < arr.Length; i++)
            {
                var nud = arr[i] as NumericUpDown;
                if (nud != null) return (int)nud.Value;
            }
            return -1; // no hay control de costo
        }

        private void LimpiarCostoEnUi()
        {
            var arr = this.Controls.Find("txtCosto", true);
            for (int i = 0; i < arr.Length; i++)
            {
                var tb = arr[i] as TextBox;
                if (tb != null) tb.Clear();
            }
            arr = this.Controls.Find("nudCosto", true);
            for (int i = 0; i < arr.Length; i++)
            {
                var nud = arr[i] as NumericUpDown;
                if (nud != null) nud.Value = nud.Minimum;
            }
        }


        private void RefrescarGrid()
        {
            try
            {
                // Obtener snapshot desde Datos (instancia) y mapear a filas para mostrar textos
                var arr = _criaturaDatos.GetAllSnapshot();
                grid.DataSource = ConstruirFilasGrid(arr);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al cargar los datos.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DTO para el grid
        private class CriaturaGridFila
        {
            public int IdCriatura { get; set; }
            public string Nombre { get; set; }
            public string TipoTexto { get; set; }
            public string NivelTexto { get; set; }
            public int Poder { get; set; }
            public int Resistencia { get; set; }
            public int Costo { get; set; }
        }

        private object ConstruirFilasGrid(CriaturaEntidad[] arr)
        {
            if (arr == null) return null;

            // contar validos
            int count = 0;
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] != null) count++;

            var result = new CriaturaGridFila[count];
            int j = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                var c = arr[i];
                if (c == null) continue;

                var fila = new CriaturaGridFila();
                fila.IdCriatura = c.IdCriatura;
                fila.Nombre = c.Nombre;
                fila.TipoTexto = c.Tipo;
                fila.NivelTexto = NivelATexto(c.Nivel);
                fila.Poder = c.Poder;
                fila.Resistencia = c.Resistencia;
                fila.Costo = c.Costo;

                result[j++] = fila;
            }
            return result;
        }

        private string NivelATexto(int nivel)
        {
            if (nivel == 1) return "01-Iniciado";
            if (nivel == 2) return "02-Aprendiz";
            if (nivel == 3) return "03-Estudiante";
            if (nivel == 4) return "04-Avanzado";
            if (nivel == 5) return "05-Maestro";
            return "N/A";
        }
    }
}
