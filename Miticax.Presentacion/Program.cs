//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Punto de entrada de la app WinForms; abre el menu principal

using System;
using System.Windows.Forms;

namespace Miticax.Presentacion
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmMenuPrincipal());
        }
    }
}
