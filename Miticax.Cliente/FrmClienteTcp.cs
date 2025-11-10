//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Formulario WinForms para probar el Cliente TCP: conectar, enviar comandos, login y ver bitacora.

using System;
using System.Windows.Forms;
using Miticax.Logica;

namespace Miticax.Cliente
{
    public partial class FrmClienteTcp : Form
    {
        // Instancia de la logica del cliente TCP.
        private readonly ClienteTcpLogica cliente;

        // Estado simple de sesion en cliente (solo memoria del lado UI).
        private bool sesionAutenticada = false;
        private string usuarioActual = string.Empty;
        private string nombreActual = string.Empty;
        private string rolActual = string.Empty;

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

            // Estado inicial de login.
            ActualizarUiLogin(false, "No autenticado");
        }

        // Click en Conectar: intenta abrir conexion con host/puerto indicados.
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

        // Click en Desconectar: cierra la conexion actual si existe.
        private void BtnDesconectar_Click(object? sender, EventArgs e)
        {
            try
            {
                cliente.Desconectar();
                // Al desconectar, invalidamos sesion local.
                sesionAutenticada = false;
                usuarioActual = string.Empty;
                nombreActual = string.Empty;
                rolActual = string.Empty;
                ActualizarUiLogin(false, "Conexion cerrada. No autenticado");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desconectar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Click en Enviar: envia exactamente la linea escrita.
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

        // Boton PING: prueba de vida con el servidor.
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

        // Boton LOGIN: envia credenciales al servidor.
        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            try
            {
                string u = txtUsuario.Text.Trim();
                string c = txtClave.Text; // puede venir vacio; el servidor valida

                if (string.IsNullOrWhiteSpace(u))
                {
                    MessageBox.Show("Ingrese el usuario.", "Validacion",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Enviar comando de login segun protocolo: LOGIN|usuario|clave
                cliente.EnviarComando("LOGIN", u, c);

                // Feedback visual minimo mientras responde el servidor.
                lblLoginEstado.Text = "Validando...";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar LOGIN: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento de estado: escribe en bitacora.
        private void Cliente_EventoEstado(string msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(Cliente_EventoEstado), msg);
                return;
            }
            txtLog.AppendText("[ESTADO] " + msg + Environment.NewLine);
        }

        // Evento de linea recibida: enruta respuestas del servidor.
        private void Cliente_EventoLineaRecibida(string linea)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(Cliente_EventoLineaRecibida), linea);
                return;
            }

            txtLog.AppendText("<< " + linea + Environment.NewLine);

            // Parseo simple por '|' sin LINQ ni colecciones genericas.
            // Ej: LOGIN_OK|usuario|nombre|rol
            if (linea.StartsWith("LOGIN_OK", StringComparison.OrdinalIgnoreCase))
            {
                string[] partes = linea.Split('|'); // arrays permitidos
                usuarioActual = partes.Length > 1 ? partes[1] : string.Empty;
                nombreActual = partes.Length > 2 ? partes[2] : string.Empty;
                rolActual = partes.Length > 3 ? partes[3] : string.Empty;

                sesionAutenticada = true;
                string texto = "Autenticado";
                if (!string.IsNullOrEmpty(usuarioActual))
                    texto += ": " + usuarioActual;
                if (!string.IsNullOrEmpty(rolActual))
                    texto += " (" + rolActual + ")";

                ActualizarUiLogin(true, texto);
                return;
            }

            // Ej: LOGIN_ERR|mensaje
            if (linea.StartsWith("LOGIN_ERR", StringComparison.OrdinalIgnoreCase))
            {
                string[] partes = linea.Split('|');
                string msg = partes.Length > 1 ? partes[1] : "Error de autenticacion.";
                sesionAutenticada = false;
                usuarioActual = string.Empty;
                nombreActual = string.Empty;
                rolActual = string.Empty;

                ActualizarUiLogin(false, "Error: " + msg);
                MessageBox.Show(msg, "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Aqui puedes enrutar otros comandos-respuestas (PONG, etc.).
        }

        // Actualiza estado visual de login (habilita/deshabilita controles y texto).
        private void ActualizarUiLogin(bool ok, string texto)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<bool, string>(ActualizarUiLogin), ok, texto);
                return;
            }

            lblLoginEstado.Text = texto;

            // Si ya esta autenticado, bloquea campos para evitar reintentos accidentales.
            txtUsuario.Enabled = !ok;
            txtClave.Enabled = !ok;
            btnLogin.Enabled = !ok;

            // Segun tu flujo, tambien puedes habilitar modulos de Inventario/Equipos/Batallas aqui.
            // Por ahora, solo visual.
        }
    }
}
