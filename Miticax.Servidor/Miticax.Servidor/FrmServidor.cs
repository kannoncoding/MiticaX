//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Formulario WinForms para iniciar/detener el servidor TCP y mostrar la bitacora.

using System;
using System.Windows.Forms;
using Miticax.Datos;
using Miticax.Logica;

namespace Miticax.Presentacion
{
    public partial class FrmServidor : Form
    {
        // Instancias de capa de datos y logica del servidor.
        private readonly ServidorTcpDatos datos;
        private readonly ServidorTcpLogica logica;

        // Constructor del formulario.
        public FrmServidor()
        {
            InitializeComponent(); // Inicializa controles generados por el diseñador.

            // Crea las capas.
            datos = new ServidorTcpDatos();
            logica = new ServidorTcpLogica(datos);

            // Suscribe el evento de bitacora para imprimir en el RichTextBox.
            logica.OnBitacora += Logica_OnBitacora;
        }

        // Evento de bitacora: asegura invocar en el hilo de UI.
        private void Logica_OnBitacora(string linea)
        {
            // Si requiere Invoke, usa Invoke para no romper el hilo de UI.
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(Logica_OnBitacora), linea);
                return;
            }

            // Agrega texto a la bitacora y hace scroll al final.
            rtbBitacora.AppendText(linea + Environment.NewLine);
        }

        // Boton "Iniciar": arranca el servidor en el puerto 14100.
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                // Lee el puerto del TextBox si lo deseas; por defecto 14100.
                int puerto = 14100;
                if (int.TryParse(txtPuerto.Text.Trim(), out int p) && p > 0 && p < 65536)
                    puerto = p;

                // Inicia el servidor.
                logica.IniciarServidor(puerto);

                // Actualiza estado de botones.
                btnIniciar.Enabled = false;
                btnDetener.Enabled = true;
            }
            catch (Exception ex)
            {
                // Muestra error y permite continuar (manejo de excepciones).
                MessageBox.Show("Error al iniciar: " + ex.Message, "Servidor",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Boton "Detener": detiene el listener y cierra conexiones.
        private void btnDetener_Click(object sender, EventArgs e)
        {
            try
            {
                // Detiene el servidor.
                logica.DetenerServidor();

                // Actualiza estado de botones.
                btnIniciar.Enabled = true;
                btnDetener.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al detener: " + ex.Message, "Servidor",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Boton "Limpiar": borra la bitacora visual (no afecta el servidor).
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            rtbBitacora.Clear();
        }
    }
}
