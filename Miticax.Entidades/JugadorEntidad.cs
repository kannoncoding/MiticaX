//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Entidad que representa un jugador del sistema (POCO; solo propiedades).

using System;

namespace Miticax.Entidades
{
    public class JugadorEntidad
    {
        public int IdJugador { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaNacimiento { get; set; }

        public int NivelJugador { get; set; }

        public int Cristales { get; set; } = 100;

        public int BatallasGanadas { get; set; } = 0;
    }
}
