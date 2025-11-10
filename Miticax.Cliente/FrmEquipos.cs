//UNED
//Mitica X
//Jorge Arias Melendez
//tercer cuatrimestre 2025
//Formulario: Equipos (listar equipos y ver miembros del equipo seleccionado).

using System;
using System.Windows.Forms;
using Miticax.Logica;

namespace Miticax.Cliente
{
    public partial class FrmEquipos : Form
    {
        private readonly ClienteTcpLogica cliente;

        public FrmEquipos(ClienteTcpLogica c)
        {
            InitializeComponent();
            cliente = c;
            cliente.EventoLineaRecibida += Cliente_EventoLineaRecibida;
        }

        private void FrmEquipos_FormClosed(object? sender, FormClosedEventArgs e)
        {
            cliente.EventoLineaRecibida -= Cliente_EventoLineaRecibida;
        }

        private void btnListarEquipos_Click(object? sender, EventArgs e)
        {
            grdEquipos.Rows.Clear();
            grdMiembros.Rows.Clear();
            cliente.EnviarComando("EQUIPO_LISTAR");
        }

        private void btnVerMiembros_Click(object? sender, EventArgs e)
        {
            if (grdEquipos.CurrentRow == null) return;
            string idEquipo = (string)(grdEquipos.CurrentRow.Cells[0].Value ?? "");
            grdMiembros.Rows.Clear();
            cliente.EnviarComando("EQUIPO_MIEMBROS", idEquipo);
        }

        private void Cliente_EventoLineaRecibida(string linea)
        {
            if (this.InvokeRequired) { this.BeginInvoke(new Action<string>(Cliente_EventoLineaRecibida), linea); return; }

            // EQUIPO|id|nombre
            if (linea.StartsWith("EQUIPO|", StringComparison.OrdinalIgnoreCase))
            {
                string[] p = linea.Split('|');
                string id = p.Length > 1 ? p[1] : "";
                string nombre = p.Length > 2 ? p[2] : "";
                grdEquipos.Rows.Add(new string[] { id, nombre });
                return;
            }
            if (linea.StartsWith("EQUIPO_FIN", StringComparison.OrdinalIgnoreCase))
            {
                lblEstadoEquipos.Text = "Equipos: " + grdEquipos.Rows.Count;
                return;
            }

            // EQ_MIEMBRO|id|nombre|nivel
            if (linea.StartsWith("EQ_MIEMBRO|", StringComparison.OrdinalIgnoreCase))
            {
                string[] p = linea.Split('|');
                string id = p.Length > 1 ? p[1] : "";
                string nombre = p.Length > 2 ? p[2] : "";
                string nivel = p.Length > 3 ? p[3] : "";
                grdMiembros.Rows.Add(new string[] { id, nombre, nivel });
                return;
            }
            if (linea.StartsWith("EQ_FIN", StringComparison.OrdinalIgnoreCase))
            {
                lblEstadoMiembros.Text = "Miembros: " + grdMiembros.Rows.Count;
                return;
            }
        }
    }
}
