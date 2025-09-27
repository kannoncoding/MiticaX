//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Punto de entrada de la aplicacion WinForms.

using System;
using System.Windows.Forms;

namespace Miticax.Presentacion
{
    internal static class Program
    {
        /// <summary>
        /// Metodo principal. Marcado STA para WinForms.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configuracion de estilos y high DPI (plantilla .NET 8).
            ApplicationConfiguration.Initialize();

            // Iniciar la aplicacion con Form1 como formulario principal.
            Application.Run(new Form1());
        }
    }
}
