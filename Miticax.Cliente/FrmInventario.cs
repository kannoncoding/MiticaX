//UNED
//Mitica X
//Jorge Arias Melendez
//tercer cuatrimestre 2025
//Formulario: Inventario de criaturas (listar desde servidor y mostrar en grid).

using System;
using System.Windows.Forms;
using Miticax.Logica;

namespace Miticax.Cliente
{
    public partial class FrmInventario : Form
    {
        private readonly ClienteTcpLogica cliente;

        public FrmInventario(ClienteTcpLogica c)
        {
            InitializeComponent();
            cliente = c;
            // Suscribir a eventos del socket para recibir items.
            cliente.EventoLineaRecibida += Cliente_EventoLineaRecibida;
        }

        private void FrmInventario_FormClosed(object? sender, FormClosedEventArgs e)
        {
            // desuscribir para evitar duplicidad de handlers.
            cliente.EventoLineaRecibida -= Cliente_EventoLineaRecibida;
        }

        private void btnRefrescar_Click(object? sender, EventArgs e)
        {
            // Limpia el grid y solicita la lista al servidor.
            grd.Rows.Clear();
            cliente.EnviarComando("INVENTARIO_LISTAR");
        }

        private void Cliente_EventoLineaRecibida(string linea)
        {
            if (this.InvokeRequired) { this.BeginInvoke(new Action<string>(Cliente_EventoLineaRecibida), linea); return; }

            // Espera: INV_ITEM|id|nombre|tipo|nivel|poder
            if (linea.StartsWith("INV_ITEM", StringComparison.OrdinalIgnoreCase))
            {
                string[] p = linea.Split('|');
                string id = p.Length > 1 ? p[1] : "";
                string nombre = p.Length > 2 ? p[2] : "";
                string tipo = p.Length > 3 ? p[3] : "";
                string nivel = p.Length > 4 ? p[4] : "";
                string poder = p.Length > 5 ? p[5] : "";
                grd.Rows.Add(new string[] { id, nombre, tipo, nivel, poder });
                return;
            }

            // Fin del lote
            if (linea.StartsWith("INV_FIN", StringComparison.OrdinalIgnoreCase))
            {
                lblEstado.Text = "Inventario actualizado: " + grd.Rows.Count + " items.";
                return;
            }
        }
    }
}
