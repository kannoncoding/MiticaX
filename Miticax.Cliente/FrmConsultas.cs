//UNED
//Mitica X
//Jorge Arias Melendez
//tercer cuatrimestre 2025
//Formulario: Consultas (Top10, Historial, Rondas) mostrado en un grid.

using System;
using System.Windows.Forms;
using Miticax.Logica;

namespace Miticax.Cliente
{
    public partial class FrmConsultas : Form
    {
        private readonly ClienteTcpLogica cliente;

        public FrmConsultas(ClienteTcpLogica c)
        {
            InitializeComponent();
            cliente = c;
            cliente.EventoLineaRecibida += Cliente_EventoLineaRecibida;
        }

        private void FrmConsultas_FormClosed(object? sender, FormClosedEventArgs e)
        {
            cliente.EventoLineaRecibida -= Cliente_EventoLineaRecibida;
        }

        private void btnTop10_Click(object? sender, EventArgs e)
        {
            grd.Rows.Clear();
            cliente.EnviarComando("CONSULTA_TOP10");
            lblEstado.Text = "Consultando Top10...";
        }

        private void btnHistorial_Click(object? sender, EventArgs e)
        {
            grd.Rows.Clear();
            cliente.EnviarComando("CONSULTA_HISTORIAL");
            lblEstado.Text = "Consultando historial...";
        }

        private void btnRondas_Click(object? sender, EventArgs e)
        {
            grd.Rows.Clear();
            cliente.EnviarComando("CONSULTA_RONDAS");
            lblEstado.Text = "Consultando rondas...";
        }

        private void Cliente_EventoLineaRecibida(string linea)
        {
            if (this.InvokeRequired) { this.BeginInvoke(new Action<string>(Cliente_EventoLineaRecibida), linea); return; }

            // Top10: CONS_ITEM|pos|jugador|puntaje
            if (linea.StartsWith("CONS_ITEM|", StringComparison.OrdinalIgnoreCase))
            {
                string[] p = linea.Split('|');
                string pos = p.Length > 1 ? p[1] : "";
                string jugador = p.Length > 2 ? p[2] : "";
                string puntaje = p.Length > 3 ? p[3] : "";
                grd.Rows.Add(new string[] { pos, jugador, puntaje });
                return;
            }

            // Historial: CONS_HIST|fecha|evento|detalle
            if (linea.StartsWith("CONS_HIST|", StringComparison.OrdinalIgnoreCase))
            {
                string[] p = linea.Split('|');
                string f = p.Length > 1 ? p[1] : "";
                string ev = p.Length > 2 ? p[2] : "";
                string det = p.Length > 3 ? p[3] : "";
                grd.Rows.Add(new string[] { f, ev, det });
                return;
            }

            // Rondas: CONS_RONDA|n|estado|detalle
            if (linea.StartsWith("CONS_RONDA|", StringComparison.OrdinalIgnoreCase))
            {
                string[] p = linea.Split('|');
                string n = p.Length > 1 ? p[1] : "";
                string est = p.Length > 2 ? p[2] : "";
                string det = p.Length > 3 ? p[3] : "";
                grd.Rows.Add(new string[] { n, est, det });
                return;
            }

            if (linea.StartsWith("CONS_FIN", StringComparison.OrdinalIgnoreCase))
            {
                lblEstado.Text = "Consulta finalizada: " + grd.Rows.Count + " filas.";
                return;
            }
        }
    }
}
