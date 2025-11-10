//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Formulario WinForms para probar el Cliente TCP: conectar, enviar comandos y ver bitacora.

using System;
using System.Windows.Forms;
using Miticax.Logica;

namespace Miticax.Cliente
{
    public partial class FrmClienteTcp : Form
    {
        // Instancia de la logica del cliente TCP.
        private readonly ClienteTcpLogica cliente;

        public FrmClienteTcp()
        {
            InitializeComponent(); // generado por el Designer

            // Crear la instancia y suscribirse a los eventos.
            cliente = new ClienteTcpLogica();
            cliente.EventoEstado += Cliente_EventoEstado;
            cliente.EventoLineaRecibida += Cliente_EventoLineaRecibida;

            // Valores por defecto en la UI para pruebas rapidas.
            txtHost.Text = "127.0.0.1"; // o IP/hostname de tu servidor
            txtPuerto.Text = "14100";    // puerto segun Phase 3
        }

        // Evento: click en Conectar -> intenta abrir conexion con los datos de la UI.
        private void BtnConectar_Click(object? sender, EventArgs e)
        {
            try
            {
                string host = txtHost.Text.Trim();
                if (string.IsNullOrWhiteSpace(host))
                {
                    MessageBox.Show("Debe indicar el host o IP del servidor.", "Validacion",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtPuerto.Text.Trim(), out int puerto))
                {
                    MessageBox.Show("Puerto no valido.", "Validacion",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                cliente.Conectar(host, puerto);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento: click en Desconectar -> cierra la conexion actual si existe.
        private void BtnDesconectar_Click(object? sender, EventArgs e)
        {
            try
            {
                cliente.Desconectar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desconectar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento: click en Enviar -> envia exactamente el texto del textbox como una linea.
        private void BtnEnviar_Click(object? sender, EventArgs e)
        {
            try
            {
                string linea = txtEnviar.Text;
                if (string.IsNullOrWhiteSpace(linea))
                {
                    MessageBox.Show("Ingrese una linea para enviar.", "Validacion",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                cliente.EnviarLinea(linea);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Boton de atajo: envia un PING segun nuestro protocolo de texto.
        private void BtnPing_Click(object? sender, EventArgs e)
        {
            try
            {
                cliente.EnviarComando("PING"); // el servidor deberia responder con PONG
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar PING: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Manejador del evento de estado: escribe en la bitacora (UI thread-safe).
        private void Cliente_EventoEstado(string msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(Cliente_EventoEstado), msg);
                return;
            }
            txtLog.AppendText("[ESTADO] " + msg + Environment.NewLine);
        }

        // Manejador del evento de linea recibida: escribe texto crudo en la bitacora.
        private void Cliente_EventoLineaRecibida(string linea)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(Cliente_EventoLineaRecibida), linea);
                return;
            }
            txtLog.AppendText("<< " + linea + Environment.NewLine);

            // Aqui podras enrutar respuestas especificas (LOGIN_OK, PONG, etc.)
        }
    }
}
