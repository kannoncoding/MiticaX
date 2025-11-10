//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Punto de entrada de la app WinForms del Servidor TCP.

using Miticax.Presentacion;
using System;
using System.Windows.Forms;

namespace Miticax.Servidor
{
    internal static class Program
    {
        // El arranque de la app (WinForms).
        [STAThread]
        static void Main()
        {
            // Inicializa configuracion de WinForms (DPI, fuentes).
            ApplicationConfiguration.Initialize();

            // Abre el formulario principal del servidor.
            // IMPORTANTE: Asegurate de que el nombre de la clase coincida con tu form real.
            Application.Run(new FrmServidor());
        }
    }
}
