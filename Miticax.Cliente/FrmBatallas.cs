//UNED
//Mitica X
//Jorge Arias Melendez
//tercer cuatrimestre 2025
//Formulario: Batallas (flujo basico de emparejamiento y mensajes de batalla).

using System;
using System.Windows.Forms;
using Miticax.Logica;

namespace Miticax.Cliente
{
    public partial class FrmBatallas : Form
    {
        private readonly ClienteTcpLogica cliente;
        private string idBatalla = "";

        public FrmBatallas(ClienteTcpLogica c)
        {
            InitializeComponent();
            cliente = c;
            cliente.EventoLineaRecibida += Cliente_EventoLineaRecibida;
        }

        private void FrmBatallas_FormClosed(object? sender, FormClosedEventArgs e)
        {
            cliente.EventoLineaRecibida -= Cliente_EventoLineaRecibida;
        }

        private void btnBuscar_Click(object? sender, EventArgs e)
        {
            txtBitacora.Clear();
            idBatalla = "";
            cliente.EnviarComando("BATALLA_BUSCAR");
            lblEstado.Text = "Buscando oponente...";
        }

        private void btnRendirse_Click(object? sender, EventArgs e)
        {
            if (idBatalla.Length == 0) return;
            cliente.EnviarComando("BATALLA_RENDIRSE", idBatalla);
        }

        private void Cliente_EventoLineaRecibida(string linea)
        {
            if (this.InvokeRequired) { this.BeginInvoke(new Action<string>(Cliente_EventoLineaRecibida), linea); return; }

            if (linea.StartsWith("BATALLA_MSG|", StringComparison.OrdinalIgnoreCase))
            {
                string[] p = linea.Split('|');
                string msg = p.Length > 1 ? p[1] : "";
                txtBitacora.AppendText(msg + Environment.NewLine);
                return;
            }

            if (linea.StartsWith("BATALLA_OK|", StringComparison.OrdinalIgnoreCase))
            {
                string[] p = linea.Split('|');
                idBatalla = p.Length > 1 ? p[1] : "";
                lblEstado.Text = "En batalla: " + idBatalla;
                return;
            }

            if (linea.StartsWith("BATALLA_FIN|", StringComparison.OrdinalIgnoreCase))
            {
                string[] p = linea.Split('|');
                string resultado = p.Length > 1 ? p[1] : "finalizada";
                lblEstado.Text = "Batalla " + resultado;
                idBatalla = "";
                return;
            }
        }
    }
}
