//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Entidad simple para describir el estado de una conexion TCP (solo propiedades).

namespace Miticax.Entidades
{
    // Entidad de solo propiedades, sin referencias a I/O ni UI.
    public class ClienteConexionEntidad
    {
        // Indice del slot en el arreglo de conexiones.
        public int Indice { get; set; }

        // Direccion remota como texto (ej: 192.168.1.5:54321).
        public string Remoto { get; set; } = string.Empty;

        // Estado textual de la conexion (Conectado, Cerrado, Error).
        public string Estado { get; set; } = "Cerrado";

        // Marca de tiempo legible cuando se creo la conexion.
        public string HoraInicio { get; set; } = string.Empty;

        // Marca de tiempo legible cuando se cerro la conexion.
        public string HoraFin { get; set; } = string.Empty;
    }
}
