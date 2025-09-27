//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Entidad que representa una ronda dentro de una batalla (POCO).

using System;

namespace Miticax.Entidades
{
    public class RondaEntidad
    {
        public int IdRonda { get; set; }

        public int IdBatalla { get; set; }

        public int IdJugador1 { get; set; }

        public int IdCriatura1 { get; set; }

        public int IdJugador2 { get; set; }

        public int IdCriatura2 { get; set; }

        public int GanadorRonda { get; set; }
    }
}
