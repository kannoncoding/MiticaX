//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Punto de entrada WinForms del proyecto de presentacion

using System;
using System.Windows.Forms;

namespace Miticax.Presentacion
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize(); // inicializa estilos WinForms
            Application.Run(new Form1());          // iniciar con Form1
        }
    }
}
