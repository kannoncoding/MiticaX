//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Entidad que representa una batalla entre dos jugadores (POCO).

using System;

namespace Miticax.Entidades
{
    public class BatallaEntidad
    {
        public int IdBatalla { get; set; }

        public int IdJugador1 { get; set; }

        public int IdEquipo1 { get; set; }

        public int IdJugador2 { get; set; }

        public int IdEquipo2 { get; set; }

        public int Ganador { get; set; }

        public DateTime Fecha { get; set; }
    }
}
