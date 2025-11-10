//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Punto de entrada del Cliente TCP de MiticaX

using System;
using System.Windows.Forms;

namespace Miticax.Cliente
{
    internal static class Program
    {
        /// <summary>
        /// Metodo principal: inicia la aplicacion cliente y abre el formulario principal TCP.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Inicializa el entorno de Windows Forms (.NET 8)
            ApplicationConfiguration.Initialize();

            // Inicia la aplicacion mostrando el formulario del cliente TCP
            Application.Run(new FrmClienteTcp());
        }
    }
}
